using BabiesAndChildren.Tools;
using HarmonyLib;
using Verse;

namespace BabiesAndChildren.Harmony
{

    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    internal static class PawnGenerator_GenerateBodyType_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn pawn)
        {
            if (RaceUtility.PawnUsesChildren(pawn) && AgeStage.IsYoungerThan(pawn, AgeStage.Teenager))
            {
                ChildrenUtility.ChangeBodyType(pawn, true, true);
            }
        }
    }
}