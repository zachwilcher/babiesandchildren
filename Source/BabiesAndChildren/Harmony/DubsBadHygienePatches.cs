using System;
using System.Reflection;
using BabiesAndChildren.api;
using HarmonyLib;
using Verse;

namespace BabiesAndChildren.Harmony
{
    public static class DubsBadHygienePatches
    {
        private static Type workGiver_washPatientType;
        public static void Patch()
        {
            HarmonyLib.Harmony harmony =
                new HarmonyLib.Harmony("RimWorld.babies.and.children." + nameof(DubsBadHygienePatches));

            workGiver_washPatientType = AccessTools.TypeByName("WorkGiver_washPatient");

            MethodInfo original = AccessTools.Method(workGiver_washPatientType, "ShouldBeWashedBySomeone");

            HarmonyMethod postfix = new HarmonyMethod(typeof(DubsBadHygienePatches), nameof(ShouldBeWashedBySomeonePostfix));

            harmony.Patch(original, postfix: postfix);
        }

        private static void ShouldBeWashedBySomeonePostfix(Pawn pawn, ref bool __result) 
        {
            if (AgeStages.IsYoungerThan(pawn, AgeStages.Child))
            {
                __result = true;
            }
        }
    }
}