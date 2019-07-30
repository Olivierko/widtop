using System.Drawing;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class WidgetService
    {
        private Bitmap _buffer;
        private Widget[] _widgets;

        public void Initialize(Bitmap buffer)
        {
            var pc = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };

            _buffer = buffer;
            _widgets = new Widget[]
            {
                new WallpaperWidget(),
                //new ClockWidget(),
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
            lock (_buffer)
            {
                using (var graphics = Graphics.FromImage(_buffer))
                {
                    for (var index = 0; index < _widgets.Length; index++)
                    {
                        _widgets[index].Render(graphics);
                    }
                }
            }
        }
    }
}
