using System;
using System.ComponentModel;
using System.Diagnostics;
using AutoSplitDebugger.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{ToString()}")]
public class PointerConfig
{
    private readonly string[] _rawOffsets;

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
    /// The offsets used to initialize the pointer.
    /// </summary>
    /// <remarks>Supports normal and multi-level pointers.</remarks>
    public int[] Offsets { get; set; }

    public PointerConfig(string[] offsets)
    {
        _rawOffsets = offsets;
            
        LoadOffsets();
    }

    public override string ToString()
    {
        return $"{Name} ({Type.ToStringFast()})";
    }

    private void LoadOffsets()
    {
        try
        {
            Offsets = new int[_rawOffsets.Length];

            for (var i = 0; i < _rawOffsets.Length; i++)
            {
                var rawOffset = _rawOffsets[i];
                var isHex = rawOffset.StartsWith("0x");

                if (isHex)
                {
                    Offsets[i] = Convert.ToInt32(rawOffset, 16);
                }
                else
                {
                    Offsets[i] = int.Parse(rawOffset);
                }
            }
        }
        catch (Exception e)
        {
            var message = $"An error occurred loading the offsets for {nameof(PointerConfig)} '{Name}'. {e.Message}";

            throw new (message, e);
        }
    }
}