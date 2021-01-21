using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
            if (pawn.RaceProps.Humanlike)
            {
                Children_Drawing.ResolveAgeGraphics(__instance);
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    _this.ResolveApparelGraphics();
                });
            }
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class PawnGraphicSet_ResolveApparelGraphics_Patch
    {

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ResolveApparelGraphics_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> ILs = instructions.ToList();
            int injectIndex = ILs.FindIndex(ILs.FindIndex(x => x.opcode == OpCodes.Ldloca_S) + 1, x => x.opcode == OpCodes.Ldloca_S) - 2; // Second occurence
            ILs.RemoveRange(injectIndex, 2);
            MethodInfo childBodyCheck = typeof(Children_Drawing).GetMethod("ModifyChildBodyType");
            ILs.Insert(injectIndex, new CodeInstruction(OpCodes.Call, childBodyCheck));

            foreach (CodeInstruction IL in ILs)
            {
                yield return IL;
            }
        }
    }

    /// <summary>
    /// By patching OverrideMaterialIfNeeded we avoid having to patch usage of materialList in other parts of the code.
    /// Disadvantage is that we also patch a few other instances that might not strictly be necessary.
    /// </summary>
    //[HarmonyPatch(typeof(PawnRenderer), "OverrideMaterialIfNeeded")]
    //public static class PawnRenderer_OverrideMaterialIfNeeded_Patch
    //{
    //    [HarmonyPrefix]
    //    public static bool OverrideMaterialIfNeeded_Prefix(ref Material original, Pawn pawn)
    //    {
    //        if (pawn.def.defName == "Human" && ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child)
    //        {
    //            original = Children_Drawing.ModifyHairForChild(original, pawn);
    //        }
    //        //else
    //        //{
    //        //    original = Children_Drawing.ModifyClothingForChild(original, pawn, pawn.Rotation);
    //        //}
    //        return true;
    //    }
    //}


    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(Boolean), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(Boolean), typeof(Boolean), typeof(Boolean) })]
    public static class RenderPawnInternal_Patch
    {
        //private static bool COnlyApplyToColonists = false;
        //private static bool HideAllHats = false;
        //private static bool ShowHatsOnlyWhenDrafted = false;

        //[HarmonyBefore(new string[] { "AlienRace", "FacialStuff" })]
        //[HarmonyPriority(600)]
        //public static bool Prefix(PawnRenderer __instance, ref Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        //{

        //    try
        //    {
        //        Pawn pawn = null;
        //        try
        //        {
        //            pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
        //        }
        //        catch
        //        {
        //        }
        //        if (pawn != null && pawn.RaceProps.Humanlike && pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager)
        //        {

        //            if ((pawn.ageTracker.CurLifeStageIndex < AgeStage.Child) || (pawn.def.defName == "Human"))
        //            {
        //                // Change the root location of the child's draw position
        //                rootLoc.z += Tweakvalue.HumanrootlocZ;
        //                PawnWoundDrawer pawnWoundDrawer = null;
        //                try
        //                {
        //                    pawnWoundDrawer = Traverse.Create(__instance).Field("woundOverlays").GetValue<PawnWoundDrawer>();
        //                }
        //                catch
        //                {
        //                }
        //                if (pawnWoundDrawer != null)
        //                {
        //                    PawnHeadOverlays pawnHeadOverlays = null;
        //                    try
        //                    {
        //                        pawnHeadOverlays = Traverse.Create(__instance).Field("statusOverlays").GetValue<PawnHeadOverlays>();
        //                    }
        //                    catch
        //                    {
        //                    }
        //                    if (pawnHeadOverlays != null)
        //                    {
        //                        //RenderPawnInternal_Patch.RenderPawnInternalHuman(__instance, pawn, pawnWoundDrawer, pawnHeadOverlays, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump, invisible);
        //                        return false;
        //                        //return true;

        //                    }
        //                }

        //            }
        //            else
        //            // to AlienRace
        //            {
        //                rootLoc.z += Tweakvalue.AlienrootlocZ;
        //                //float bodySizeFactor = Tweakvalue.AlienBodySize;
        //                ////rootLoc = Children_Drawing.ModifyChildYPosOffset(rootLoc, pawn, portrait);                                  
        //                //RenderPawnInternal_AlienRace.RenderPawnInternalScaled(__instance, pawn, bodySizeFactor, pawnWoundDrawer, pawnHeadOverlays, rootLoc, angle, renderBody, bodyFacing, headFacing, bodyDrawType, portrait, headStump, invisible);
        //                //return false;
        //                return true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return true;
        //}



        [HarmonyPrefix]
        internal static void RenderPawnInternal_Patchq(ref PawnGraphicSet __instance, ref Vector3 rootLoc, Pawn ___pawn, bool portrait)
        {
            // Change the root location of the child's draw position
            rootLoc = Children_Drawing.ModifyChildYPosOffset(rootLoc, ___pawn, portrait);
        }



        //[HarmonyTranspiler]
        //static IEnumerable<CodeInstruction> RenderPawnInternal_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ILgen)
        //{
        //    List<CodeInstruction> ILs = instructions.ToList();

        //    // Render body for babies and toddlers
        //    // from if (renderBody) to if (pawn.ageTracker.CurLifStageIndex < 2 || renderBody)
        //    int renderBodyIndex = ILs.FindIndex(x => x.opcode == OpCodes.Ldarg_3);
        //    Label babyDrawBodyJump = ILgen.DefineLabel();
        //    ILs[renderBodyIndex + 2].labels = new List<Label> { babyDrawBodyJump }; //Label entrance to the code block
        //    List<CodeInstruction> injection1 = new List<CodeInstruction> {
        //        new CodeInstruction (OpCodes.Ldarg_0),
        //        new CodeInstruction (OpCodes.Ldfld, AccessTools.Field(typeof(PawnRenderer), "pawn")),
        //        new CodeInstruction (OpCodes.Ldfld, AccessTools.Field(typeof(Pawn), "ageTracker")),
        //        new CodeInstruction (OpCodes.Call, typeof(Pawn_AgeTracker).GetProperty("CurLifeStageIndex").GetGetMethod()),
        //        new CodeInstruction (OpCodes.Ldc_I4_2),
        //        new CodeInstruction (OpCodes.Blt, babyDrawBodyJump) //Jump renderBody and enter block
        //    };
        //    ILs.InsertRange(renderBodyIndex, injection1);

        //    // Ensure pawn is a child or higher before drawing head
        //    // from if (this.graphics.headGraphic != null) to if (this.graphics.headGraphic != null && (!RaceUsesChildren(this.pawn) || EnsurePawnIsChildOrOlder(pawn)))
        //    int injectIndex2 = ILs.FindIndex(x => x.opcode == OpCodes.Ldfld && x.operand == AccessTools.Field(typeof(PawnGraphicSet), "headGraphic")) + 2;
        //    Label drawHeadJump = ILgen.DefineLabel();
        //    List<CodeInstruction> injection2 = new List<CodeInstruction> {
        //        new CodeInstruction (OpCodes.Ldarg_0),
        //        new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
        //        new CodeInstruction (OpCodes.Call, typeof(ChildrenUtility).GetMethod("RaceUsesChildren")),
        //        new CodeInstruction (OpCodes.Brfalse, drawHeadJump),
        //        new CodeInstruction (OpCodes.Ldarg_0),
        //        new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
        //        new CodeInstruction (OpCodes.Call, typeof(Children_Drawing).GetMethod("EnsurePawnIsChildOrOlder")),
        //        new CodeInstruction (OpCodes.Brfalse, ILs [injectIndex2 - 1].operand),
        //        new CodeInstruction (OpCodes.Nop){labels = new List<Label>{drawHeadJump}},
        //    };
        //    ILs.InsertRange(injectIndex2, injection2);

        //    // Modify the scale of a hat graphic when worn by a child
        //    int injectIndex3 = ILs.FindIndex(x => x.opcode == OpCodes.Call && x.operand == typeof(GenDraw).GetMethod("DrawMeshNowOrLater", AccessTools.all)) + 4;
        //    List<CodeInstruction> injection3 = new List<CodeInstruction> {
        //        new CodeInstruction (OpCodes.Ldloc_S, 19),
        //        new CodeInstruction (OpCodes.Ldarg_0),
        //        new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
        //        new CodeInstruction (OpCodes.Call, typeof(Children_Drawing).GetMethod("ModifyHatForChild")),
        //        new CodeInstruction (OpCodes.Stloc_S, 19),
        //    };
        //    ILs.InsertRange(injectIndex3, injection3);

        //    // Modify the scale of a hair graphic when drawn on a child
        //    int injectIndex4 = ILs.FindIndex(x => x.opcode == OpCodes.Callvirt && x.operand == AccessTools.Method(typeof(PawnGraphicSet), "HairMatAt")) + 2;
        //    List<CodeInstruction> injection4 = new List<CodeInstruction> {
        //        new CodeInstruction (OpCodes.Ldloc_S, 19),
        //        new CodeInstruction (OpCodes.Ldarg_0),
        //        new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
        //        new CodeInstruction (OpCodes.Call, AccessTools.Method(typeof(Children_Drawing), "ModifyHairForChild")),
        //        new CodeInstruction (OpCodes.Stloc_S, 19),
        //    };
        //    ILs.InsertRange(injectIndex4, injection4);

        //    foreach (CodeInstruction IL in ILs)
        //    {
        //        yield return IL;
        //    }
        //}

        //public static bool CHideHats(Pawn pawn, bool portrait)
        //{

        //    if (ChildrenBase.showhair_On)
        //    {
        //        if (ChildrenBase.only_apply_to_colonists && pawn.Faction != Faction.OfPlayer)
        //        {
        //            return false;
        //        }
        //        if (ChildrenBase.show_only_when_drafted)
        //        {
        //            return !pawn.Drafted;
        //        }
        //        return ChildrenBase.hide_all_hats || (portrait && Prefs.HatsOnlyOnMap);
        //    }
        //    return true;
        //}

        /// <summary>
        /// By patching OverrideMaterialIfNeeded we avoid having to patch usage of materialList in other parts of the code.
        /// Disadvantage is that we also patch a few other instances that might not strictly be necessary.
        /// </summary>
        //private static Material OverrideMaterialIfNeeded(PawnRenderer PRinstance, Material original, Pawn pawn)
        //{
        //    original = Children_Drawing.ModifyClothingForChild(original, pawn, pawn.Rotation);
        //    return PRinstance.graphics.flasher.GetDamagedMat(pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original);
        //}

        //private static Material Hat_OverrideMaterialIfNeeded(PawnRenderer PRinstance, Material original, Pawn pawn)
        //{
        //    original = Children_Drawing.ModifyHatForChild(original, pawn);
        //    return PRinstance.graphics.flasher.GetDamagedMat(pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original);
        //}


        //static void RenderPawnInternalHuman(PawnRenderer __instance, Pawn pawn, PawnWoundDrawer woundOverlays, PawnHeadOverlays statusOverlays, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        //{

        //    if (!__instance.graphics.AllResolved)
        //    {
        //        __instance.graphics.ResolveAllGraphics();
        //    }
        //    Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
        //    Mesh mesh = null;

        //    // from if (renderBody) to if (pawn.ageTracker.CurLifStageIndex < 2 || renderBody)
        //    //if (renderBody)

        //    if (pawn.ageTracker.CurLifeStageIndex < 2 || renderBody)
        //    {
        //        Vector3 loc = rootLoc;
        //        loc.y += 0.007575758f;
        //        if (bodyDrawType == RotDrawMode.Dessicated && !pawn.RaceProps.Humanlike && __instance.graphics.dessicatedGraphic != null && !portrait)
        //        {
        //            __instance.graphics.dessicatedGraphic.Draw(loc, bodyFacing, pawn, angle);
        //        }
        //        else
        //        {

        //            mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);

        //            List<Material> list = __instance.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                Material mat = RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, list[i], pawn);
        //                GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, mat, portrait);
        //                loc.y += 0.003787879f;
        //            }
        //            if (bodyDrawType == RotDrawMode.Fresh)
        //            {
        //                Vector3 drawLoc = rootLoc;
        //                drawLoc.y += 0.0189393945f;
        //                woundOverlays.RenderOverBody(drawLoc, mesh, quaternion, portrait);

        //            }
        //        }
        //    }
        //    Vector3 vector = rootLoc;
        //    Vector3 a = rootLoc;
        //    if (bodyFacing != Rot4.North)
        //    {
        //        a.y += 0.0265151523f;
        //        vector.y += 0.0227272734f;
        //    }
        //    else
        //    {
        //        a.y += 0.0227272734f;
        //        vector.y += 0.0265151523f;
        //    }

        //    // Ensure pawn is a child or higher before drawing head
        //    // from if (this.graphics.headGraphic != null) to if (this.graphics.headGraphic != null && (!RaceUsesChildren(this.pawn) || EnsurePawnIsChildOrOlder(pawn)))
        //    //if (__instance.graphics.headGraphic != null)
        //    if (__instance.graphics.headGraphic != null && (!ChildrenUtility.RaceUsesChildren(pawn) || Children_Drawing.EnsurePawnIsChildOrOlder(pawn)))
        //    {
        //        Vector3 b = quaternion * __instance.BaseHeadOffsetAt(headFacing);
        //        Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
        //        if (material != null)
        //        {
        //            GenDraw.DrawMeshNowOrLater(MeshPool.humanlikeHeadSet.MeshAt(headFacing), a + b, quaternion, material, portrait);
        //        }
        //        Vector3 loc2 = rootLoc + b;
        //        loc2.y += 0.0303030312f;
        //        bool flag = false;

        //        // showhair patch
        //        //bool HideHatsOk = (bool)CCHideHats.Invoke( __instance, new object[] { portrait });

        //        if ((!portrait || !Prefs.HatsOnlyOnMap) && !CHideHats(pawn, portrait))
        //        {
        //            Mesh mesh2 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
        //            List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
        //            for (int j = 0; j < apparelGraphics.Count; j++)
        //            {
        //                if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
        //                {
        //                    if (!apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace)
        //                    {
        //                        flag = true;
        //                        Material material2 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
        //                        material2 = RenderPawnInternal_Patch.Hat_OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
        //                        GenDraw.DrawMeshNowOrLater(mesh2, loc2, quaternion, material2, portrait);
        //                    }
        //                    else
        //                    {
        //                        Material material3 = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
        //                        material3 = RenderPawnInternal_Patch.Hat_OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
        //                        Vector3 loc3 = rootLoc + b;
        //                        loc3.y += ((bodyFacing == Rot4.North) ? 0.003787879f : 0.03409091f);
        //                        GenDraw.DrawMeshNowOrLater(mesh2, loc3, quaternion, material3, portrait);
        //                    }
        //                }
        //            }
        //        }
        //        if (!flag && bodyDrawType != RotDrawMode.Dessicated && !headStump)
        //        {
        //            Mesh mesh3 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
        //            Material mat2 = __instance.graphics.HairMatAt(headFacing);

        //            // Modify the scale of a hair graphic when drawn on a child
        //            mat2 = Children_Drawing.ModifyHairForChild(mat2, pawn);

        //            GenDraw.DrawMeshNowOrLater(mesh3, loc2, quaternion, mat2, portrait);
        //        }
        //    }
        //    if (renderBody)
        //    {
        //        for (int k = 0; k < __instance.graphics.apparelGraphics.Count; k++)
        //        {
        //            ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[k];
        //            if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell)
        //            {
        //                Material material4 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
        //                material4 = RenderPawnInternal_Patch.OverrideMaterialIfNeeded(__instance, apparelGraphicRecord.graphic.MatAt(bodyFacing, null), pawn);
        //                GenDraw.DrawMeshNowOrLater(mesh, vector, quaternion, material4, portrait);
        //            }
        //        }
        //    }
        //    if (!portrait)
        //    {
        //        //__instance.DrawEquipment(rootLoc);
        //        Traverse.Create(__instance).Method("DrawEquipment", new object[] { rootLoc }).GetValue();
        //        if (pawn.apparel != null)
        //        {
        //            List<Apparel> wornApparel = pawn.apparel.WornApparel;
        //            for (int l = 0; l < wornApparel.Count; l++)
        //            {
        //                wornApparel[l].DrawWornExtras();
        //            }
        //        }
        //        Vector3 bodyLoc = rootLoc;
        //        bodyLoc.y += 0.0416666679f;
        //        statusOverlays.RenderStatusOverlays(bodyLoc, quaternion, MeshPool.humanlikeHeadSet.MeshAt(headFacing));
        //    }

        //    //
        //}
    }

    public static class Children_Drawing
    {

        internal static void ResolveAgeGraphics(PawnGraphicSet graphics)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {

                if (!ChildrenUtility.RaceUsesChildren(graphics.pawn))
                {
                    return;
                }

                if (graphics.pawn.story.hairDef != null)
                {
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(graphics.pawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
                }

                // Reroute the graphics for children
                // For babies and toddlers
                if (ChildrenUtility.GetAgeStage(graphics.pawn) <= AgeStage.Baby)
                {
                    //string toddler_hair = "Boyish";
                    //if (graphics.pawn.gender == Gender.Female)
                    //{
                    //    toddler_hair = "Girlish";
                    //}
                    //graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Hairs/Child_" + toddler_hair, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
          
                    // The pawn is a baby
                    if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Baby)
                    {
                        graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Single>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                        graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                        graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                        graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    }
                }

                // The pawn is a toddler
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Toddler)
                {
                    string upright = "";
                    float a = graphics.pawn.def.race.lifeStageAges[1].minAge + ((graphics.pawn.def.race.lifeStageAges[2].minAge - graphics.pawn.def.race.lifeStageAges[1].minAge)/2);            
                    CLog.Message("[3-2]"+ a + "\n" + graphics.pawn.ageTracker.AgeBiologicalYearsFloat);
                    if (graphics.pawn.ageTracker.AgeBiologicalYearsFloat >= 3.25f)
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
                        graphics.nakedGraphic = Children_Drawing.GetChildBodyGraphics(graphics, ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                        graphics.rottingGraphic = Children_Drawing.GetChildBodyGraphics(graphics, ShaderDatabase.CutoutSkin, PawnGraphicSet.RottingColor);                        
                        graphics.headGraphic = Children_Drawing.GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    }
                    if(ChildrenBase.human_like_head_enabled && graphics.pawn.def.defName == "Kurin_Race")
                    {
                        graphics.headGraphic = Children_Drawing.GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
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
        internal static Graphic GetChildBodyGraphics(PawnGraphicSet graphicSet, Shader shader, Color skinColor)
        {
            Graphic_Multi graphic = null;
            string str = "Naked_Boy";
            if (graphicSet.pawn.gender == Gender.Female)
            {
                str = "Naked_Girl";
            }
            string path = "Things/Pawn/Humanlike/Children/Bodies/" + str;
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
            return graphic;
        }

        // Injected methods
        public static BodyTypeDef ModifyChildBodyType(Pawn pawn)
        {
            if (ChildrenUtility.RaceUsesChildren(pawn))
            {
                if ((ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child) && (pawn.def.defName == "Human"))
                    return BodyTypeDefOf.Thin;
                //can't use harmony to patch enum so far as I can tell so I'll use preexisting
                //thin and fat to represent the crawling and upright toddler types until a better
                //system can be implemented
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Toddler)
                {
                    if (pawn.ageTracker.AgeBiologicalYears >= 1)
                    {
                        return BodyTypeDefOf.Fat;
                    }
                    return BodyTypeDefOf.Thin;
                }
            }
            return pawn.story.bodyType;
        }

        public static Vector3 ModifyChildYPosOffset(Vector3 pos, Pawn pawn, bool portrait)
        {
            Vector3 newPos = pos;
            if (ChildrenUtility.RaceUsesChildren(pawn))
            {
                // move the draw target down to compensate for child shortness
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child && !pawn.InBed())
                {
                    if (pawn.def.defName == "Human") newPos.z += ChildrenBase.HumanrootlocZ;
                    else newPos.z += ChildrenBase.AlienrootlocZ;
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
                        if (pawn.def.defName == "Human") newPos.z += ChildrenBase.HumanrootlocZ;
                        else newPos.z += ChildrenBase.AlienrootlocZ;
                        // Are we in a crib?
                        Vector3 vector = new Vector3(0, 0, 0.2f).RotatedBy(bed.Rotation.AsAngle);
                        newPos -= vector;
                    }
                    newPos += new Vector3(0, 0, 0.2f);
                }
            }
            return newPos;
        }


        //public static bool EnsurePawnIsChildOrOlder(Pawn pawn)
        //{
        //    if (ChildrenUtility.GetAgeStage(pawn) >= AgeStage.Child)
        //        return true;
        //    return false;
        //}

        //public static Material ModifyHairForChild(Material mat, Pawn pawn)
        //{
        //    Material newMat = mat;
        //    if (ChildrenUtility.RaceUsesChildren(pawn))
        //    {
        //        newMat.mainTexture.wrapMode = TextureWrapMode.Clamp;
        //        // Scale down the child hair to fit the head
        //        if (ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Child)
        //        {
        //            newMat.mainTextureScale = new Vector2(1.13f, 1.13f);
        //            float benis = 0;
        //            if (!pawn.Rotation.IsHorizontal)
        //            {
        //                benis = -0.015f;
        //            }
        //            newMat.mainTextureOffset = new Vector2(-0.045f + benis, -0.045f);
        //        }
        //        // Scale down the toddler hair to fit the head
        //        if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Toddler)
        //        {
        //            newMat.mainTextureOffset = new Vector2(-0.07f, 0.12f);
        //        }
        //    }
        //    return newMat;

        //}

        //public static Material ModifyClothingForChild(Material damagedMat, Pawn pawn, Rot4 bodyFacing)
        //{
        //    if (damagedMat != null)
        //    {
        //        if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child && pawn.RaceProps.Humanlike)
        //        {
        //            Material xDamagedMat = new Material(damagedMat);
        //            xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
        //            xDamagedMat.mainTextureScale = new Vector2(Tweakvalue.Clothing_TextureScaleX, Tweakvalue.Clothing_TextureScaleY);
        //            xDamagedMat.mainTextureOffset = new Vector2(Tweakvalue.Clothing_TextureOffsetX, Tweakvalue.Clothing_TextureOffsetY);
        //            if (bodyFacing == Rot4.West || bodyFacing == Rot4.East)
        //            {
        //                //xDamagedMat.mainTextureOffset = new Vector2(Clothing_TextureOffsetEWX, Clothing_TextureOffsetY);
        //            }

        //            return xDamagedMat;
        //        }
        //    }
        //    return null;

        //}

        //public static Material ModifyHatForChild(Material mat, Pawn pawn)
        //{
        //    if (mat != null)
        //    {
        //        if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child && pawn.RaceProps.Humanlike)
        //        {
        //            Material xDamagedMat = new Material(mat);
        //            xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
        //            xDamagedMat.mainTextureScale = new Vector2(Tweakvalue.Hat_TextureScaleX, Tweakvalue.Hat_TextureScaleY);
        //            xDamagedMat.mainTextureOffset = new Vector2(Tweakvalue.Hat_TextureOffsetX, Tweakvalue.Hat_TextureOffsetY);
        //            return xDamagedMat;
        //        }
        //    }
        //    return null;
        //}
    }
}

