﻿// ReSharper disable NotAccessedField.Local

using Widtop.Hid;
using Widtop.Utility;

namespace Widtop.Logitech
{
    public abstract class LightspeedDevice : Device
    {
        private const byte DeviceId = 0x01;
        private const string VirtualHardwareIdPattern = "mi_02&col01";
        private const string PhysicalHardwareIdPattern = "mi_02&col02";

        public override int VendorId => 0x046D;
        public override int ReceiverId => 0xC539;

        public abstract Discharge[] DischargeCurve { get; }

        public decimal? Battery { get; private set; }
        public BatteryVoltageStatus? Status { get; private set; }

        private QueuedTimer _batteryTimer;

        private void OnBatteryUpdated(double volt, BatteryVoltageStatus status, double discharge, decimal percentage)
        {
            Status = status;
            Battery = percentage;
        }

        public override void OnInitialize(Connector connector)
        {
            var batteryProcessor = new BatteryReportProcessor(this);
            batteryProcessor.BatteryUpdated += OnBatteryUpdated;

            var errorProcessor = new ErrorReportProcessor();
            errorProcessor.ErrorReceived += connector.Reset;

            Processors = new ReportProcessor[]
            {
                batteryProcessor,
                errorProcessor
            };

            // poll battery status once per minute starting after 3 seconds
            _batteryTimer = new QueuedTimer(
                state => connector.IssueReport((byte)ReportSize.Short, DeviceId, (byte)ReportType.Battery),
                3000,
                60000
            );
        }

        public override void OnConnected()
        {
            _batteryTimer?.Trigger();
        }

        public override bool MatchesVirtual(string devicePath)
        {
            return devicePath.Contains(VirtualHardwareIdPattern);
        }

        public override bool MatchesPhysical(string devicePath)
        {
            return devicePath.Contains(PhysicalHardwareIdPattern);
        }
    }
}