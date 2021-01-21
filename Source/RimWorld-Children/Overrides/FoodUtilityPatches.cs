using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorldChildren
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
