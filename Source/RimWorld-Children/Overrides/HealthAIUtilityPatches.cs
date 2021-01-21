using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorldChildren {
    public static class HealthAIUtilityPatches {
        [HarmonyPatch(typeof(HealthAIUtility), "ShouldSeekMedicalRestUrgent")]
        public static class ShouldSeekMedicalRestUrgent_Patch {
            /// <summary>
            /// Combined with RestUtility patches, this patch ensures that a baby pawn who is not
            /// truly in need of medical attention will be moved to a crib instead of rescued
            /// to a hospital bed. 
            /// </summary>
            [HarmonyPrefix]
            public static bool ShouldSeekMedicalRestUrgent_Prefix(Pawn pawn, bool __result) {
                if (!ChildrenUtility.ShouldUseCrib(pawn)) {
                    return true; //Default back to original flow
                }
                __result = true;
                if (!pawn.health.HasHediffsNeedingTend(false)) {
                    __result = HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn);
                }
                return false;
            }
        }
    }
}
