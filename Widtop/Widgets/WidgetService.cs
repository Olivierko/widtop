using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Widtop.Widgets
{
    public class WidgetService : IWidgetService
    {
        private readonly Stopwatch _watch;
        private readonly List<Widget> _widgets;
        private readonly Dictionary<Type, object> _objects;

        public WidgetService()
        {
            _watch = new Stopwatch();
            _widgets = new List<Widget>();
            _objects = new Dictionary<Type, object>();
        }

        public T Get<T>() where T : new()
        {
            var type = typeof(T);

            if (!_objects.TryGetValue(type, out var item) || !(item is T))
            {
                _objects[type] = new T();
            }

            return (T)_objects[type];
        }

        public async Task Initialize()
        {
            _widgets.AddRange(new Widget[]
            {
                new WallpaperWidget(), 
                new DateTimeWidget(), 
                new MouseWidget(), 
                new CPUWidget(), 
                new GPUWidget(),
                new YeelightWidget(),
                new DeskWidget()
            });

            foreach (var widget in _widgets)
            {
                widget.Setup(this);
                await widget.Initialize();
            }
        }

        public async Task Update()
        {
            var elapsed = _watch.Elapsed;
            _watch.Restart();

            foreach (var widget in _widgets)
            {
                await widget.Update(elapsed);
            }
        }
        
        public async Task Render(Graphics graphics)
        {
            foreach (var widget in _widgets)
            {
                await widget.Render(graphics);
            }
        }

        public async Task OnShutdown()
        {
            foreach (var widget in _widgets)
            {
                await widget.OnShutdown();
            }
        }
    }
}
