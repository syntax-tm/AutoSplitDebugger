using System;
using System.Collections.Generic;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.Models;

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

        public object GetDisplay(object obj)
        {
            if (obj is not T value) throw new ArgumentException($"{nameof(obj)} must be of type {typeof(T).Name}.");
            
            if (!Map.ContainsKey(value)) return null;

            return new StringModel(Map[value]);
        }
    }
}
