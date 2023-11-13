namespace AutoSplitDebugger.Core.Interfaces;

public interface IValueSource
{
    bool Contains(object value);
    object GetDisplay(object value);
}