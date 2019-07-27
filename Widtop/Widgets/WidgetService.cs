using System.Drawing;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    public class WidgetService
    {
        private Bitmap _renderTarget;
        private Widget[] _widgets;

        public void Initialize(Bitmap renderTarget)
        {
            var pc = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };

            _renderTarget = renderTarget;
            _widgets = new Widget[]
            {
                new WallpaperWidget(),
                new ClockWidget(),
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
            lock (_renderTarget)
            {
                using (var graphics = Graphics.FromImage(_renderTarget))
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
