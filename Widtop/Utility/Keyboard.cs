using System;

namespace Widtop.Utility
{
    public class Keyboard
    {
        private const short UP = 1;
        private const short DOWN = -32767;

        public delegate void KeyboardKeyPressCallback(Keys key);
        public event KeyboardKeyPressCallback KeyPress;

        public Keyboard()
        {
            var timer = new QueuedTimer(x => Callback(), 5);
            GC.KeepAlive(timer);
        }

        private void Callback()
        {
            for (var i = 0; i < 255; i++)
            {
                int key = Native.GetAsyncKeyState(i);

                if (key == UP || key == DOWN)
                {
                    KeyPress?.Invoke((Keys)i);
                    break;
                }
            }
        }
    }
}
