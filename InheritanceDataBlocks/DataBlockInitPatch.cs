using GameData;
using HarmonyLib;
using InheritanceDataBlocks.Inheritance;

namespace InheritanceDataBlocks
{
    [HarmonyPatch(typeof(GameDataInit), nameof(GameDataInit.Initialize))]
    internal static class DataBlockReInitPatch
    {
        [HarmonyWrapSafe]
        private static void Prefix()
        {
            InheritanceResolverManager.ResetResolvers();
        }

        [HarmonyPriority(Priority.High)]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        private static void Postfix()
        {
            InheritanceResolverManager.RunResolvers();
        }
    }
}
