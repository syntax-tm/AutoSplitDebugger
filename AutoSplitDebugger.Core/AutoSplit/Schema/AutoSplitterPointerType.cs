using System.ComponentModel;

namespace AutoSplitDebugger.Core.AutoSplit.Schema;

public enum AutoSplitterPointerType
{
    /// <summary>
    /// <c>sbyte</c>
    /// </summary>
    [Description("sbyte")]
    _sbyte,
    /// <summary>
    /// <c>byte</c>
    /// </summary>
    [Description("byte")]
    _byte,
    /// <summary>
    /// <c>short</c>
    /// </summary>
    [Description("short")]
    _short,
    /// <summary>
    /// <c>ushort</c>
    /// </summary>
    [Description("ushort")]
    _ushort,
    /// <summary>
    /// <c>int</c>
    /// </summary>
    [Description("int")]
    _int,
    /// <summary>
    /// <c>uint</c>
    /// </summary>
    [Description("uint")]
    _uint,
    /// <summary>
    /// <c>long</c>
    /// </summary>
    [Description("long")]
    _long,
    /// <summary>
    /// <c>ulong</c>
    /// </summary>
    [Description("ulong")]
    _ulong,
    /// <summary>
    /// <c>float</c>
    /// </summary>
    [Description("float")]
    _float,
    /// <summary>
    /// <c>double</c>
    /// </summary>
    [Description("double")]
    _double,
    /// <summary>
    /// <c>bool</c>
    /// </summary>
    [Description("bool")]
    _bool,
    /// <summary>
    /// <c>string&lt;length&gt;</c>
    /// </summary>
    [Description("string[]")]
    _string,
    /// <summary>
    /// <c>byte[length]</c>
    /// </summary>
    [Description("byte[]")]
    _byteArray
}
