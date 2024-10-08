using GameData;
using HarmonyLib;
using InheritanceDataBlocks.Inheritance;
using System;

namespace InheritanceDataBlocks
{
    [HarmonyPatch(typeof(GameDataInit), nameof(GameDataInit.ReInitialize))]
    internal static class DataBlockReInitPatch
    {
        [HarmonyPriority(Priority.High)]
        [HarmonyWrapSafe]
        private static void Prefix()
        {
            InheritanceResolverManager.ResetResolvers();
        }
    }

    [HarmonyPatch(typeof(GameDataInit), nameof(GameDataInit.Initialize))]
    internal static class DataBlockInitPatch
    {
        [HarmonyPriority(Priority.High)]
        [HarmonyWrapSafe]
        private static void Postfix()
        {
            InheritanceResolverManager.RunResolvers();
        }
    }
}
