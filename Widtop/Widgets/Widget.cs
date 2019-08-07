using System.Drawing;

namespace Widtop.Widgets
{
    public abstract class Widget
    {
        public virtual void Initialize(IWidgetService service) { }

        public virtual void Update() { }

        public virtual void Render(Graphics graphics) { }
    }
}