using System.Diagnostics;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{Process} ({Pointers.Length} Pointer(s))")]
public class AutoSplitConfig
{
    public string Process { get; set; }
    public PointerConfig[] Pointers { get; set; }
    public ValueSourceConfig[] ValueSources { get; set; }
}
