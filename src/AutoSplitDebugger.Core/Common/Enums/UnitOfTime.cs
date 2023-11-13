using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace AutoSplitDebugger.Core;

[DefaultValue(Unknown)]
[EnumExtensions]
public enum UnitOfTime
{
    Unknown = 0,
    Ticks,
    Milliseconds,
    Seconds,
    Minutes,
    Hours,
    Days
}
