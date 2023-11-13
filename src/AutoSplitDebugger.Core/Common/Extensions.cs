using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AutoSplitDebugger.Core;

public static class Extensions
{
    public static bool EqualsIgnoreCase(this string a, string b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) return false;

        return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool ContainsIgnoreCase(this string a, string b)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        if (a == string.Empty) return b == string.Empty;

        return a.Contains(b, StringComparison.CurrentCultureIgnoreCase);
    }

    public static string GetDescription(this Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        if (fi is null)
        {
            return string.Empty;
        }

        var descAttributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (descAttributes.Length > 0)
        {
            return descAttributes[0].Description;
        }

        var displayAttributes = (DisplayAttribute[]) fi.GetCustomAttributes(typeof(DisplayAttribute), false);
        if (displayAttributes.Length > 0)
        {
            var displayAttr = displayAttributes[0];

            if (!string.IsNullOrWhiteSpace(displayAttr.Description)) return displayAttr.Description;
            if (!string.IsNullOrWhiteSpace(displayAttr.Name)) return displayAttr.Name;
            if (!string.IsNullOrWhiteSpace(displayAttr.ShortName)) return displayAttr.ShortName;

            return string.Empty;
        }

        return value.ToString();
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if(source == null) return;

        foreach (var t in source)
        {
            action(t);
        }
    }
}
