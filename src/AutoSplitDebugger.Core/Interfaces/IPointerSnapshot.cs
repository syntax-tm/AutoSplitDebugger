namespace AutoSplitDebugger.Core.Interfaces;

public interface IPointerSnapshot
{
    string Text { get; set; }
    object Value { get; set; }
}
