using System.Drawing;

namespace Widtop.Widgets
{
    public abstract class Widget
    {
        protected IWidgetService Service;

        internal void Setup(IWidgetService service)
        {
            Service = service;
        }

        public virtual void Initialize() { }

        public virtual void Update() { }

        public virtual void Render(Graphics graphics) { }
    }
}