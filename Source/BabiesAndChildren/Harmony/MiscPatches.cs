using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch]
    public static class MiscPatches
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
        
        // Causes children to drop too-heavy weapons and potentially hurt themselves on firing
        [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
        public static class VerbShoot_TryCastShot_Patch
        {
            [HarmonyPostfix]
            internal static void TryCastShot_Patch(ref Verb_Shoot __instance)
            {
                Pawn pawn = __instance.CasterPawn;
                if (pawn != null && ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Child && pawn.Faction.IsPlayer)
                {
                    // The weapon is too heavy and the child will (likely) drop it when trying to fire
                    if (__instance.EquipmentSource.def.BaseMass > ChildrenUtility.ChildMaxWeaponMass(pawn))
                    {

                        ThingWithComps benis;
                        pawn.equipment.TryDropEquipment(__instance.EquipmentSource, out benis, pawn.Position, false);

                        float recoilForce = (__instance.EquipmentSource.def.BaseMass - 3);

                        if (recoilForce > 0)
                        {
                            string[] hitPart = {
                                "Torso",
                                "Shoulder",
                                "Arm",
                                "Hand",
                                "Head",
                                "Neck",
                                "Eye",
                                "Nose",
                            };
                            int hits = Rand.Range(1, 4);
                            while (hits > 0)
                            {
                                pawn.TakeDamage(new DamageInfo(DamageDefOf.Blunt, (int)((recoilForce + Rand.Range(0f, 3f)) / hits), 0, -1, __instance.EquipmentSource,
                                    ChildrenUtility.GetPawnBodyPart(pawn, hitPart.RandomElement<String>()), null));
                                hits--;
                            }
                        }
                    }
                }
            }
        }
        // Children are downed easier
        [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDowned")]
        public static class Pawn_HealtherTracker_ShouldBeDowned_Patch
        {
            [HarmonyPostfix]
            internal static void SBD(ref Pawn_HealthTracker __instance, ref bool __result)
            {
                Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
                if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
                {
                    __result = __instance.hediffSet.PainTotal >= pawn.GetStatValue(StatDefOf.PainShockThreshold, true) * 0.75f || !__instance.capacities.CanBeAwake || !__instance.capacities.CapableOf(PawnCapacityDefOf.Moving);
                }
            }
        }
    }
}