using System.Diagnostics;

namespace AutoSplitDebugger.Core.AutoSplit.Schema;

[DebuggerDisplay("{Type} {Name}")]
public class AutoSplitterPointer
{

    public AutoSplitterPointerType Type { get; set; }
    public string Name { get; set; }
    public string BaseModule { get; set; }
    public int[] Offsets { get; set; }
}
