using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip", 
        new[] { 
            typeof(Thing), 
            typeof(Pawn), 
            typeof(string) 
        }
        , new[]{
            ArgumentType.Normal, 
            ArgumentType.Normal, 
            ArgumentType.Out
        })]
    internal static class EquipmentUtility_CanEquip_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref bool __result, Thing thing, Pawn pawn, out string cantReason)
        {
            cantReason = null;
            if (thing.def.thingSetMakerTags != null)
            {
                // prevent not a child equip toy
                if (thing.def.thingSetMakerTags.Contains("Toy") && ChildrenUtility.GetAgeStage(pawn) != AgeStage.Child)
                {
                    cantReason = "OnlyChildrenCanEquip".Translate();
                    __result = false;
                    return false;
                }
                // prevent not a toddler equip babysuit
                else if (thing.def.thingSetMakerTags.Contains("BabyGear") && ChildrenUtility.GetAgeStage(pawn) != AgeStage.Toddler)
                {
                    cantReason = "OnlyForUprightToddler".Translate();
                    __result = false;
                    return false;

                }
            }
            // prevent a toddler equip adultsuit
            if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child && pawn.RaceProps.lifeStageAges.Count == 5)
            {
                if (thing.def.thingSetMakerTags == null || !thing.def.thingSetMakerTags.Contains("BabyGear"))
                {
                    cantReason = "BabyCantEquipNormal".Translate();
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HealthAIUtility), "ShouldSeekMedicalRestUrgent")]
    internal static class HealthAiUtility_ShouldSeekMedicalRestUrgent_Patch
    {
        /// <summary>
        /// Combined with RestUtility patches, this patch ensures that a baby pawn who is not
        /// truly in need of medical attention will be moved to a crib instead of rescued
        /// to a hospital bed. 
        /// </summary>
        [HarmonyPrefix]
        static bool Prefix(Pawn pawn, ref bool __result)
        {
            if (!ChildrenUtility.ShouldUseCrib(pawn))
            {
                return true; //Default back to original flow
            }

            __result = true;
            if (!pawn.health.HasHediffsNeedingTend(false))
            {
                __result = HealthAIUtility.ShouldHaveSurgeryDoneNow(pawn);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "ShouldBeFedBySomeone")]
    internal static class FoodUtility_ShouldBeFedBySomeone_Patch
    {
        /// <summary>
        /// 
        /// </summary>
        [HarmonyPostfix]
        static void Postfix(Pawn pawn, ref bool __result)
        {
            if (ChildrenUtility.ShouldBeFed(pawn))
            {
                __result = true;
            }
        }
    }

    // Creates a blacklist of thoughts Toddlers cannot have
    [HarmonyPatch(typeof(ThoughtUtility), "CanGetThought")]
    internal static class ThoughtUtility_CanGetThought_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn pawn, ref ThoughtDef def, ref bool __result)
        {
            // Toddlers and younger can't get these thoughts
            if (ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Toddler && ChildrenUtility.RaceUsesChildren(pawn))
            {
                List<ThoughtDef> thoughtlist = new List<ThoughtDef>
                {
                    ThoughtDefOf.AteWithoutTable,
                    ThoughtDefOf.KnowPrisonerDiedInnocent,
                    ThoughtDefOf.KnowPrisonerSold,
                    ThoughtDefOf.Naked,
                    ThoughtDefOf.SleepDisturbed,
                    ThoughtDefOf.SleptOnGround,
                    ThoughtDef.Named("NeedOutdoors"),
                    ThoughtDef.Named("SleptInBarracks"),
                    ThoughtDef.Named("Expectations")
                };
                __result = __result && !thoughtlist.Contains(def);
            }
        }
    }

    [HarmonyPatch(typeof(Alert_ColonistsIdle), "IdleColonists", MethodType.Getter)]
    internal static class Alert_ColonistsIdle_IdleColonists_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref List<Pawn> __result)
        {
            __result.RemoveAll(pawn => ChildrenUtility.GetAgeStage(pawn) < 3);
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    internal static class PawnEquipmentracker_NotifyEquipmentAdded_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref ThingWithComps eq,
            ref Pawn_EquipmentTracker __instance)
        {
            Pawn pawn = __instance.ParentHolder as Pawn;
            if (pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager && ChildrenUtility.RaceUsesChildren(pawn) &&
                pawn.Faction.IsPlayer)
            {
                if (eq.def.BaseMass > ChildrenUtility.ChildMaxWeaponMass(pawn))
                {
                    Messages.Message(
                        "MessageWeaponTooLarge".Translate(eq.def.label,
                            ((Pawn) __instance.ParentHolder).Name.ToStringShort), MessageTypeDefOf.CautionInput);
                }
            }
        }
    }

    // Causes children to drop too-heavy weapons and potentially hurt themselves on firing
    [HarmonyPatch(typeof(Verb_Shoot), "TryCastShot")]
    internal static class VerbShoot_TryCastShot_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Verb_Shoot __instance)
        {
            Pawn pawn = __instance.CasterPawn;
            if (pawn != null && ChildrenUtility.RaceUsesChildren(pawn) &&
                ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Child && pawn.Faction.IsPlayer)
            {
                // The weapon is too heavy and the child will (likely) drop it when trying to fire
                if (__instance.EquipmentSource.def.BaseMass > ChildrenUtility.ChildMaxWeaponMass(pawn))
                {
                    ThingWithComps benis;
                    pawn.equipment.TryDropEquipment(__instance.EquipmentSource, out benis, pawn.Position, false);

                    float recoilForce = (__instance.EquipmentSource.def.BaseMass - 3);

                    if (recoilForce > 0)
                    {
                        string[] hitPart =
                        {
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
                            pawn.TakeDamage(new DamageInfo(DamageDefOf.Blunt,
                                (int) ((recoilForce + Rand.Range(0f, 3f)) / hits), 0, -1,
                                __instance.EquipmentSource,
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
    internal static class Pawn_HealtherTracker_ShouldBeDowned_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn_HealthTracker __instance, ref bool __result)
        {
            Pawn pawn = (Pawn) AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
            {
                __result =
                    __instance.hediffSet.PainTotal >=
                    pawn.GetStatValue(StatDefOf.PainShockThreshold, true) * 0.75f ||
                    !__instance.capacities.CanBeAwake || !__instance.capacities.CapableOf(PawnCapacityDefOf.Moving);
            }
        }
    }
}
