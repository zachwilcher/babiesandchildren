using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BabiesAndChildren.Harmony
{


    //[HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    //public static class PawnGraphicSet_ResolveApparelGraphics_Patch
    //{

    //    [HarmonyTranspiler]
    //    static IEnumerable<CodeInstruction> ResolveApparelGraphics_Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        List<CodeInstruction> ILs = instructions.ToList();
    //        int injectIndex = ILs.FindIndex(ILs.FindIndex(x => x.opcode == OpCodes.Ldloca_S) + 1, x => x.opcode == OpCodes.Ldloca_S) - 2; // Second occurence
    //        ILs.RemoveRange(injectIndex, 2);
    //        MethodInfo childBodyCheck = typeof(Children_Drawing).GetMethod("ModifyChildBodyType");
    //        ILs.Insert(injectIndex, new CodeInstruction(OpCodes.Call, childBodyCheck));

    //        foreach (CodeInstruction IL in ILs)
    //        {
    //            yield return IL;
    //        }
    //    }
    //}



    //for showhair
    //[HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(Boolean), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(Boolean), typeof(Boolean), typeof(Boolean) })]
    //public static class Showhair_Patch
    //{
    //    [HarmonyPostfix]
    //    [HarmonyBefore(new string[] { "AlienRace", "FacialStuff", "ShowHair" })]
    //    [HarmonyPriority(200)]
    //    internal static void RShowhair_Patch_Patchf(ref PawnGraphicSet __instance, ref Vector3 rootLoc, Pawn ___pawn, bool portrait)
    //    {
    //        // Change the root location for showhair
    //        rootLoc = CAlienHarmony.ModifiedHairLoc(rootLoc, __instance.pawn);
    //    }
    //}


    //[HarmonyPatch(typeof(PawnGraphicSet), "HairMatAt", new[] { typeof(Rot4) })]
    //public static class PawnGraphicSet_Patch
    //{
    //    [HarmonyPrefix]
    //    //[HarmonyBefore(new string[] { "AlienRace", "FacialStuff" })]
    //    //[HarmonyPriority(600)]
    //    public static bool HairMatAt_Patch(ref PawnGraphicSet __instance, Rot4 facing, ref Material __result)
    //    {
    //        Material baseMat = __instance.hairGraphic.MatAt(facing, null);
    //        if (__instance.pawn.IsInvisible())
    //        {
    //            baseMat = InvisibilityMatPool.GetInvisibleMat(baseMat);
    //        }
    //        __result = Children_Drawing.ModifyHairForChild(__instance.flasher.GetDamagedMat(baseMat), __instance.pawn) ;
    //        return false;

    //    }
    //}


    public static class Children_Drawing
    {
        // My own methods
        //internal static Graphic GetChildBodyGraphics(PawnGraphicSet graphicSet, Shader shader, Color skinColor)
        //{
        //    Graphic_Multi graphic = null;
        //    string str = "Naked_Boy";
        //    if (graphicSet.pawn.gender == Gender.Female)
        //    {
        //        str = "Naked_Girl";
        //    }
        //    string path = "Things/Pawn/Humanlike/Children/Bodies/" + str;
        //    graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
        //    return graphic;
        //}

        // Injected methods
        //public static BodyTypeDef ModifyChildBodyType(Pawn pawn)
        //{
        //    if (ChildrenUtility.RaceUsesChildren(pawn))
        //    {
        //        if ((ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child) && (pawn.def.defName == "Human"))
        //            return BodyTypeDefOf.Thin;
        //        //can't use harmony to patch enum so far as I can tell so I'll use preexisting
        //        //thin and fat to represent the crawling and upright toddler types until a better
        //        //system can be implemented
        //        if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Toddler)
        //        {
        //            if (ChildrenUtility.ToddlerIsUpright(pawn))
        //            {
        //                return BodyTypeDefOf.Thin;
        //            }
        //            return BodyTypeDefOf.Fat;
        //        }
        //    }
        //    return pawn.story.bodyType;
        //}

        //public static Material ModifyHairForChild(Material mat, Pawn pawn)
        //{            
        //    Material newDamagedMat = mat;
        //    if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child)
        //    {
        //        //float TextureScaleX = 1.2f;
        //        //float TextureScaleY = 1.4f;
        //        //float TextureOffsetX = 0;
        //        //float TextureOffsetY = -0.4f;
        //        //float TextureOffsetEWX = -0.015f;
        //        Material xDamagedMat = new Material(mat);
        //        xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
        //        xDamagedMat.mainTextureScale = new Vector2(Tweakvalue.Hair_TextureScaleX, Tweakvalue.Hair_TextureScaleY);
        //        xDamagedMat.mainTextureOffset = new Vector2(Tweakvalue.Hair_TextureOffsetX, Tweakvalue.Hair_TextureOffsetY);
        //        newDamagedMat = xDamagedMat;
        //    }
        //    return newDamagedMat;

        //}
    }
}

