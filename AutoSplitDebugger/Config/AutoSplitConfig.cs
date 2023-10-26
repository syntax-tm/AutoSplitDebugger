using System.Diagnostics;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{Title} | {Process} (P: {Pointers.Length})")]
public class AutoSplitConfig
{
    public string Title { get; set; }
    public string Process { get; set; }
    public PointerConfig[] Pointers { get; set; }
    public ValueSourceConfig[] ValueSources { get; set; }
}
