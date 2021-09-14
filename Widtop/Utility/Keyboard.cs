using System;

namespace Widtop.Utility
{
    public class Keyboard
    {
        public delegate void KeyboardKeyPressCallback(Keys key);
        public event KeyboardKeyPressCallback KeyPress;

        public Keyboard()
        {
            var timer = new QueuedTimer(x => Callback(), 5);
            GC.KeepAlive(timer);
        }

        private static KeyState Resolve(short value)
        {
            var isToggled = (value & (1 << 0)) != 0;
            var isPressed = (value & (1 << 7)) != 0;

            var state = KeyState.None;

            if (isToggled)
            {
                state |= KeyState.Toggled;
            }

            if (isPressed)
            {
                state |= KeyState.Down;
            }

            return state;
        }

        private void Callback()
        {
            for (var i = 0; i < 255; i++)
            {
                var key = Native.GetAsyncKeyState(i);

                var state = Resolve(key);

                if (state.HasFlag(KeyState.Down) || state.HasFlag(KeyState.Toggled))
                {
                    KeyPress?.Invoke((Keys)i);
                    break;
                }
            }
        }

        public bool IsDown(Keys key)
        {
            var value = Native.GetKeyState((int)key);
            var state = Resolve(value);

            return state.HasFlag(KeyState.Down);
        }
    }
}
