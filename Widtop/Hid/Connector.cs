﻿// ReSharper disable NotAccessedField.Local

using System;
using System.Collections.Generic;
using System.Linq;
using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using Widtop.Utility;

namespace Widtop.Hid
{
    public class Connector
    {
        private readonly Device _device;
        private readonly Dictionary<string, bool> _connections;

        private HidDevice _hidDevice;
        private HidStream _hidStream;
        private QueuedTimer _connectionTimer;

        private string Virtual { get; set; }
        private string Physical { get; set; }

        public Connector(Device device)
        {
            _device = device;
            _connections = new Dictionary<string, bool>();
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

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
                Log($"Received report with id: {report.ReportID}, type: {report.ReportType}");

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
            var virtualWirelessDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            var virtualWiredDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesVirtual(x.DevicePath));

            if (SetupDevice(virtualWirelessDevice, out _hidStream))
            {
                _hidDevice = virtualWirelessDevice;
                Virtual = virtualWirelessDevice?.DevicePath;
            }
            else if (SetupDevice(virtualWiredDevice, out _hidStream))
            {
                _hidDevice = virtualWiredDevice;
                Virtual = virtualWiredDevice?.DevicePath;
            }
            else
            {
                Log("Failed connecting to virtual device.");
                Virtual = null;
            }

            var physicalWirelessDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ReceiverId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

            var physicalWiredDevice = DeviceList.Local
                .GetHidDevices(_device.VendorId, _device.ProductId)
                .FirstOrDefault(x => _device.MatchesPhysical(x.DevicePath));

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
            _connectionTimer = new QueuedTimer(
                state => EnsureConnection(), 
                1000
            );

            _device.OnInitialize(this);
        }

        public void IssueReport(params byte[] parameters)
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
                parameters.CopyTo(buffer, 0);

                Log("Issued report:");
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
