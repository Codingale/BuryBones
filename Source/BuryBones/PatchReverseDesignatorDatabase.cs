using Verse;
using HarmonyLib;

namespace BuryBones
{
    /// <summary>
	/// Why god is this hard coded and not at startup or even called normally?
	/// </summary>
	[HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
    internal static class PatchReverseDesignatorDatabase
    {
        [HarmonyPostfix]
        public static void InjectReverseDesignators(ReverseDesignatorDatabase __instance)
        {
            __instance.AllDesignators.Add(new Designator_BuryCorpse());
        }
    }
}
