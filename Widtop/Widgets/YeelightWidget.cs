using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Widtop.Utility;
using YeelightAPI;
using YeelightAPI.Events;

namespace Widtop.Widgets
{
    public class YeelightWidget : Widget
    {
        private const int TimerInterval = 60000;
        private const int LightSmooth = 10000;

        private static Rectangle Area => new Rectangle(2560, 0, 1080, 200);
        private static Font PrimaryFont => new Font("Agency FB", 12);
        private static SolidBrush PrimaryBrush => new SolidBrush(Color.White);
        private static StringFormat PrimaryFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private readonly ThreadSafeStringBuilder _stringBuilder;
        private readonly List<Device> _connectedDevices;

        private QueuedTimer _lightsTimer;

        public YeelightWidget()
        {
            _stringBuilder = new ThreadSafeStringBuilder();
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
                case Keys.F24:
                    foreach (var device in _connectedDevices)
                    {
                        ToggleDevicePower(device);
                    }
                    break;
            }
        }

        private static async Task EnsureDeviceConnection(Device device)
        {
            if (device.IsConnected)
            {
                return;
            }

            try
            {
                var connected = await device.Connect();

                Debug.WriteLine($"Attempted to connect to device: {device.Name}:{connected}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to connect to device: {e.Message}");
            }
        }

        private static void ToggleDevicePower(Device device)
        {
            try
            {
                EnsureDeviceConnection(device).GetAwaiter().GetResult();

                var toggle = device.Toggle();
                var power = toggle.GetAwaiter().GetResult();

                Debug.WriteLine($"Power toggled for device: {device.Name}:{power}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to toggle device power: {e.Message}");
            }
        }

        private async Task OnLightsTimer()
        {
            var sunrise = await Service.Get<Solar>().GetSunrise();
            var sunset = await Service.Get<Solar>().GetSunset();

            if (!sunrise.HasValue || !sunset.HasValue)
            {
                return;
            }

            if (DateTime.UtcNow.TimeOfDay < sunset && DateTime.UtcNow.TimeOfDay > sunrise)
            {
                return;
            }

            foreach (var device in _connectedDevices)
            {
                try
                {
                    await EnsureDeviceConnection(device);
                    await device.SetPower(true, LightSmooth);
                }
                catch
                {
                    // ignored
                }
            }

            // only run timer action once
            _lightsTimer.Stop();
        }

        public override async Task Initialize()
        {
            DeviceLocator.OnDeviceFound += OnDeviceFound;
            await DeviceLocator.Discover();

            var keyboard = Service.Get<Keyboard>();
            keyboard.KeyPress += OnKeyPress;

            _lightsTimer = new QueuedTimer(async x => await OnLightsTimer(), TimerInterval);
        }

        public override async Task Update()
        {
            _stringBuilder.Clear();

            foreach (var device in _connectedDevices)
            {
                await EnsureDeviceConnection(device);

                _stringBuilder.AppendLine($"{device.Model} - {device.Properties["power"]}");
            }
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
                    await EnsureDeviceConnection(device);
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
