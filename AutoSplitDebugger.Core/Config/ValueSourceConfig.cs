using System;
using System.Collections.Generic;
using System.Globalization;
using AutoSplitDebugger.Core.Interfaces;
using AutoSplitDebugger.Core.Models;
using Newtonsoft.Json;

namespace AutoSplitDebugger.Core.Config;

[JsonObject(IsReference = true)]
public class ValueSourceConfig
{
    public Dictionary<string, string> Map { get; set; }

    public IValueSource ToValueSource<T>() where T : struct, IParsable<T>
    {
        var map = new Dictionary<T, string>();

        foreach (var item in Map)
        {
            var value = T.Parse(item.Key, CultureInfo.CurrentCulture);
            map.Add(value, item.Value);
        }
        
        var source = new ValueSource<T>(map);
        return source;
    }
}