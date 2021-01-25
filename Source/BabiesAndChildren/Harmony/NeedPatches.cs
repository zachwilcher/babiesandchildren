using HarmonyLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren.Harmony {

    /// <summary>
    /// Babies should not develop a tolerance for social joy since this is currently their only available joy source.
    /// </summary>
    [HarmonyPatch(typeof(Need_Joy), "GainJoy")]
    public static class Need_Joy_GainJoy_Patch {
        [HarmonyPostfix]
        internal static void GainJoy_Patch(Need_Joy __instance, JoyKindDef joyKind, Pawn ___pawn) {
            if (ChildrenUtility.RaceUsesChildren(___pawn) && joyKind == JoyKindDefOf.Social && ChildrenUtility.GetAgeStage(___pawn) <= AgeStage.Toddler) {
                DefMap<JoyKindDef, float> tolerances = Traverse.Create(__instance).Field("tolerances").Field("tolerances").GetValue<DefMap<JoyKindDef, float>>();
                DefMap<JoyKindDef, bool> bored = Traverse.Create(__instance).Field("tolerances").Field("bored").GetValue<DefMap<JoyKindDef, bool>>();
                tolerances[JoyKindDefOf.Social] = 0;
                bored[JoyKindDefOf.Social] = false;
            }
        }
    }
}