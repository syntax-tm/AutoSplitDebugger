using AutoSplitDebugger.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AutoSplitDebugger.Models;

[DebuggerDisplay("{ToString()}")]
public class TimeSpanModel : IDisplayObject
{
    public DisplayType DisplayType => DisplayType.Timer;

    public List<TimeSpanSegmentModel> Segments { get; }

    public TimeSpanModel(TimeSpan value)
    {
        Segments = new ();

        // seconds only
        if (value.TotalMinutes < 1)
        {
            Segments.Add(TimeSpanSegmentModel.FromSecondsWithMs(value.Seconds, value.Milliseconds));

            return;
        }

        // minutes + seconds
        if (value.TotalHours < 1)
        {
            Segments.Add(TimeSpanSegmentModel.FromMinutes(value.Minutes));
            Segments.Add(TimeSpanSegmentModel.FromSecondsWithMs(value.Seconds, value.Milliseconds));

            return;
        }
        
        // hours + minutes + seconds (no ms)
        Segments.Add(TimeSpanSegmentModel.FromHours(value.Hours));
        Segments.Add(TimeSpanSegmentModel.FromMinutes(value.Minutes));
        Segments.Add(TimeSpanSegmentModel.FromSeconds(value.Seconds));
    }

    public override string ToString()
    {
        return string.Join(' ', Segments);
    }
}
