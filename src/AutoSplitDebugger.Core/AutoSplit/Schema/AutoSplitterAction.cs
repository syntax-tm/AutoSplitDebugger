using System.Diagnostics;

namespace AutoSplitDebugger.Core.AutoSplit.Schema;

[DebuggerDisplay("{Name}")]
public class AutoSplitterAction
{
    public string Name { get; set; }
    public StateDescriptor StateDescriptor { get; set; }
    public string Body { get; set; }
}
