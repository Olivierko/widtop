using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Widtop.Utility;
using YeelightAPI;
using YeelightAPI.Events;
using YeelightAPI.Models;

namespace Widtop.Widgets
{
    public class YeelightWidget : Widget
    {
        private const int TimerInterval = 60000;
        private const int LightSmooth = 10000;

        private static Rectangle Area => new Rectangle(WidgetConstants.X, 0, 1080, 200);
        private static Font PrimaryFont => new Font("Agency FB", 12);
        private static SolidBrush PrimaryBrush => new SolidBrush(Color.White);
        private static StringFormat PrimaryFormat => new StringFormat { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Far };

        private readonly ThreadSafeStringBuilder _stringBuilder;
        private readonly List<Device> _connectedDevices;

        private bool _pendingRequest;
        private LightState _lightState;
        private QueuedTimer _sunlightTimer;
        private QueuedTimer _connectionTimer;

        private enum LightState
        {
            VeryHigh = 1,
            High = 2,
            MediumHigh = 3,
            Medium = 4,
            MediumLow = 5,
            Low = 6,
            VeryLow = 7,
            NightHigh = 8,
            NightLow = 9,
            Off = 10
        }

        public YeelightWidget()
        {
            _stringBuilder = new ThreadSafeStringBuilder();
            _connectedDevices = new List<Device>();
            _lightState = LightState.VeryHigh;
        }

