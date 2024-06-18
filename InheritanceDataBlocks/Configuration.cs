using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InheritanceDataBlocks
{
    internal static class Configuration
    {
        public static bool DebugChains { get; private set; } = false;

        public static void Init()
        {
            BindAll(new ConfigFile(Path.Combine(Paths.ConfigPath, EntryPoint.MODNAME + ".cfg"), saveOnInit: true));
        }

        private static void BindAll(ConfigFile config)
        {
            string section = "General Settings";
            string key = "Enable Debug Logging";
            string description = "Prints inheritance chains to the console when data blocks are loaded (useful when debugging broken child blocks).";
            DebugChains = config.Bind(new ConfigDefinition(section, key), DebugChains, new ConfigDescription(description, null)).Value;
        }
    }
}
