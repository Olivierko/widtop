using System;

namespace Widtop.Widgets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WidgetAttribute : Attribute
    {
        public int Z { get; set; }
    }
}