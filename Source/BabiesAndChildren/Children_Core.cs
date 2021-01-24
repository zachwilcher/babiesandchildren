using HarmonyLib;
using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BabiesAndChildren
{
    public class ChildrenBase : ModBase
    {
        public static ChildrenBase Instance { get; private set; }
        public override string ModIdentifier
        {
            get { return "Babies_and_Children"; }
        }

        //public static SettingHandle<bool> enable_postpartum;
        //public static SettingHandle<int> option_child_max_weapon_mass;
        //public static SettingHandle<bool> accelerated_growth;
        //public static SettingHandle<int> accelerated_growth_end_age;
        //public static SettingHandle<int> baby_accelerated_growth;
        //public static SettingHandle<int> toddler_accelerated_growth;
        //public static SettingHandle<int> child_accelerated_growth;
        //public static SettingHandle<int> teenager_accelerated_growth;
        //public static SettingHandle<float> cryVolume;
        //public static SettingHandle<bool> debug_and_gsetting;
        //public static SettingHandle<bool> child_cute_act_enabled;

        ////Graphic Setting
        //public static SettingHandle<bool> human_like_head_enabled;
        //public static SettingHandle<bool> Rabbie_Child_head_enabled;
        //public static SettingHandle<float> HumanBodySize;
        //public static SettingHandle<float> HumanHeadSize;
        //public static SettingHandle<float> HumanHairSize;
        //public static SettingHandle<float> HumanrootlocZ;
        //public static SettingHandle<float> AlienBodySize;
        //public static SettingHandle<float> AlienHeadSizeA;
        //public static SettingHandle<float> AlienHeadSizeB;
        //public static SettingHandle<float> AlienHairSize;
        //public static SettingHandle<float> AlienrootlocZ;

        //// for showhair
        //public static SettingHandle<float> ShowHairSize;
        //public static SettingHandle<float> ShowHairLocY;
        //public static SettingHandle<float> ShowHairHumanLocZ;
        //public static SettingHandle<float> ShowHairAlienLocZ;
        //public static SettingHandle<float> ShowHairAlienHFLocZ;

        //public static SettingHandle<int> GestationPeriodDays;

        //// for Initialize
        //public static SettingHandle<bool> OncePerGame;

        //public enum BabyInheritPercentageHandleEnum
        //{
        //    _None,
        //    _25,
        //    _50,
        //    _75,
        //    _90,
        //    _Random,
        //}
        //public static SettingHandle<BabyInheritPercentageHandleEnum> baby_Inherit_percentage;

        public static bool ModCSL_ON = false;
        public static bool ModRJW_ON = false;
        public static bool ModWIP_ON = false;

        //public override void DefsLoaded()
        //{
        //    bool GrowthVisible() => accelerated_growth;
        //    bool GSettingVisible() => debug_and_gsetting;

        //    //max_traits = Settings.GetHandle<int>("max_traits", "MaxTraitsSetting_title".Translate(), "MaxTraitsSetting_desc".Translate(), 3, Validators.IntRangeValidator(0, 15));
        //    //random_traits = Settings.GetHandle<int>("random_traits", "RandTraitsSetting_title".Translate(), "RandTraitsSetting_desc".Translate(), 2, Validators.IntRangeValidator(0, 15));          
        //    accelerated_growth = Settings.GetHandle<bool>("accelerated_growth", "AcceleratedGrowth_title".Translate(), "AcceleratedGrowth_desc".Translate(), true, null);
        //    accelerated_growth_end_age = Settings.GetHandle<int>("accelerated_growth_end_age", "AcceleratedGrowthEndAge_title".Translate(), "AcceleratedGrowthEndAge_desc".Translate(), 14, Validators.IntRangeValidator(0, 18));
        //    accelerated_growth_end_age.SpinnerIncrement = 1;
        //    accelerated_growth_end_age.VisibilityPredicate = GrowthVisible;
        //    baby_accelerated_growth = Settings.GetHandle<int>("baby_accelerated_growth", "BabyAcceleratedGrowth_title".Translate(), "BabyAcceleratedGrowth_desc".Translate(), 7, Validators.IntRangeValidator(1, MAX_ACCELRATED_GROWTH_FACTOR));
        //    baby_accelerated_growth.SpinnerIncrement = 1;
        //    baby_accelerated_growth.VisibilityPredicate = GrowthVisible;
        //    toddler_accelerated_growth = Settings.GetHandle<int>("toddler_accelerated_growth", "ToddlerAcceleratedGrowth_title".Translate(), "ToddlerAcceleratedGrowth_desc".Translate(), 7, Validators.IntRangeValidator(1, MAX_ACCELRATED_GROWTH_FACTOR));
        //    toddler_accelerated_growth.SpinnerIncrement = 1;
        //    toddler_accelerated_growth.VisibilityPredicate = GrowthVisible;
        //    child_accelerated_growth = Settings.GetHandle<int>("child_accelerated_growth", "ChildAcceleratedGrowth_title".Translate(), "ChildAcceleratedGrowth_desc".Translate(), 7, Validators.IntRangeValidator(1, MAX_ACCELRATED_GROWTH_FACTOR));
        //    child_accelerated_growth.SpinnerIncrement = 1;
        //    child_accelerated_growth.VisibilityPredicate = GrowthVisible;
        //    teenager_accelerated_growth = Settings.GetHandle<int>("teenager_accelerated_growth", "TeenagerAcceleratedGrowth_title".Translate(), "TeenagerAcceleratedGrowth_desc".Translate(), 1, Validators.IntRangeValidator(1, MAX_ACCELRATED_GROWTH_FACTOR));
        //    teenager_accelerated_growth.SpinnerIncrement = 1;
        //    teenager_accelerated_growth.VisibilityPredicate = GrowthVisible;

        //    //baby_inherit_traits = Settings.GetHandle<BabyInheritTraitsHandleEnum>("baby_inherit_traits", "BabyInheritTraits_Title".Translate(), "BabyInheritTraits_Desc".Translate(), BabyInheritTraitsHandleEnum._InheritedAndRandomTraits, null, "BabyInheritTraits_Desc");
        //    baby_Inherit_percentage = Settings.GetHandle<BabyInheritPercentageHandleEnum>("baby_Inherit_percentage", "BabyInheritPercentage_Title".Translate(), "BabyInheritPercentage_Desc".Translate(), BabyInheritPercentageHandleEnum._50, null, "BabyInheritPercentage_Desc");
        //    option_child_max_weapon_mass = Settings.GetHandle<int>("option_child_max_weapon_mass", "OptionChildMaxWeaponMass".Translate(), "OptionChildMaxWeaponMass_desc".Translate(), 2, Validators.IntRangeValidator(0, 5));
        //    GestationPeriodDays = Settings.GetHandle<int>("GestationPeriodDays", "GestationPeriodDays_Title".Translate(), "GestationPeriodDays_desc".Translate(), 45, Validators.FloatRangeValidator(1, 45));            
        //    enable_postpartum = Settings.GetHandle<bool>("enable_postpartum", "EnablePostPartum_title".Translate(), "EnablePostPartum_desc".Translate(), true, null);
        //    cryVolume = Settings.GetHandle<float>("cryVolume", "CryVolumeSetting_title".Translate(), "CryVolumeSetting_desc".Translate(), 0.8f, Validators.FloatRangeValidator(0.0f, 1f));

        //    //gestation_period_factor = Settings.GetHandle<float>("gestation_period_factor", "GestationPeriodFactor_title".Translate(), "GestationPeriodFactor_desc".Translate(), 1, Validators.FloatRangeValidator(0.1f, 1));
        //    debug_and_gsetting = Settings.GetHandle<bool>("debug_messages_enabled", "DebugInfoMessagesEnabled_Title".Translate(), "DebugInfoMessagesEnabled_desc".Translate(), false, null);

        //    //graphics.setting
        //    child_cute_act_enabled = Settings.GetHandle<bool>("child_cute_act_enabled", "Child cute act enabled".Translate(), "Child cute act enabled".Translate(), true, null);
        //    child_cute_act_enabled.VisibilityPredicate = GSettingVisible;
        //    human_like_head_enabled = Settings.GetHandle<bool>("human_like_head_enabled", "humanlikeHeadEnabled_Title".Translate(), "humanlikeHeadEnabled_desc".Translate(), true, null);
        //    human_like_head_enabled.VisibilityPredicate = GSettingVisible;
        //    Rabbie_Child_head_enabled = Settings.GetHandle<bool>("Rabbie_Child_head_enabled", "Rabbie child head enabled".Translate(), "Rabbie child head enabled".Translate(), true, null);
        //    Rabbie_Child_head_enabled.VisibilityPredicate = GSettingVisible;
        //    HumanBodySize = Settings.GetHandle<float>("HumanBodySize", "HumanChildrenBodysize_Title".Translate(), "HumanChildrenBodysize_desc".Translate(), 0.713f, Validators.FloatRangeValidator(-2f, 2f));
        //    HumanBodySize.VisibilityPredicate = GSettingVisible;
        //    HumanHeadSize = Settings.GetHandle<float>("HumanHeadSize", "HumanChildrenHeadsize_Title".Translate(), "HumanChildrenHeadsize_desc".Translate(), 0.922f, Validators.FloatRangeValidator(-2f, 2f));
        //    HumanHeadSize.VisibilityPredicate = GSettingVisible;
        //    HumanHairSize = Settings.GetHandle<float>("HumanHairSize", "HumanChildrenHairsize_Title".Translate(), "HumanChildrenZroc_desc".Translate(), 0.730f, Validators.FloatRangeValidator(-2f, 2f));
        //    HumanHairSize.VisibilityPredicate = GSettingVisible;
        //    HumanrootlocZ = Settings.GetHandle<float>("HumanrootlocZ", "HumanChildrenZroc_Title".Translate(), "HumanChildrenZroc_desc".Translate(), -0.11f, Validators.FloatRangeValidator(-2f, 2f));
        //    HumanrootlocZ.VisibilityPredicate = GSettingVisible;
        //    AlienBodySize = Settings.GetHandle<float>("AlienBodySize", "AlienChildrenBodysize_Title".Translate(), "AlienChildrenBodysize_desc".Translate(), 0.842f, Validators.FloatRangeValidator(-2f, 2f));
        //    AlienBodySize.VisibilityPredicate = GSettingVisible;
        //    AlienHeadSizeA = Settings.GetHandle<float>("AlienHeadSizeA", "AlienChildrenOriginalHeadsize_Title".Translate(), "AlienChildrenOriginalHeadsize_desc".Translate(), 0.767f, Validators.FloatRangeValidator(-2f, 2f));
        //    AlienHeadSizeA.VisibilityPredicate = GSettingVisible;
        //    AlienHeadSizeB = Settings.GetHandle<float>("AlienHeadSizeB", "AlienChildrenHumanlikeHeadsize_Title".Translate(), "AlienChildrenHumanlikeHeadsize_desc".Translate(), 0.767f, Validators.FloatRangeValidator(-2f, 2f));
        //    AlienHeadSizeB.VisibilityPredicate = GSettingVisible;
        //    AlienHairSize = Settings.GetHandle<float>("AlienHairSize", "AlienChildrenHairsize_Title".Translate(), "AlienChildrenHairsize_desc".Translate(), 0.832f, Validators.FloatRangeValidator(-2f, 2f));
        //    AlienHairSize.VisibilityPredicate = GSettingVisible;
        //    AlienrootlocZ = Settings.GetHandle<float>("AlienrootlocZ", "AlienChildrenZroc_Title".Translate(), "AlienChildrenZroc_desc".Translate(), -0.038f, Validators.FloatRangeValidator(-2f, 2f));
        //    AlienrootlocZ.VisibilityPredicate = GSettingVisible;

        //    //for showhair
        //    ShowHairSize = Settings.GetHandle<float>("ShowHairSize", "Show Hair Size".Translate(), "Show Hair Size".Translate(), 1f, Validators.FloatRangeValidator(-2f, 2f));
        //    ShowHairSize.VisibilityPredicate = GSettingVisible;
        //    ShowHairLocY = Settings.GetHandle<float>("ShowHairLocY", "Show Hair Loc Y".Translate(), "Show Hair Loc Y".Translate(), -0.003f, Validators.FloatRangeValidator(-2f, 2f));
        //    ShowHairLocY.VisibilityPredicate = GSettingVisible;
        //    ShowHairHumanLocZ = Settings.GetHandle<float>("ShowHairLocZ", "ShowHair Human Z Loc".Translate(), "Show Hair Loc Human Z".Translate(), 0.092f, Validators.FloatRangeValidator(-2f, 2f));
        //    ShowHairHumanLocZ.VisibilityPredicate = GSettingVisible;
        //    ShowHairAlienLocZ = Settings.GetHandle<float>("ShowHairAlienLocZ", "ShowHair Alien Z Loc".Translate(), "ShowHair Alien Z Loc".Translate(), 0.035f, Validators.FloatRangeValidator(-2f, 2f));
        //    ShowHairAlienLocZ.VisibilityPredicate = GSettingVisible;
        //    ShowHairAlienHFLocZ = Settings.GetHandle<float>("ShowHairAlienHFLocZ", "ShowHair Alien HF Z Loc".Translate(), "ShowHair Alien HF Z Loc".Translate(), 0.048f, Validators.FloatRangeValidator(-2f, 2f));
        //    ShowHairAlienHFLocZ.VisibilityPredicate = GSettingVisible;

        //    OncePerGame = Settings.GetHandle<bool>("OncePerGame", "OncePerGame_title".Translate(), "OncePerGame_desc".Translate(), false, null);
        //    OncePerGame.NeverVisible = true;
        //}
        public ChildrenBase() { Instance = this; }

        public override void MapLoaded(Map map)
        {
            if (map == Current.Game.CurrentMap)
            {
                if (!BnCSettings.OncePerGame)
                {
                    //Log.Message("=============BNC Once Per Game=============");
                    foreach (Map tmap in Current.Game.Maps)
                    {
                        foreach (Pawn pawn in tmap.mapPawns.AllPawnsSpawned)
                        {
                            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
                            {
                                ChildrenUtility.ChangeBodyType(pawn, true, false);
                                ChildrenUtility.ChangeChildBackstory(pawn);
                            }
                        }
                    }
                    BnCSettings.OncePerGame = true;
                }
                if (BnCSettings.GestationPeriodDays_Enable)
                {
                    foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
                    {
                        if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                        {
                            thingDef.race.gestationPeriodDays = (float)BnCSettings.GestationPeriodDays;
               
                        }
                    }
                }
            }
        }
    }

    // Handy for more readable code when age-checking
    public static class AgeStage
    {
        public const int Baby = 0;
        public const int Toddler = 1;
        public const int Child = 2;
        public const int Teenager = 3;
        public const int Adult = 4;
    }

    internal static class TranspilerHelper
    {
        internal static List<ThingStuffPair> FilterChildWeapons(Pawn pawn, List<ThingStuffPair> weapons)
        {
            var weapons_out = new List<ThingStuffPair>();
            if (weapons.Count > 0)
                foreach (ThingStuffPair weapon in weapons)
                {
                    if (weapon.thing.BaseMass < ChildrenUtility.ChildMaxWeaponMass(pawn))
                    {
                        weapons_out.Add(weapon);
                    }
                }
            return weapons_out;
        }

        //[HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
        //static class JobDriver_OptimizeApparel_Patch
        //{
        //    [HarmonyPostfix]
        //    static void TryGiveJob_Patch(JobGiver_OptimizeApparel __instance, ref Job __result, Pawn pawn)
        //    {
        //        if (__result != null)
        //        {
        //            Stop the game from automatically allocating pawns Wear jobs they cannot fulfil
        //            if ((ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Toddler && !ChildrenUtility.SetMakerTagCheck(__result.targetA.Thing, "BabyGear") && ChildrenUtility.RaceUsesChildren(pawn))
        //                    || (ChildrenUtility.GetAgeStage(pawn) >= AgeStage.Child && ChildrenUtility.SetMakerTagCheck(__result.targetA.Thing, "BabyGear")))
        //            {
        //                __result = null;
        //            }
        //        }
        //        else
        //        {
        //           //makes pawn to remove baby clothes when too old for them.
        //           List<Apparel> wornApparel = pawn.apparel.WornApparel;
        //            if (ChildrenUtility.GetAgeStage(pawn) > AgeStage.Toddler)
        //                {
        //                    for (int i = wornApparel.Count - 1; i >= 0; i--)
        //                    {
        //                        if (ChildrenUtility.SetMakerTagCheck(wornApparel[i], "BabyGear"))
        //                        {
        //                            __result = new Job(JobDefOf.RemoveApparel, wornApparel[i])
        //                            {
        //                                haulDroppedApparel = true
        //                            };
        //                            return;
        //                        }
        //                    }
        //                }
        //        }
        //    }
        //}
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

    // 
    //[HarmonyPatch(typeof(Pawn_HealthTracker), "InPainShock")]
    //public static class Pawn_HealtherTracker_ShouldBeDowned_Patch
    //{
    //    [HarmonyPostfix]
    //    internal static void InPainShock_Patch(ref Pawn_HealthTracker __instance, ref bool __result)
    //    {
    //        Pawn pawn = (Pawn)AccessTools.Field(typeof(Pawn_HealthTracker), "pawn").GetValue(__instance);
    //        if (pawn.RaceProps.Humanlike && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
    //        {
    //            __result = __instance.hediffSet.PainTotal >= pawn.GetStatValue(StatDefOf.PainShockThreshold, true)*0.75f;
    //        }
    //    }
    //}

    // Stops pawns from randomly dying when downed
    // This is a necessary patch for preventing children from dying on pawn generation unfortunately
    //[HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
    //public static class Pawn_HealthTracker_CheckForStateChange_Patch
    //{
    //    [HarmonyTranspiler]
    //    static IEnumerable<CodeInstruction> CheckForStateChange_Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        List<CodeInstruction> ILs = instructions.ToList();

    //        MethodInfo isPlayerFaction = typeof(Faction).GetProperty("IsPlayer").GetGetMethod();
    //        int index = ILs.FindIndex(IL => IL.opcode == OpCodes.Callvirt && IL.operand == isPlayerFaction) + 2;
    //        List<CodeInstruction> injection = new List<CodeInstruction> {
    //            new CodeInstruction(OpCodes.Br, ILs[index-1].operand),
    //        };
    //        ILs.InsertRange(index, injection);

    //        foreach (CodeInstruction instruction in ILs)
    //        {
    //            yield return instruction;
    //        }
    //    }
    //}

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

    // Gives a notification that the weapon a child has picked up is dangerous for them to handle

    // Prevents children from being spawned with weapons too heavy for them
    // (For example in raids a child might otherwise spawn with an auto-shotty and promptly drop it the first time
    // they fire it at you. Which would be silly.)
    //[HarmonyPatch(typeof(PawnWeaponGenerator), "TryGenerateWeaponFor")]
    //public static class PawnWeaponGenerator_TryGenerateWeaponFor_Patch
    //{
    //	[HarmonyTranspiler]
    //	static IEnumerable<CodeInstruction> TryGenerateWeaponFor_Transpiler(IEnumerable<CodeInstruction> instructions)
    //	{
    //		List<CodeInstruction> ILs = instructions.ToList ();
    //
    //		int index = ILs.FindIndex (IL => IL.opcode == OpCodes.Ldfld && IL.operand.ToStringSafe().Contains("Pawn_EquipmentTracker")) - 1;
    //		
    //		MethodInfo giveChildWeapons = typeof(TranspilerHelper).GetMethod("FilterChildWeapons", AccessTools.all);
    //		var injection = new List<CodeInstruction> {
    //			new CodeInstruction(OpCodes.Ldarg_0),
    //			new CodeInstruction(OpCodes.Ldloc_2),
    //			new CodeInstruction(OpCodes.Callvirt, giveChildWeapons),
    //			new CodeInstruction(OpCodes.Stloc_2),
    //		};
    //		ILs.InsertRange (index, injection);
    //
    //		foreach (CodeInstruction instruction in ILs)
    //			yield return instruction;
    //	}
    //}


    /// <summary>
    /// TODO: Document
    /// Why is this patch necessary?
    /// </summary>
    //[HarmonyPatch(typeof(WorkGiver_Warden_DeliverFood), "FoodAvailableInRoomTo")]
    //public static class WGWDF_FAIRT_Patch
    //{
    //    [HarmonyTranspiler]
    //    static IEnumerable<CodeInstruction> _Transpiler(IEnumerable<CodeInstruction> insts)
    //    {
    //        foreach (CodeInstruction inst in insts)
    //        {
    //            if (inst.opcode == OpCodes.Ldc_R4 && (float)(inst.operand) == 0.5f)
    //                yield return new CodeInstruction(OpCodes.Ldc_R4, 0.01f);
    //            else
    //                yield return inst;
    //        }
    //    }
    //}


}

