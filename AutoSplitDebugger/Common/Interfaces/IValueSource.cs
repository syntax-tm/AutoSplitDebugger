namespace AutoSplitDebugger.Interfaces;

public interface IValueSource
{
    bool Contains(object value);
    string GetDisplayText(object value);
}