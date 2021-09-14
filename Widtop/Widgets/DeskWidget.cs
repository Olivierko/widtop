using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Widtop.Desk;
using Widtop.Utility;

namespace Widtop.Widgets
{
    public class DeskWidget : Widget
    {
        private const int UpdateInterval = 1000;

        private double _lastUpdate;
        private Keyboard _keyboard;
        private readonly Communicator _communicator;

        public DeskWidget()
        {
            _communicator = new Communicator();
            _communicator.OnMessageReceived += OnMessageReceived;
        }

        private static void OnMessageReceived(MessageType type, byte value)
        {
            switch (type)
            {
                case MessageType.STATUS_RESPONSE:
                    var state = (DeskState)value;
                    Debug.WriteLine($"Desk state is: {state}");
                    break;
                case MessageType.DEBUG_RESPONSE:
                    //Debug.WriteLine($"############  DESK DEBUG MSG: {value}");
                    break;
                default:
                    Debug.WriteLine($"Message received: {type}, value: {value}");
                    break;
            }
        }

        private void OnKeyPress(Keys key)
        {
            if (!_keyboard.IsDown(Keys.LShiftKey) || !_keyboard.IsDown(Keys.RShiftKey))
            {
                return;
            }

            switch (key)
            {
                case Keys.Up:
                    _communicator.Write(MessageType.UP_REQUEST);
                    break;
                case Keys.Down:
                    _communicator.Write(MessageType.DOWN_REQUEST);
                    break;
                case Keys.End:
                    _communicator.Write(MessageType.STOP_REQUEST);
                    break;
            }
        }

        public override Task Initialize()
        {
            _communicator.Start();

            _keyboard = Service.Get<Keyboard>();
            _keyboard.KeyPress += OnKeyPress;

            return base.Initialize();
        }
        
        public override Task Update(TimeSpan elapsed)
        {
            _lastUpdate += elapsed.TotalMilliseconds;

            if (_lastUpdate >= UpdateInterval)
            {
                _lastUpdate = 0;
                _communicator.Write(MessageType.STATUS_REQUEST);
            }

            return base.Update(elapsed);
        }

        public override Task Render(Graphics graphics)
        {
            // TODO: render label with current state
            return base.Render(graphics);
        }

        public override Task OnShutdown()
        {
            _communicator.Stop();
            return base.OnShutdown();
        }
    }
}
