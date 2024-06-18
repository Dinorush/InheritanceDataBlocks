using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using InheritanceDataBlocks.Utils;

namespace InheritanceDataBlocks;

[BepInPlugin("Dinorush." + MODNAME, MODNAME, "1.1.0")]
[BepInDependency("GTFO.InjectLib", BepInDependency.DependencyFlags.HardDependency)]
internal sealed class EntryPoint : BasePlugin
{
    public const string MODNAME = "InheritanceDataBlocks";

    public override void Load()
    {
        IDBLogger.Log("Loading " + MODNAME);
        Configuration.Init();
        DataBlockHandlerSetup.Init();

        Harmony harmonyInstance = new(MODNAME);
        harmonyInstance.PatchAll();

        IDBLogger.Log("Loaded " + MODNAME);
    }
}