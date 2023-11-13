using System.Diagnostics;

namespace AutoSplitDebugger.Core.AutoSplit.Schema;

[DebuggerDisplay("{Module} | {Version}")]
public class StateDescriptor
{
    public string Module { get; set; }
    public string Version { get; set; }
}
