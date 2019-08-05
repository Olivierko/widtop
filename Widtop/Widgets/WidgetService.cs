using System.Drawing;
using OpenHardwareMonitor.Hardware;

namespace Widtop.Widgets
{
    // TODO: enforce empty ctor on widgets and load them by scanning assembly/a given folder
    public class WidgetService
    {
        private Widget[] _widgets;

        public void Initialize()
        {
            var pc = new Computer
            {
                CPUEnabled = true,
                GPUEnabled = true
            };

            _widgets = new Widget[]
            {
                new WallpaperWidget(),
                new DateTimeWidget(),
                new MouseWidget(),
                new CPUWidget(pc), 
                new GPUWidget(pc)
            };

            foreach (var widget in _widgets)
            {
                widget.Initialize();
            }
        }

        public void Update()
        {
            foreach (var widget in _widgets)
            {
                widget.Update();
            }
        }
        
        public void Render(Graphics graphics)
        {
            foreach (var widget in _widgets)
            {
                widget.Render(graphics);
            }
        }
    }
}
