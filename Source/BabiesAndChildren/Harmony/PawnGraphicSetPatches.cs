using HarmonyLib;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch]
    public static class PawnGraphicSetPatches
    {
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public static class ResolveAllGraphics
    {
        [HarmonyPostfix]
        internal static void Postfix(ref PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            PawnGraphicSet _this = __instance;
            if (ChildrenUtility.RaceUsesChildren(pawn))
            {
                GraphicTools.ResolveAgeGraphics(__instance);
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    _this.ResolveApparelGraphics();
                });
            }
        }
    }
        
    }
}