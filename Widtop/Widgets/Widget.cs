using System.Drawing;
using System.Threading.Tasks;

namespace Widtop.Widgets
{
    public abstract class Widget
    {
        protected IWidgetService Service;

        internal void Setup(IWidgetService service)
        {
            Service = service;
        }

        public virtual async Task Initialize()
        {
            await Task.CompletedTask;
        }

        public virtual async Task Update()
        {
            await Task.CompletedTask;
        }

        public virtual async Task Render(Graphics graphics)
        {
            await Task.CompletedTask;
        }

        public virtual async Task OnShutdown()
        {
            await Task.CompletedTask;
        }
    }
}