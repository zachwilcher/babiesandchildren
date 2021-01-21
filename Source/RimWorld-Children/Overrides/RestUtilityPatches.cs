using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorldChildren {
    public static class RestUtilityPatches {

        /// <summary>
        /// Get the most appropriate bed list for a pawn. Child pawns will recieve 
        /// a bed list sorted to prioritize cribs
        /// </summary>
        /// <param name="pawn">The pawn being evaluated</param>
        /// <returns>Sorted list of beds </returns>
        private static List<ThingDef> GetSortedBeds_RestEffectiveness(Pawn pawn) {
            return (ChildrenUtility.ShouldUseCrib(pawn)) ? ChildrenUtility.AllBedDefBestToWorstCribRest : RestUtility.AllBedDefBestToWorst;
        }
        private static MethodInfo GetSortedBeds_MethodInfo = AccessTools.Method(typeof(RestUtilityPatches), nameof(GetSortedBeds_RestEffectiveness));

        // Babies wake up if they're unhappy
        [HarmonyPatch(typeof(RestUtility), "WakeThreshold")]
        public static class RestUtility_WakeThreshold_Patch {
            [HarmonyPostfix]
            internal static void WakeThreshold_Patch(ref float __result, ref Pawn p) {
                if (ChildrenUtility.GetAgeStage(p) < AgeStage.Child && p.health.hediffSet.HasHediff(ChildHediffDefOf.UnhappyBaby)) {
                    __result = 0.15f;
                }
                // Adults nearby wake up too
                if (ChildrenUtility.NearCryingBaby(p))
                {
                    __result = 0.15f;
                }
            }
        }
        [HarmonyPatch(typeof(RestUtility), "IsValidBedFor")]
        public static class RestUtility_IsValidBedFor_Patch {
            /// <summary>
            /// Failsafe check to ensure that any pawn looking for a bed will not find a crib if
            /// they do not belong in a crib
            /// </summary>
            [HarmonyPostfix]
            internal static void IsValidBedFor_Postfix(Pawn sleeper, Thing bedThing, bool __result) {
                if (ChildrenUtility.IsBedCrib((Building_Bed)bedThing) && !ChildrenUtility.ShouldUseCrib(sleeper)) {
                    __result = false;
                } 
            }
        }

        [HarmonyPatch(typeof(RestUtility), "FindBedFor", new[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(bool) })]
        public static class RestUtility_FindBedFor_Patch {
            /// <summary>
            /// Modify FindBedFor to pass in a different order for bedDefs when looking
            /// for the best bed for a pawn. We do not touch the order of beds for
            /// medical effectiveness, as a baby still needs to go to the most effective
            /// hospital bed if actually unwell.
            /// </summary>
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> FindBedFor_Transpiler(IEnumerable<CodeInstruction> instructions) {
                FieldInfo bedDefFieldInfo = AccessTools.Field(typeof(RestUtility), "bedDefsBestToWorst_RestEffectiveness");
                foreach (CodeInstruction instruction in instructions) {
                    //Convert all references to bedDefsBestToWorst_RestEffectiveness for toddler pawns
                    if (instruction.opcode == OpCodes.Ldsfld && instruction.OperandIs(bedDefFieldInfo)) {
                        yield return new CodeInstruction(OpCodes.Ldarg_1) { labels = instruction.labels.ListFullCopy() };
                        yield return new CodeInstruction(OpCodes.Call, GetSortedBeds_MethodInfo);
                    }
                    else {
                        yield return instruction;
                    }
                    
                }
            }
        }
        
    }
}
