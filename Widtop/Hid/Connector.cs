// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable NotAccessedField.Local

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;

namespace Widtop.Hid
{
    public class Connector
    {
        private const byte DeviceId = 0x01;
        private const int ConnectionInterval = 1000;

        private readonly Device _device;
        private readonly Dictionary<string, bool> _connections;

        private HidDevice _hidDevice;
        private HidStream _hidStream;
        private Timer _connectionTimer;

        private string Virtual { get; set; }
        private string Physical { get; set; }

        public Connector(Device device)
        {
            _device = device;
            _connections = new Dictionary<string, bool>();
        }

        private static void Log(string message) { }

        private static void Log(byte[] buffer)
        {
            Log("Raw:");
            Log(string.Join(" ", buffer));

            var bits = string.Empty;
            foreach (var @byte in buffer)
            {
                bits += Convert.ToString(@byte, 2) + " ";
            }

            Log("Bits: ");
            Log(bits);

            var hex = string.Empty;
            foreach (var @byte in buffer)
            {
                hex += $"0x{@byte:X} ";
            }

            Log("Hex: ");
            Log(hex);
        }

        private void OnStreamClosed(object sender, EventArgs args)
        {
            var stream = (DeviceStream)sender;

            stream.Closed -= OnStreamClosed;
            stream.InterruptRequested -= OnStreamInterruptRequested;

            if (stream.Device == null)
            {
                return;
            }

            Log($"Stream closed for: {stream.Device.DevicePath}");
            _connections[stream.Device.DevicePath] = false;
        }

        private void OnStreamInterruptRequested(object sender, EventArgs args)
        {
            var stream = (DeviceStream)sender;

            if (stream.Device == null)
            {
                return;
            }

            Log($"Stream interrupt requested for: {stream.Device.DevicePath}");
            _connections[stream.Device.DevicePath] = false;
        }

        private void OnReceiverStarted(object sender, EventArgs args)
        {
            var receiver = (HidDeviceInputReceiver)sender;

            if (receiver.Stream?.Device == null)
            {
                return;
            }

            Log($"Receiver started for: {receiver.Stream.Device.DevicePath}");
            _connections[receiver.Stream.Device.DevicePath] = true;
        }

        private void OnReceiverStopped(object sender, EventArgs args)
        {
            var receiver = (HidDeviceInputReceiver)sender;

            if (receiver.Stream?.Device == null)
            {
                return;
            }

            Log($"Receiver stopped for: {receiver.Stream.Device.DevicePath}");
            _connections[receiver.Stream.Device.DevicePath] = false;
        }

        private void OnReceiverReceived(object sender, EventArgs args)
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
                Log($"Report ID: {report.ReportID}, type: {report.ReportType}");

                report.Read(buffer, 0, (bytes, offset, item, dataItem) =>
                {
                    Log(bytes);

                    _device.OnReportReceived(buffer);
                });
            }
            else
            {
                Log("Failed to read report.");
            }
        }

        private void SetupDevices()
        {
            var virtualReceiverDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            var virtualDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            if (SetupDevice(virtualReceiverDevice, out _hidStream))
            {
                _hidDevice = virtualReceiverDevice;
                Virtual = virtualReceiverDevice?.DevicePath;
            }
            else if (SetupDevice(virtualDevice, out _hidStream))
            {
                _hidDevice = virtualDevice;
                Virtual = virtualDevice?.DevicePath;
            }
            else
            {
                Log("Failed connecting to virtual device.");
                Virtual = null;
            }

            var physicalReceiverDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

            var physicalDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

            if (SetupDevice(physicalReceiverDevice, out _))
            {
                Physical = physicalReceiverDevice?.DevicePath;
            }
            else if (SetupDevice(physicalDevice, out _))
            {
                Physical = physicalDevice?.DevicePath;
            }
            else
            {
                Log("Failed connecting to physical device.");
                Physical = null;
            }
        }

        private bool SetupDevice(HidDevice device, out HidStream stream)
        {
            stream = null;
            if (device == null)
            {
                return false;
            }

            if (!device.TryOpen(out stream))
            {
                return false;
            }

            ReportDescriptor descriptor;
            try
            {
                descriptor = device.GetReportDescriptor();
            }
            catch (Exception e)
            {
                Log($"Exception while setting up device occured: {e}");
                return false;
            }

            var receiver = descriptor.CreateHidDeviceInputReceiver();

            stream.Closed += OnStreamClosed;
            stream.InterruptRequested += OnStreamInterruptRequested;
            receiver.Started += OnReceiverStarted;
            receiver.Stopped += OnReceiverStopped;
            receiver.Received += OnReceiverReceived;

            receiver.Start(stream);
            return receiver.IsRunning;
        }

        private void EnsureConnection()
        {
            var connected =
                !string.IsNullOrEmpty(Virtual) &&
                !string.IsNullOrEmpty(Physical) &&
                _connections.ContainsKey(Virtual) &&
                _connections.ContainsKey(Physical) &&
                _connections[Virtual] &&
                _connections[Physical];

            if (connected)
            {
                return;
            }

            SetupDevices();
        }

        public void Initialize()
        {
            // poll connections once per second starting immediately
            _connectionTimer = new Timer(
                state => EnsureConnection(), 
                null, 
                0,
                ConnectionInterval
            );

            _device.OnInitialize(this);
        }

        public void IssueReport(ReportSize reportSize, params byte[] parameters)
        {
            var canIssueReport =
                _hidDevice != null &&
                _hidStream != null &&
                _connections.ContainsKey(_hidDevice.DevicePath) &&
                _connections[_hidDevice.DevicePath];

            if (!canIssueReport)
            {
                return;
            }

            try
            {
                var length = _hidDevice.GetMaxOutputReportLength();

                var buffer = new byte[length];
                buffer[0] = (byte)reportSize;
                buffer[1] = DeviceId;

                parameters.CopyTo(buffer, 2);

                Log($"Issued report at: {DateTime.Now:HH:mm:ss} with ID: {buffer[0]}");
                Log(buffer);

                _hidStream.Write(buffer);
            }
            catch (Exception e)
            {
                Log($"Exception while issuing a report occured: {e}");
            }
        }

        public void Reset()
        {
            Virtual = null;
            Physical = null;
            _connections.Clear();
        }
    }
}
