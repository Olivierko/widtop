// ReSharper disable NotAccessedField.Local

using System;
using System.Linq;
using HidSharp;
using HidSharp.Reports.Input;
using Widtop.Utility;

namespace Widtop.Hid
{
    public class Connector
    {
        private readonly Device _device;

        private bool _initialized;
        private HidDevice _virtualDevice;
        private HidDevice _physicalDevice;
        private HidStream _virtualStream;
        private HidStream _physicalStream;
        private HidDeviceInputReceiver _virtualReceiver;
        private HidDeviceInputReceiver _physicalReceiver;
        private QueuedTimer _connectionTimer;

        public bool IsConnected =>
            _virtualDevice != null &&
            _virtualReceiver != null &&
            _virtualReceiver.IsRunning &&
            _virtualStream != null &&
            _physicalDevice != null &&
            _physicalReceiver != null &&
            _physicalReceiver.IsRunning &&
            _physicalStream != null;

        public Connector(Device device)
        {
            _device = device;
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void OnDeviceListChanged(object sender, DeviceListChangedEventArgs e)
        {
            var allDevices = DeviceList.Local.GetHidDevices().ToList();

            var requireReset = 
                _virtualDevice != null && 
                !allDevices.Contains(_virtualDevice) ||
                _physicalDevice != null && 
                !allDevices.Contains(_physicalDevice);

            if (!requireReset)
            {
                return;
            }

            Log("Device was disconnected.");
            Reset();
        }

        private void OnReceiverReceived(object sender, EventArgs args)
        {
            try
            {
                var receiver = (HidDeviceInputReceiver)sender;

                if (receiver.Stream?.Device == null)
                {
                    return;
                }

                var length = receiver.Stream.Device.GetMaxInputReportLength();

                var buffer = new byte[length];

                if (receiver.TryRead(buffer, 0, out var report))
                {
                    Log($"Received report with id: {report.ReportID}, type: {report.ReportType}");

                    report.Read(buffer, 0, (bytes, offset, item, dataItem) =>
                    {
                        Log(string.Join(" ", bytes));
                        _device.OnReportReceived(buffer);
                    });
                }
                else
                {
                    Log("Failed to read report.");
                }
            }
            catch (Exception e)
            {
                Log($"Exception while reading report occured: {e}");
            }
        }

        private void SetupDevices()
        {
            Reset();

            var virtualWiredDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            var virtualWirelessDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            if (SetupDevice(virtualWiredDevice, out _virtualStream, out _virtualReceiver))
            {
                _virtualDevice = virtualWiredDevice;
            }
            else if (SetupDevice(virtualWirelessDevice, out _virtualStream, out _virtualReceiver))
            {
                _virtualDevice = virtualWirelessDevice;
            }
            else
            {
                Reset();
                Log("Failed connecting to virtual device.");
                return;
            }

            var physicalWiredDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

            var physicalWirelessDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

            if (SetupDevice(physicalWiredDevice, out _physicalStream, out _physicalReceiver))
            {
                _physicalDevice = physicalWiredDevice;
            }
            else if (SetupDevice(physicalWirelessDevice, out _physicalStream, out _physicalReceiver))
            {
                _physicalDevice = physicalWirelessDevice;
            }
            else
            {
                Reset();
                Log("Failed connecting to physical device.");
                return;
            }

            if (IsConnected)
            {
                Log("Successfully connected to device.");
                _device.OnConnected();
            }
            else
            {
                Log("Failed connecting to device.");
                Reset();
            }
        }

        private bool SetupDevice(HidDevice device, out HidStream stream, out HidDeviceInputReceiver receiver)
        {
            stream = null;
            receiver = null;
            if (device == null)
            {
                return false;
            }

            try
            {
                if (!device.TryOpen(out stream))
                {
                    return false;
                }

                var descriptor = device.GetReportDescriptor();
                receiver = descriptor.CreateHidDeviceInputReceiver();
                receiver.Received += OnReceiverReceived;
                receiver.Start(stream);
            }
            catch (Exception e)
            {
                Log($"Exception while setting up device occured: {e}");
                return false;
            }

            return receiver.IsRunning;
        }

        private void EnsureConnection()
        {
            if (IsConnected)
            {
                return;
            }

            SetupDevices();
        }

        public void Initialize()
        {
            if (_initialized)
            {
                throw new Exception("Initialize has already been invoked.");
            }

            // poll connections once per second starting immediately
            _connectionTimer = new QueuedTimer(
                state => EnsureConnection(), 
                1000
            );

            _device.OnInitialize(this);

            DeviceList.Local.Changed += OnDeviceListChanged;

            _initialized = true;
        }

        public void IssueReport(params byte[] parameters)
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                var length = _virtualDevice.GetMaxOutputReportLength();

                var buffer = new byte[length];
                parameters.CopyTo(buffer, 0);

                Log("Issued report:");
                Log(string.Join(" ", buffer));

                _virtualStream.Write(buffer);
            }
            catch (Exception e)
            {
                Log($"Exception while issuing a report occured: {e}");
            }
        }

        public void Reset()
        {
            _virtualStream?.Dispose();
            _physicalStream?.Dispose();

            if (_virtualReceiver != null)
            {
                _virtualReceiver.Received -= OnReceiverReceived;
            }

            if (_physicalReceiver != null)
            {
                _physicalReceiver.Received -= OnReceiverReceived;
            }

            _virtualDevice = null;
            _virtualStream = null;
            _virtualReceiver = null;
            _physicalDevice = null;
            _physicalStream = null;
            _physicalReceiver = null;
        }
    }
}
