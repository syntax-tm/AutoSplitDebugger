using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using AutoSplitDebugger.Common.Converters;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.Models;
using DevExpress.Mvvm.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{ToString()}")]
public class PointerConfig
{
    /// <summary>
    /// The name of the pointer.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The <see cref="PointerDataType"/> of the pointer (eg. <see cref="PointerDataType._int"/>).
    /// </summary>
    [JsonConverter(typeof(StringEnumDescriptionConverter))]
    public PointerDataType Type { get; set; }
    /// <summary>
    /// The <b>standard</b> or <b>custom</b> format <see langword="string"/> used when converting the value to its equivalent <see langword="string"/> representation.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings" />
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"/>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings" />
    public string Format { get; set; }
    /// <summary>
    /// If <see langword="true"/>, will use a dedicated <see cref="BackgroundWorker"/> to perform refreshes.
    /// Otherwise, the default <see cref="BackgroundWorker"/> will request a refresh.
    /// </summary>
    /// <see cref="IPointerViewModel" />
    /// <seealso cref="IPointerViewModel.Refresh" />
    public bool IsPriority { get; set; }
    /// <summary>
    /// Indicates whether this pointer is to an in-game timer.
    /// </summary>
    public bool IsTime { get; set; }
    /// <summary>
    /// The <see cref="UnitOfTime"/> represented by the pointer (eg. <see cref="UnitOfTime.Seconds"/>).
    /// </summary>
    /// <remarks>This is only required when <see cref="IsTime"/> is <see langword="true"/>.</remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    public UnitOfTime? UnitOfTime { get; set; }
    /// <summary>
    /// An optional <see cref="ValueSourceConfig"/> for mapping values and their display text.
    /// </summary>
    /// <remarks>U</remarks>
    /// <see cref="ValueSource{T}"/>
    [JsonProperty("valueSource")]
    public ValueSourceConfig ValueSourceConfig { get; set; }
    /// <summary>
    /// The version-specific offsets for the pointer.
    /// </summary>
    public OffsetConfig[] Offsets { get; set; }

    public PointerConfig(OffsetConfig[] offsets)
    {
        Offsets = offsets;

        Offsets.ForEach(o => o.PointerConfig = this);
    }

    public OffsetConfig GetOffsets(ModuleVersionInfo versionInfo)
    {
        var offsetConfig = Offsets.FirstOrDefault(o => o.Version.Equals(versionInfo));

        return offsetConfig;
    }

    public override string ToString()
    {
        return $"{Name} ({Type.ToStringFast()})";
    }
}