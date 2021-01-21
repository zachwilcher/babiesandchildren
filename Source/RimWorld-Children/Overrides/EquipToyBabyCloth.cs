using HarmonyLib;
using RimWorld;
using Verse;


namespace RimWorldChildren
{
    [HarmonyPatch(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string) }
    , new[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out })]
    internal static class AdultCantEquipToy
    {
        [HarmonyPrefix]
        public static bool CanEquip_patch(ref bool __result, Thing thing, Pawn pawn, out string cantReason)
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

    [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly")]
    internal static class ShowDollAlways
    {
        [HarmonyPrefix]
        public static bool CarryWeaponOpenly_Patch(ref PawnGraphicSet __instance, ref bool __result)
        {

            if (ChildrenUtility.SetMakerTagCheck(__instance.pawn.equipment.Primary, "Toy"))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

}

