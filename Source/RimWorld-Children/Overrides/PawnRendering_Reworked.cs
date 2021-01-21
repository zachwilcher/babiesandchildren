using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RimWorldChildren
{

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public static class PawnGraphicSet_ResolveAllGraphics_Patch
    {
        [HarmonyPostfix]
        internal static void ResolveAllGraphics_Patch(ref PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            PawnGraphicSet _this = __instance;
            if (ChildrenUtility.RaceUsesChildren(pawn))
            {
                Children_Drawing.ResolveAgeGraphics(__instance);
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    _this.ResolveApparelGraphics();
                });
            }
        }
    }

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

    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(Boolean), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(Boolean), typeof(Boolean), typeof(Boolean) })]
    public static class RenderPawnInternal_Patch
    {
        [HarmonyPrefix]
        //[HarmonyBefore(new string[] { "AlienRace", "FacialStuff" })]
        //[HarmonyPriority(600)]
        [HarmonyAfter(new string[] { "rimworld.Nals.FacialAnimation" })]
        internal static void RenderPawnInternal_Patchq(ref PawnGraphicSet __instance, ref Vector3 rootLoc, ref Pawn ___pawn, bool portrait)
        {
            if (ChildrenUtility.RaceUsesChildren(___pawn) && ChildrenUtility.GetAgeStage(___pawn) < AgeStage.Teenager)
            {
                // Change the root location of the child's draw position
                rootLoc = Children_Drawing.ModifyChildYPosOffset(rootLoc, ___pawn, portrait);

                // for WIP
                if (ChildrenBase.ModWIP_ON)
                {
                    ThingComp WIPcomp = ChildrenUtility.GetCompByClassName(___pawn, "FacialAnimation.DrawFaceGraphicsComp");
                    if (WIPcomp != null) ___pawn.AllComps.Remove(WIPcomp);
                }
            }

            // The rest of the child Pawn renderering is done by alienrace mod.
        }
    }

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
        internal static void ResolveAgeGraphics(PawnGraphicSet graphics)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (!ChildrenUtility.RaceUsesChildren(graphics.pawn)) return;

                if (graphics.pawn.story.hairDef != null)
                {
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(graphics.pawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
                }
                if (ChildrenUtility.GetAgeStage(graphics.pawn) > AgeStage.Child) return;                

                // The pawn is a baby
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Baby)
                {
                    graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Single>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                    graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                }

                // The pawn is a toddler
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Toddler)
                {
                    string upright = "";
                    if (ChildrenUtility.ToddlerIsUpright(graphics.pawn))
                    {
                        upright = "Upright";
                    }
                    graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                    graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);                    
                }
                // The pawn is a child
                else if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Child)
                {
                    if (graphics.pawn.def.defName == "Human")
                    {               
                        graphics.headGraphic = Children_Drawing.GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    }
                    else if (BnCSettings.human_like_head_enabled && ChildrenUtility.HumanFaceRaces(graphics.pawn))
                    {
                        graphics.headGraphic = Children_Drawing.GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    }
                    else if (BnCSettings.Rabbie_Child_head_enabled && graphics.pawn.def.defName == "Rabbie")
                    {
                        graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Heads/RabbieChild", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    }
                }
            });
        }

        // My own methods
        internal static Graphic GetChildHeadGraphics(Shader shader, Color skinColor)
        {
            Graphic_Multi graphic = null;
            string str = "Male_Child";
            string path = "Things/Pawn/Humanlike/Children/Heads/" + str;
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
            return graphic;
        }
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

        public static Vector3 ModifyChildYPosOffset(Vector3 pos, Pawn pawn, bool portrait)
        {
            Vector3 newPos = pos;

            // move the draw target down to compensate for child shortness
            if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child && !pawn.InBed())
            {
                if (pawn.def.defName == "Human") newPos.z += BnCSettings.HumanrootlocZ;
                else newPos.z += BnCSettings.AlienrootlocZ;
                //newPos.z -= 0.15f;            
            }
            if (pawn.InBed() && !portrait && pawn.CurrentBed().def.size.z == 1)
            {
                Building_Bed bed = pawn.CurrentBed();
                // Babies and toddlers get drawn further down along the bed
                if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child)
                {
                    Vector3 vector = new Vector3(0, 0, 0.5f).RotatedBy(bed.Rotation.AsAngle);
                    newPos -= vector;
                    // ... as do children, but to a lesser extent
                }
                else if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child)
                {
                    if (pawn.def.defName == "Human") newPos.z += BnCSettings.HumanrootlocZ;
                    else newPos.z += BnCSettings.AlienrootlocZ;
                    // Are we in a crib?
                    Vector3 vector = new Vector3(0, 0, 0.2f).RotatedBy(bed.Rotation.AsAngle);
                    newPos -= vector;
                }
                newPos += new Vector3(0, 0, 0.2f);

            }
            return newPos;
        }

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

