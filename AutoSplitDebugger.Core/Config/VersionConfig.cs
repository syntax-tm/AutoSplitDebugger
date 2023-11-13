using System;
using System.Diagnostics;
using System.Linq;
using AutoSplitDebugger.Core.Models;
using Newtonsoft.Json;

namespace AutoSplitDebugger.Core.Config
{
    [JsonObject(IsReference = true)]
    [DebuggerDisplay("{ModuleName} ({Name})")]
    public class VersionConfig : IComparable<VersionConfig>, IEquatable<VersionConfig>, IEquatable<ModuleVersionInfo>
    {
        public string Name { get; set; }
        public string ModuleName { get; set; }
        public uint[] ModuleSize { get; set; }

        public int CompareTo(VersionConfig other)
        {
            if (other == null) return 1;

            var currentKey = ToString();
            var otherKey = other.ToString();

            return string.Compare(currentKey, otherKey, StringComparison.Ordinal);
        }

        public bool Equals(VersionConfig other)
        {
            if (other == null) return false;
            
            var currentKey = ToString();
            var otherKey = other.ToString();

            return currentKey!.Equals(otherKey);
        }

        public bool Equals(ModuleVersionInfo other)
        {
            if (other == null) return false;

            if (!ModuleName.Equals(other.ModuleName)) return false;
            if (!ModuleSize.Contains(other.MainModuleSize)) return false;

            return true;
        }

        public override string ToString()
        {
            return $"{Name}|{ModuleName}|{string.Join(',', ModuleSize)}";
        }
    }
}
