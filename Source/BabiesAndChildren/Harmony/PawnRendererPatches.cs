using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch]
    public static class PawnRendererPatches
    {
        
        
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
    public static class RenderPawnInternal
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
        
    //A Facial Animation (WIP) patch causes children heads to be too far above their bodies
    //this patch fixes that
    [HarmonyPatch(typeof(PawnRenderer), "BaseHeadOffsetAt", new[] {typeof(Rot4)})]
    public static class BaseHeadOffsetAt
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Vector3 __result, ref Pawn ___pawn)
        {
            if (ChildrenBase.ModWIP_ON)
            {
                __result += GraphicTools.ModifyChildYPosOffset(Vector3.zero, ___pawn, false);
            }
        }
    }
        
    }
}