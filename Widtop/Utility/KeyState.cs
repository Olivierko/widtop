using System;

namespace Widtop.Utility
{
    [Flags]
    public enum KeyState
    {
        None = 0,
        Down = 1,
        Toggled = 2,
    }
}