namespace AutoSplitDebugger.Interfaces;

public interface IPointerViewModel
{
    string Name { get; set; }
    string Format { get; set; }
    string DisplayText { get; }
    string ValueText { get; }

    int[] PointerPath { get; }
    bool IsValid { get; }
    nint? Address { get; }

    bool IsChanged { get; }
    bool IsWarn { get; }

    bool HasValueSource { get; }
    IValueSource ValueSource { get; set; }

    void Refresh();
}
