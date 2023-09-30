using System;
using System.Collections.Generic;
using AutoSplitDebugger.Interfaces;

namespace AutoSplitDebugger
{
    public class ValueSource<T> : IValueSource where T : struct
    {
        public Dictionary<T, string> Map { get; set; }
        
        public ValueSource()
        {

        }

        public ValueSource(Dictionary<T, string> map)
        {
            Map = map;
        }

        public bool Contains(object obj)
        {
            if (obj is not T value) throw new ArgumentException($"{nameof(obj)} must be of type {typeof(T).Name}.");

            return Map.ContainsKey(value);
        }

        public string GetDisplayText(object obj)
        {
            if (obj is not T value) throw new ArgumentException($"{nameof(obj)} must be of type {typeof(T).Name}.");
            
            if (!Map.ContainsKey(value)) return null;

            return Map[value];
        }
    }
}
