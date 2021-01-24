using HarmonyLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren.Harmony
{
    public static class FoodUtilityPatches
    {
        [HarmonyPatch(typeof(FoodUtility), "ShouldBeFedBySomeone")]
        public static class ShouldBeFedBySomeone_Patch
        {
            /// <summary>
            /// 
            /// </summary>
            [HarmonyPostfix]
            internal static void ShouldBeFedBySomeone_Postfix(Pawn pawn, ref bool __result)
            {
                if (ChildrenUtility.ShouldBeFed(pawn))
                {
                    __result = true;
                }
            }
        }
    }
}
