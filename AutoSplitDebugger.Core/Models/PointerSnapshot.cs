using AutoSplitDebugger.Core.Interfaces;

namespace AutoSplitDebugger.Core.Models;

public class PointerSnapshot<T> : IPointerSnapshot where T : struct
{
    public string Text { get; set; }
    public object Value { get; set; }

    public PointerSnapshot(T? value, string text)
    {
        Value = value;
        Text = text;
    }
}
