using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren.Harmony {


public static class Alert_Patches 
{
    [HarmonyPatch(typeof(Alert_ColonistsIdle), "IdleColonists", MethodType.Getter)]
    public static class Alert_ColonistsIdle_IdleColonists_Patch {
        [HarmonyPostfix]
        public static void IdleColonists_Getter_Patch(ref List<Pawn> __result) {
            __result.RemoveAll(pawn => ChildrenUtility.GetAgeStage(pawn) < 3);
        }
    }
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class PawnEquipmentracker_NotifyEquipmentAdded_Patch
    {
        [HarmonyPostfix]
        internal static void Notify_EquipmentAdded_Patch(ref ThingWithComps eq, ref Pawn_EquipmentTracker __instance)
        {
            Pawn pawn = __instance.ParentHolder as Pawn;
            if (pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager && ChildrenUtility.RaceUsesChildren(pawn) && pawn.Faction.IsPlayer)
            {
                if (eq.def.BaseMass > ChildrenUtility.ChildMaxWeaponMass(pawn))
                {
                    Messages.Message("MessageWeaponTooLarge".Translate(eq.def.label, ((Pawn)__instance.ParentHolder).Name.ToStringShort), MessageTypeDefOf.CautionInput);
                }
            }
        }
    }
}
}
