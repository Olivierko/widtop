using System.Drawing;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class WidgetService
    {
        private Buffer[] _buffers;
        private Widget[] _widgets;

        public void Initialize(Buffer[] buffers)
        {
            var pc = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };

            _buffers = buffers;
            _widgets = new Widget[]
            {
                new WallpaperWidget(),
                new DateTimeWidget(),
                new MouseWidget(),
                new CPUWidget(pc), 
                new GPUWidget(pc)
            };

            for (var index = 0; index < _widgets.Length; index++)
            {
                _widgets[index].Initialize();
            }
        }

        public void Update()
        {
            for (var index = 0; index < _widgets.Length; index++)
            {
                _widgets[index].Update();
            }
        }

        public void Render()
        {
            lock (_buffers)
            {
                foreach (var buffer in _buffers)
                {
                    using (var graphics = Graphics.FromImage(buffer.RenderTarget))
                    {
                        for (var index = 0; index < _widgets.Length; index++)
                        {
                            _widgets[index].Render(buffer, graphics);
                        }
                    }
                }
            }
        }
    }
}
