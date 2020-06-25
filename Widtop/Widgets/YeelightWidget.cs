using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Widtop.Utility;
using YeelightAPI;
using YeelightAPI.Events;

namespace Widtop.Widgets
{
    public class YeelightWidget : Widget
    {
        private static Rectangle Area => new Rectangle(2560, 0, 1080, 200);
        private static Font PrimaryFont => new Font("Agency FB", 12);
        private static SolidBrush PrimaryBrush => new SolidBrush(Color.White);
        private static StringFormat PrimaryFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private readonly StringBuilder _stringBuilder;
        private readonly List<Device> _connectedDevices;

        public YeelightWidget()
        {
            _stringBuilder = new StringBuilder();
            _connectedDevices = new List<Device>();
        }

        private void OnDeviceFound(object sender, DeviceFoundEventArgs e)
        {
            var status = e.Device.Connect();

            if (!status.Result)
            {
                Debug.WriteLine("Failed to connect to Yeelight Device.");
                return;
            }

            _connectedDevices.Add(e.Device);
        }

        private void OnKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.F12:
                    foreach (var device in _connectedDevices)
                    {
                        ToggleDevicePower(device);
                    }
                    break;
            }
        }

        private static void ToggleDevicePower(IDeviceController device)
        {
            try
            {
                var task = device.Toggle();
                var power = task.GetAwaiter().GetResult();

                Debug.WriteLine($"Power toggled: {power}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to toggle device power: {e.Message}");

            }
        }

        public override async Task Initialize()
        {
            DeviceLocator.OnDeviceFound += OnDeviceFound;
            await DeviceLocator.Discover();

            var keyboard = Service.Get<Keyboard>();
            keyboard.KeyPress += OnKeyPress;
        }

        public override async Task Update()
        {
            await Task.Run(() =>
            {
                _stringBuilder.Clear();

                foreach (var device in _connectedDevices)
                {
                    _stringBuilder.AppendLine($"{device.Model} - {device.Properties["power"]}");
                }
            });
        }

        public override async Task Render(Graphics graphics)
        {
            await Task.Run(() =>
            {
                graphics.DrawString(
                    _stringBuilder.ToString(),
                    PrimaryFont,
                    PrimaryBrush,
                    Area,
                    PrimaryFormat
                );
            });
        }

        public override async Task OnShutdown()
        {
            foreach (var device in _connectedDevices)
            {
                try
                {
                    await device.TurnOff();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
