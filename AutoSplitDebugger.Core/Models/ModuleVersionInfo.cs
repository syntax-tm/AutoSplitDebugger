using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AutoSplitDebugger.Core.Config;

namespace AutoSplitDebugger.Core.Models
{
    public class ModuleVersionInfo
    {

        public string Version { get; set; }
        public string ModuleName { get; set; }
        public uint MainModuleSize { get; set; }

        protected ModuleVersionInfo()
        {

        }

        public ModuleVersionInfo(string version, string moduleName, uint mainModuleSize)
        {
            Version = version;
            ModuleName = moduleName;
            MainModuleSize = mainModuleSize;
        }

        public bool IsMatch(Process p)
        {
            var mainModule = p.MainModule;

            Debug.Assert(mainModule != null);

            var moduleName = Path.HasExtension(mainModule.ModuleName)
                ? Path.GetFileNameWithoutExtension(mainModule.ModuleName)
                : mainModule.ModuleName;

            if (!moduleName.EqualsIgnoreCase(ModuleName)) return false;
            if (mainModule.ModuleMemorySize != MainModuleSize) return false;

            return true;
        }

        public static List<ModuleVersionInfo> Create(VersionConfig config)
        {
            var info = new List<ModuleVersionInfo>();

            foreach (var moduleSize in config.ModuleSize)
            {
                info.Add(new ()
                {
                    Version = config.Name,
                    ModuleName = config.ModuleName,
                    MainModuleSize = moduleSize
                });
            }

            return info;
        }

    }
}
