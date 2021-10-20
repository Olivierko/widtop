// ReSharper disable InconsistentNaming
namespace Widtop.Desk
{
    public enum MessageType : byte
    {
        NONE = 0x00,
        STATUS_REQUEST = 0x21,
        STATUS_RESPONSE = 0x22,
        DEBUG_REQUEST = 0x23,
        DEBUG_RESPONSE = 0x24,
        STOP_REQUEST = 0x25,
        DOWN_REQUEST = 0x27,
        UP_REQUEST = 0x29
    }
}