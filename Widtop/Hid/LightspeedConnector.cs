// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HidSharp;
using HidSharp.Reports;

namespace Widtop.Hid
{
    public class LightspeedConnector
    {
        private const int VendorId = 0x046D;
        private const byte DeviceId = 0x01;
        private const int ConnectionInterval = 1000;
        private const int BatteryInterval = 60000;

        private const int WiredProductId = 0xC090;
        private const string VirtualWired = @"\\?\hid#vid_046d&pid_c090&mi_02&col01#8&2928b6d5&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}";
        private const string PhysicalWired = @"\\?\hid#vid_046d&pid_c090&mi_02&col02#8&2928b6d5&0&0001#{4d1e55b2-f16f-11cf-88cb-001111000030}";

        private const int WirelessProductId = 0xC539;
        private const string VirtualWireless = @"\\?\hid#vid_046d&pid_c539&mi_02&col01#8&20b6abbf&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}";
        private const string PhysicalWireless = @"\\?\hid#vid_046d&pid_c539&mi_02&col02#8&20b6abbf&0&0001#{4d1e55b2-f16f-11cf-88cb-001111000030}";

        private readonly Action<string> _log;
        private readonly Dictionary<string, bool> _connected;
        private readonly List<ReportProcessor> _processors;

        private string Virtual { get; set; }
        private string Physical { get; set; }

        private HidDevice _virtualDevice;
        private HidDevice _physicalDevice;
        private HidStream _virtualStream;
        private HidStream _physicalStream;

        public LightspeedConnector(Action<string> log, List<ReportProcessor> processors)
        {
            _log = log;
            _connected = new Dictionary<string, bool>();
            _processors = processors;
        }

        private void SetupDevices()
        {
            var virtualWirelessDevice = DeviceList.Local
                .GetHidDevices(VendorId, WirelessProductId)
                .FirstOrDefault(x => x.DevicePath == VirtualWireless);

            var virtualWiredDevice = DeviceList.Local
                .GetHidDevices(VendorId, WiredProductId)
                .FirstOrDefault(x => x.DevicePath == VirtualWired);

            if (SetupDevice(virtualWirelessDevice, out _virtualStream))
            {
                _virtualDevice = virtualWirelessDevice;
                Virtual = VirtualWireless;
            }
            else if (SetupDevice(virtualWiredDevice, out _virtualStream))
            {
                _virtualDevice = virtualWiredDevice;
                Virtual = VirtualWired;
            }
            else
            {
                _log("Failed connecting to virtual device.");
                Virtual = null;
            }

            var physicalWirelessDevice = DeviceList.Local
                .GetHidDevices(VendorId, WirelessProductId)
                .FirstOrDefault(x => x.DevicePath == PhysicalWireless);

            var physicalWiredDevice = DeviceList.Local
                .GetHidDevices(VendorId, WiredProductId)
                .FirstOrDefault(x => x.DevicePath == PhysicalWired);

            if (SetupDevice(physicalWirelessDevice, out _physicalStream))
            {
                _physicalDevice = physicalWirelessDevice;
                Physical = PhysicalWireless;
            }
            else if (SetupDevice(physicalWiredDevice, out _physicalStream))
            {
                _physicalDevice = physicalWiredDevice;
                Physical = PhysicalWired;
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

            stream.Closed += (sender, args) =>
            {
                _log($"Stream closed for: {device.DevicePath}");
                _connected[device.DevicePath] = false;
            };

            stream.InterruptRequested += (sender, eventArgs) =>
            {
                _log($"Stream interrupt requested for: {device.DevicePath}");
                _connected[device.DevicePath] = false;
            };

            receiver.Started += (sender, eventArgs) =>
            {
                _log($"Receiver started for: {device.DevicePath}");
                _connected[device.DevicePath] = true;
            };

            receiver.Stopped += (sender, eventArgs) =>
            {
                _log($"Receiver stopped for: {device.DevicePath}");
                _connected[device.DevicePath] = false;
            };

            receiver.Received += (sender, eventArgs) =>
            {
                var length = device.GetMaxInputReportLength();

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
            };

            receiver.Start(stream);
            return true;
        }

        private void IssueReport(HidStream stream, HidDevice device, ReportSize reportSize, ReportType reportType, params byte[] parameters)
        {
            var canIssueReport =
                device != null &&
                stream != null &&
                _connected.ContainsKey(device.DevicePath) &&
                _connected[device.DevicePath];

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
            var devicesAreConnected =
                !string.IsNullOrEmpty(Virtual) &&
                !string.IsNullOrEmpty(Physical) &&
                _connected.ContainsKey(Virtual) &&
                _connected.ContainsKey(Physical) &&
                _connected[Virtual] &&
                _connected[Physical];

            if (devicesAreConnected)
            {
                return;
            }

            SetupDevices();
        }

        public void Initialize()
        {
            // poll connections once per second starting immediately
            var connectionTimer = new Timer(
                state => EnsureConnection(), 
                null, 
                0,
                ConnectionInterval
            );

            // poll battery status once per minute starting after 3 seconds
            var batteryTimer = new Timer(
                state => IssueReport(_virtualStream, _virtualDevice, ReportSize.Short, ReportType.Battery), 
                null, 
                3000,
                BatteryInterval
            );

            GC.KeepAlive(connectionTimer);
            GC.KeepAlive(batteryTimer);
        }
    }
}
