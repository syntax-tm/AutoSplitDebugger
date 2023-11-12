using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoSplitDebugger.Models;

namespace AutoSplitDebugger.Config;

[DebuggerDisplay("{Title}")]
public class AutoSplitConfig
{
    public string Title { get; set; }
    public VersionConfig[] Versions { get; set; }
    public PointerConfig[] Pointers { get; set; }
    public ValueSourceConfig[] ValueSources { get; set; }

    public IEnumerable<ModuleVersionInfo> ModuleVersions => Versions.SelectMany(ModuleVersionInfo.Create);
}
