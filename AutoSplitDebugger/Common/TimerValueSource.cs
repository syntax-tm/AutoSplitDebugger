using System;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.Models;

namespace AutoSplitDebugger;

public class TimerValueSource<T> : IValueSource where T : struct
{
    public UnitOfTime UnitOfTime { get; set; }

    public TimerValueSource(UnitOfTime unit)
    {
        UnitOfTime = unit;
    }

    public bool Contains(object obj)
    {
        return true;
    }

    public object GetDisplay(object obj)
    {
        if (obj is not T tValue) throw new ArgumentException($"{nameof(obj)} must be of type {typeof(T).Name}.");

        var value = Convert.ToDouble(tValue);
        var ts = TimeSpan.FromSeconds(value);
        var timerDisplay = new TimeSpanModel(ts);

        return timerDisplay;
    }
}
