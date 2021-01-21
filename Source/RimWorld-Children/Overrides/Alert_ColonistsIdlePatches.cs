
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace RimWorldChildren {
    [HarmonyPatch(typeof(Alert_ColonistsIdle), "IdleColonists", MethodType.Getter)]
    public static class Alert_ColonistsIdle_IdleColonists_Patch {
        [HarmonyPostfix]
        public static void IdleColonists_Getter_Patch(ref List<Pawn> __result) {
            __result.RemoveAll(pawn => ChildrenUtility.GetAgeStage(pawn) < 3);
        }
    }
}
