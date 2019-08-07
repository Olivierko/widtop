namespace Widtop.Widgets
{
    public interface IWidgetService
    {
        T Get<T>() where T : new();
    }
}