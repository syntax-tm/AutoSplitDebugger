using System.Diagnostics;

namespace AutoSplitDebugger.Models;

[DebuggerDisplay("{ToString()}")]
public class TimeSpanSegmentModel
{
    public string DisplayValue { get; }
    public double Value { get; }
    public double Precision { get; }
    public TimeSegment Segment { get; }

    public string Label => Segment.GetDescription();

    public TimeSpanSegmentModel(double value, TimeSegment segment)
    {
        Value = value;
        Segment = segment;

        DisplayValue = $"{value,2}";
    }

    public TimeSpanSegmentModel(double value, double precision)
    {
        Value = value;
        Precision = precision;
        Segment = TimeSegment.SecondsAndMs;

        // seconds are displayed with three decimals for the milliseconds
        DisplayValue = $"{value}.{precision:000}";
    }

    public override string ToString()
    {
        return $"{DisplayValue} {Label}";
    }

    public static TimeSpanSegmentModel FromDays(double days)
    {
        return new (days, TimeSegment.Days);
    }

    public static TimeSpanSegmentModel FromHours(double h)
    {
        return new (h, TimeSegment.Hours);
    }

    public static TimeSpanSegmentModel FromMinutes(double m)
    {
        return new (m, TimeSegment.Minutes);
    }

    public static TimeSpanSegmentModel FromSeconds(double s)
    {
        return new (s, TimeSegment.Seconds);
    }

    public static TimeSpanSegmentModel FromSecondsWithMs(double s, double ms)
    {
        return new (s, ms);
    }
}