        private void OnDeviceFound(object sender, DeviceFoundEventArgs e)
        {
            if (_connectedDevices.Exists(x => x.Id == e.Device.Id))
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    var connected = await e.Device.Connect();

                    if (!connected)
                    {
                        Debug.WriteLine("Failed to connect to device.");
                        return;
                    }

                    _connectedDevices.Add(e.Device);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Failed to connect to device: {exception.Message}");
                }
                finally
                {
                    await SetLightState(_connectedDevices, _lightState);
                }
            });
        }

        private void OnKeyPress(Keys key)
        {
            Task.Run(async () =>
            {
                switch (key)
                {
                    case Keys.F13:
                        await SetLightState(_connectedDevices, LightState.VeryHigh);
                        break;
                    case Keys.F14:
                        await SetLightState(_connectedDevices, LightState.High);
                        break;
                    case Keys.F15:
                        await SetLightState(_connectedDevices, LightState.MediumHigh);
                        break;
                    case Keys.F16:
                        await SetLightState(_connectedDevices, LightState.Medium);
                        break;
                    case Keys.F17:
                        await SetLightState(_connectedDevices, LightState.MediumLow);
                        break;
                    case Keys.F18:
                        await SetLightState(_connectedDevices, LightState.Low);
                        break;
                    case Keys.F19:
                        await SetLightState(_connectedDevices, LightState.VeryLow);
                        break;
                    case Keys.F20:
                        await SetLightState(_connectedDevices, LightState.NightHigh);
                        break;
                    case Keys.F21:
                        await SetLightState(_connectedDevices, LightState.NightLow);
                        break;
                    case Keys.F22:
                        await SetLightState(_connectedDevices, LightState.Off);
                        break;
                    case Keys.F23:
                        await DecreaseLightState();
                        break;
                    case Keys.F24:
                        await IncreaseLightState();
                        break;
                }
            });
        }

        private async Task IncreaseLightState(int? smooth = null)
        {
            if (Enum.IsDefined(typeof(LightState), (int)_lightState + 1))
            {
                var lightState = (LightState)((int)_lightState + 1);
                await SetLightState(_connectedDevices, lightState, smooth);
            }
        }

        private async Task DecreaseLightState(int? smooth = null)
        {
            if (Enum.IsDefined(typeof(LightState), (int)_lightState - 1))
            {
                var lightState = (LightState)((int)_lightState - 1);
                await SetLightState(_connectedDevices, lightState, smooth);
            }
        }

        private async Task SetLightState(List<Device> devices, LightState state, int? smooth = null)
        {
            _pendingRequest = true;

            foreach (var device in devices)
            {
                Debug.WriteLine($"Setting device: {device.Name} to state: {state}");

                switch (state)
                {
                    case LightState.VeryHigh:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 6500, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 100, smooth);
                        break;
                    case LightState.High:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 5000, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 100, smooth);
                        break;
                    case LightState.MediumHigh:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 4000, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 100, smooth);
                        break;
                    case LightState.Medium:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 3000, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 100, smooth);
                        break;
                    case LightState.MediumLow:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 2700, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 100, smooth);
                        break;
                    case LightState.Low:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 2700, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 50, smooth);
                        break;
                    case LightState.VeryLow:
                        await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                        await SetDeviceColorTemperature(device, 2700, smooth);
                        await SetDeviceBackgroundPower(device, false);
                        await SetDeviceBrightness(device, 20, smooth);
                        break;
                    case LightState.NightHigh:
                        // TODO: support for arwen ceiling light
                        if (device.Model == MODEL.Unknown)
                        {
                            await SetDevicePower(device, true, smooth, PowerOnMode.Night);
                            await SetDeviceBrightness(device, 100, smooth);
                        }
                        else
                        {
                            await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                            await SetDeviceColorTemperature(device, 2700, smooth);
                            await SetDeviceBrightness(device, 10, smooth);
                        }
                        await SetDeviceBackgroundPower(device, false);
                        break;
                    case LightState.NightLow:
                        // TODO: support for arwen ceiling light
                        if (device.Model == MODEL.Unknown)
                        {
                            await SetDevicePower(device, true, smooth, PowerOnMode.Night);
                            await SetDeviceBrightness(device, 20, smooth);
                        }
                        else
                        {
                            await SetDevicePower(device, true, smooth, PowerOnMode.Ct);
                            await SetDeviceColorTemperature(device, 2700, smooth);
                            await SetDeviceBrightness(device, 1, smooth);
                        }
                        await SetDeviceBackgroundPower(device, false);
                        break;
                    case LightState.Off:
                        await SetDevicePower(device, false, smooth, PowerOnMode.Ct);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }

                Debug.WriteLine($"Finished setting device: {device.Name} to state: {state}");
            }

            _lightState = state;
            _pendingRequest = false;
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

        private static async Task SetDeviceBrightness(Device device, int value, int? smooth = null)
        {
            try
            {
                await EnsureDeviceConnection(device);
                var operation = await device.SetBrightness(value, smooth);
                
                Debug.WriteLine(operation
                    ? $"Brightness set to {value} for device: {device.Name}"
                    : "Failed to set device brightness"
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to set device brightness: {e.Message}");
            }
        }

        private static async Task SetDeviceColorTemperature(Device device, int value, int? smooth = null)
        {
            try
            {
                await EnsureDeviceConnection(device);
                var operation = await device.SetColorTemperature(value, smooth);

                Debug.WriteLine(operation
                    ? $"Color temperature set to {value} for device: {device.Name}"
                    : "Failed to set Color temperature"
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to set Color temperature: {e.Message}");
            }
        }

        private static async Task SetDevicePower(Device device, bool state, int? smooth, PowerOnMode mode)
        {
            try
            {
                await EnsureDeviceConnection(device);
                var operation = await device.SetPower(state, smooth, mode);

                Debug.WriteLine(operation
                    ? $"Power set to {state} for device: {device.Name}, mode: {mode}"
                    : "Failed to set device power"
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to set device power: {e.Message}");
            }
        }

        private static async Task SetDeviceBackgroundPower(Device device, bool value)
        {
            try
            {
                await EnsureDeviceConnection(device);
                var operation = await device.BackgroundSetPower(value);

                Debug.WriteLine(operation
                    ? $"Background power set to {value} for device: {device.Name}"
                    : "Failed to set background power"
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to set background power: {e.Message}");
            }
        }

        private async Task OnSunlightTimer()
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

            if (_lightState == LightState.Off)
            {
                return;
            }

            await SetLightState(_connectedDevices, LightState.NightLow, LightSmooth);

            // only run timer action once
            _sunlightTimer.Stop();
        }

        public override async Task Initialize()
        {
            DeviceLocator.OnDeviceFound += OnDeviceFound;
            await DeviceLocator.Discover();

            var keyboard = Service.Get<Keyboard>();
            keyboard.KeyPress += OnKeyPress;

            _sunlightTimer = new QueuedTimer(async x => await OnSunlightTimer(), TimerInterval);
            _connectionTimer = new QueuedTimer(async x => await DeviceLocator.Discover(), TimerInterval, TimerInterval);
        }

        public override async Task Update()
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine(_pendingRequest 
                ? "State: pending request ..." 
                : $"State: {_lightState}"
            );

            foreach (var device in _connectedDevices)
            {
                await EnsureDeviceConnection(device);
                _stringBuilder.AppendLine($"{device.Name} - {device.Properties["power"]}");
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
