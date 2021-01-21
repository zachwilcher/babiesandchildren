using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorldChildren
{
    // Prevents babies from taking food into inventory
    [HarmonyPatch(typeof(JobGiver_PackFood), "TryGiveJob")]
    public static class PackFood_Override
    {
        [HarmonyPostfix]
        internal static void TryGiveJob_Postfix(ref Pawn pawn, ref Job __result) {
            if(ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child && ChildrenUtility.RaceUsesChildren(pawn))
            {
                __result = null;
            }
        }
    }
}

