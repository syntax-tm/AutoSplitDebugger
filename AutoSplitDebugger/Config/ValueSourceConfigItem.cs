using System.Diagnostics;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{Key}: {Value}")]
public class ValueSourceConfigItem
{
    public object Key { get; set; }
    public string Value { get; set; }
}