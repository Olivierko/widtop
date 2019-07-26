// ReSharper disable InconsistentNaming

namespace Widtop.Hid
{
    public enum BatteryVoltageStatus : byte
    {
        BATTERY_VOLTAGE_STATUS_DISCHARGING = 0x00,
        BATTERY_VOLTAGE_STATUS_WIRELESS_CHARGING = 0x10,
        BATTERY_VOLTAGE_STATUS_CHARGING = 0x80,
        BATTERY_VOLTAGE_STATUS_FULLY_CHARGED = 0x81
    }
}