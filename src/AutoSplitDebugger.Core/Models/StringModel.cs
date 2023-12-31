﻿using System.Diagnostics;
using AutoSplitDebugger.Core.Interfaces;

namespace AutoSplitDebugger.Core.Models;

[DebuggerDisplay("{Value}")]
public class StringModel : IDisplayObject
{
    public DisplayType DisplayType => DisplayType.String;
    public string Value { get; }

    public StringModel(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
