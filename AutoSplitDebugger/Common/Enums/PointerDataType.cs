using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NetEscapades.EnumGenerators;

namespace AutoSplitDebugger;

[DefaultValue(Unknown)]
[EnumExtensions]
public enum PointerDataType
{
    [Display(Name = "")]
    Unknown = 0,
    [Display(Name = "sbyte")]
    _sbyte,
    [Display(Name = "byte")]
    _byte,
    [Display(Name = "short")]
    _short,
    [Display(Name = "ushort")]
    _ushort,
    [Display(Name = "int")]
    _int,
    [Display(Name = "uint")]
    _uint,
    [Display(Name = "long")]
    _long,
    [Display(Name = "ulong")]
    _ulong,
    [Display(Name = "float")]
    _float,
    [Display(Name = "double")]
    _double,
    [Display(Name = "decimal")]
    _decimal
}
