using HarmonyLib;
using RimWorld;
using Verse;

namespace RimWorldChildren
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    internal static class BodyTypePost
    {
        [HarmonyPostfix]
        public static void GenerateBodyTypePost(ref Pawn pawn)
        {
            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
            {
                ChildrenUtility.ChangeBodyType(pawn, true, true);
            }
        }
    }
}

