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
    public class LightspeedConnector
    {
        private const byte DeviceId = 0x01;
        private const int VendorId = 0x046D;
        private const int ConnectionInterval = 1000;
        private const int BatteryInterval = 60000;
        private const int WiredProductId = 0xC090;
        private const int WirelessProductId = 0xC539;
        private const string VirtualPathMatch = "mi_02&col01";
        private const string PhysicalPathMatch = "mi_02&col02";

        private readonly Action<string> _log;
        private readonly Dictionary<string, bool> _connections;
        private readonly List<ReportProcessor> _processors;

        private HidDevice _device;
        private HidStream _stream;
        private Timer _connectionTimer;
        private Timer _batteryTimer;

        private string Virtual { get; set; }
        private string Physical { get; set; }

        public LightspeedConnector(Action<string> log, List<ReportProcessor> processors)
        {
            _log = log;
            _connections = new Dictionary<string, bool>();
            _processors = processors;
        }

        private void OnStreamClosed(object sender, EventArgs args)
        {
            var stream = (DeviceStream)sender;

            // TODO: can be removed?
            stream.Closed -= OnStreamClosed;
            stream.InterruptRequested -= OnStreamInterruptRequested;

            if (stream.Device == null)
            {
                return;
            }

            _log($"Stream closed for: {stream.Device.DevicePath}");
            _connections[stream.Device.DevicePath] = false;
        }

        private void OnStreamInterruptRequested(object sender, EventArgs args)
        {
            var stream = (DeviceStream)sender;

            if (stream.Device == null)
            {
                return;
            }

            _log($"Stream interrupt requested for: {stream.Device.DevicePath}");
            _connections[stream.Device.DevicePath] = false;
        }

        private void OnReceiverStarted(object sender, EventArgs args)
        {
            var receiver = (HidDeviceInputReceiver)sender;

            if (receiver.Stream?.Device == null)
            {
                return;
            }

            _log($"Receiver started for: {receiver.Stream.Device.DevicePath}");
            _connections[receiver.Stream.Device.DevicePath] = true;
        }

        private void OnReceiverStopped(object sender, EventArgs args)
        {
            var receiver = (HidDeviceInputReceiver)sender;

            if (receiver.Stream?.Device == null)
            {
                return;
            }

            _log($"Receiver stopped for: {receiver.Stream.Device.DevicePath}");
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
                _log($"Report ID: {report.ReportID}, type: {report.ReportType}");

                report.Read(buffer, 0, (bytes, offset, item, dataItem) =>
                {
                    LogBuffer(bytes);

                    foreach (var feature in _processors)
                    {
                        if (feature.Process(buffer))
                        {
                            break;
                        }
                    }
                });
            }
            else
            {
                _log("Failed to read report.");
            }
        }

        private void SetupDevices()
        {
            var virtualWirelessDevice = DeviceList.Local
                .GetHidDevices(VendorId, WirelessProductId)
                .FirstOrDefault(x => x.DevicePath.Contains(VirtualPathMatch));

            var virtualWiredDevice = DeviceList.Local
                .GetHidDevices(VendorId, WiredProductId)
                .FirstOrDefault(x => x.DevicePath.Contains(VirtualPathMatch));

            if (SetupDevice(virtualWirelessDevice, out _stream))
            {
                _device = virtualWirelessDevice;
                Virtual = virtualWirelessDevice?.DevicePath;
            }
            else if (SetupDevice(virtualWiredDevice, out _stream))
            {
                _device = virtualWiredDevice;
                Virtual = virtualWiredDevice?.DevicePath;
            }
            else
            {
                _log("Failed connecting to virtual device.");
                Virtual = null;
            }

            var physicalWirelessDevice = DeviceList.Local
                .GetHidDevices(VendorId, WirelessProductId)
                .FirstOrDefault(x => x.DevicePath.Contains(PhysicalPathMatch));

            var physicalWiredDevice = DeviceList.Local
                .GetHidDevices(VendorId, WiredProductId)
                .FirstOrDefault(x => x.DevicePath.Contains(PhysicalPathMatch));

            if (SetupDevice(physicalWirelessDevice, out _))
            {
                Physical = physicalWirelessDevice?.DevicePath;
            }
            else if (SetupDevice(physicalWiredDevice, out _))
            {
                Physical = physicalWiredDevice?.DevicePath;
            }
            else
            {
                _log("Failed connecting to physical device.");
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
                _log($"Exception while setting up device occured: {e}");
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

        private void IssueReport(HidStream stream, HidDevice device, ReportSize reportSize, ReportType reportType, params byte[] parameters)
        {
            var canIssueReport =
                device != null &&
                stream != null &&
                _connections.ContainsKey(device.DevicePath) &&
                _connections[device.DevicePath];

            if (!canIssueReport)
            {
                return;
            }

            try
            {
                var length = device.GetMaxOutputReportLength();

                var buffer = new byte[length];
                buffer[0] = (byte)reportSize;
                buffer[1] = DeviceId;
                buffer[2] = (byte)reportType;

                parameters.CopyTo(buffer, 3);

                _log($"Issued report at: {DateTime.Now:HH:mm:ss} with ID: {buffer[0]}");
                LogBuffer(buffer);

                stream.Write(buffer);
            }
            catch (Exception e)
            {
                _log($"Exception while issuing a report occured: {e}");
            }
        }

        private void LogBuffer(byte[] buffer)
        {
            _log("Raw:");
            _log(string.Join(" ", buffer));

            var bits = string.Empty;
            foreach (var @byte in buffer)
            {
                bits += Convert.ToString(@byte, 2) + " ";
            }
            
            _log("Bits: ");
            _log(bits);

            var hex = string.Empty;
            foreach (var @byte in buffer)
            {
                hex += $"0x{@byte:X} ";
            }

            _log("Hex: ");
            _log(hex);
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

            // poll battery status once per minute starting after 3 seconds
            _batteryTimer = new Timer(
                state => IssueReport(_stream, _device, ReportSize.Short, ReportType.Battery), 
                null, 
                3000,
                BatteryInterval
            );
        }

        public void Reset()
        {
            Virtual = null;
            Physical = null;
            _connections.Clear();
        }
    }
}
