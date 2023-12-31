﻿using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace AutoSplitDebugger.Core;

[EnumExtensions]
public enum TimeSegment
{
    [Description("s")]
    SecondsAndMs,
    [Description("s")]
    Seconds,
    [Description("m")]
    Minutes,
    [Description("h")]
    Hours,
    [Description("d")]
    Days
}
