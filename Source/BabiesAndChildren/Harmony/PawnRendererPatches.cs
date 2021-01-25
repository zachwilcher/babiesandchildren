using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch(typeof(PawnRenderer), "CarryWeaponOpenly")]
    internal static class PawnRenderer_CarryWeaponOpenly_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref PawnGraphicSet __instance, ref bool __result)
        {
            if (ChildrenUtility.SetMakerTagCheck(__instance.pawn.equipment.Primary, "Toy"))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[]
    {
        typeof(Vector3),
        typeof(float),
        typeof(Boolean),
        typeof(Rot4),
        typeof(Rot4),
        typeof(RotDrawMode),
        typeof(Boolean),
        typeof(Boolean),
        typeof(Boolean)
    })]
    public static class PawnRenderer_RenderPawnInternal_Patch
    {
        [HarmonyPrefix]
        //TODO determine if the following annotation is necessary 
        [HarmonyBefore(new string[] {"rimworld.Nals.FacialAnimation"})]
        internal static void Prefix(ref PawnGraphicSet __instance, ref Vector3 rootLoc,
            ref Pawn ___pawn, bool portrait)
        {
            if (ChildrenUtility.RaceUsesChildren(___pawn))
            {
                if (ChildrenUtility.GetAgeStage(___pawn) < AgeStage.Teenager)
                {
                    // Change the root location of the child's draw position
                    rootLoc = GraphicTools.ModifyChildYPosOffset(rootLoc, ___pawn, portrait);
                }

                if (ChildrenUtility.GetAgeStage(___pawn) < AgeStage.Child)
                    // For facial animation (WIP)
                    if (ChildrenBase.ModWIP_ON)
                    {
                        ThingComp WIPcomp =
                            ChildrenUtility.GetCompByClassName(___pawn, "FacialAnimation.DrawFaceGraphicsComp");
                        if (WIPcomp != null)
                        {
                            ___pawn.AllComps.Remove(WIPcomp);
                        }
                    }
            }

            // The rest of the child Pawn renderering is done by alienrace mod.
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt")]
    [HarmonyAfter(new string[] { "AlienRace" })]
    public static class PawnRenderer_BaseHeadOffsetAt_Patch
    {
        [HarmonyPostfix]
        public static void BaseHeadOffsetAtPostfix_Post(PawnRenderer __instance, Rot4 rotation, ref Vector3 __result, ref Pawn ___pawn)
        {
            try
            {
                Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

                if (pawn != null && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child)
                {

                    if (ChildrenUtility.RaceUsesChildren(pawn))
                    {
                        float bodySizeFactor = ChildrenUtility.GetBodySize(pawn);
                        float num2 = 1f;
                        float num3 = 1f;

                        if (pawn.def.defName == "Alien_Orassan")
                        {
                            num2 = 1.4f;
                            num3 = 1.4f;
                        }
                        __result.z *= bodySizeFactor * num2;
                        __result.x *= bodySizeFactor * num3;
                        if (pawn.def.defName == "Human")
                        {
                            __result.z += Tweaks.HuHeadlocZ ; 
                        }
                        if (ChildrenBase.ModWIP_ON)
                        {
                            __result += GraphicTools.ModifyChildYPosOffset(Vector3.zero, ___pawn, false);
                        }

                    }
                }
            }
            catch
            {
            }
        }
    }
}