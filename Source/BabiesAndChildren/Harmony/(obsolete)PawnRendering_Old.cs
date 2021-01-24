using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWorldChildren.Harmony {

   

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public static class PawnGraphicSet_ResolveAllGraphics_Patch {
        [HarmonyPostfix]
        internal static void ResolveAllGraphics_Patch(ref PawnGraphicSet __instance) {
            Pawn pawn = __instance.pawn;
            PawnGraphicSet _this = __instance;
            if (pawn.RaceProps.Humanlike) {
                Children_Drawing.ResolveAgeGraphics(__instance);
                LongEventHandler.ExecuteWhenFinished(delegate {
                    _this.ResolveApparelGraphics();
                });
            }
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveApparelGraphics")]
    public static class PawnGraphicSet_ResolveApparelGraphics_Patch {
        
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> ResolveApparelGraphics_Transpiler(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> ILs = instructions.ToList();
            int injectIndex = ILs.FindIndex(ILs.FindIndex(x => x.opcode == OpCodes.Ldloca_S) + 1, x => x.opcode == OpCodes.Ldloca_S) - 2; // Second occurence
            ILs.RemoveRange(injectIndex, 2);
            MethodInfo childBodyCheck = typeof(Children_Drawing).GetMethod("ModifyChildBodyType");
            ILs.Insert(injectIndex, new CodeInstruction(OpCodes.Call, childBodyCheck));

            foreach (CodeInstruction IL in ILs) {
                yield return IL;
            }
        }
    }

    /// <summary>
    /// By patching OverrideMaterialIfNeeded we avoid having to patch usage of materialList in other parts of the code.
    /// Disadvantage is that we also patch a few other instances that might not strictly be necessary.
    /// </summary>
    [HarmonyPatch(typeof(PawnRenderer), "OverrideMaterialIfNeeded")]
    public static class PawnRenderer_OverrideMaterialIfNeeded_Patch {
        [HarmonyPrefix]
        public static bool OverrideMaterialIfNeeded_Prefix(ref Material original, Pawn pawn) {
            original = Children_Drawing.ModifyClothingForChild(original, pawn, pawn.Rotation);
            return true;
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new[] { typeof(Vector3), typeof(float), typeof(Boolean), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(Boolean), typeof(Boolean), typeof(Boolean) })]
    [HarmonyBefore(new string[] { "rimworld.erdelf.alien_race.main" })]
    public static class PawnRenderer_RenderPawnInternal_Patch {
        /// <summary>
        /// Adjust Y position for child pawns at the start of the method so we don't have to do
        /// transpiler patching.
        /// </summary>
        [HarmonyPrefix]
        internal static void RenderPawnInternal_Patch(ref PawnGraphicSet __instance, ref Vector3 rootLoc, Pawn ___pawn, bool portrait) {
            // Change the root location of the child's draw position
            rootLoc = Children_Drawing.ModifyChildYPosOffset(rootLoc, ___pawn, portrait);
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> RenderPawnInternal_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ILgen) {
            List<CodeInstruction> ILs = instructions.ToList();

            // Render body for babies and toddlers
            // from if (renderBody) to if (pawn.ageTracker.CurLifStageIndex < 2 || renderBody)
            int renderBodyIndex = ILs.FindIndex(x => x.opcode == OpCodes.Ldarg_3);
            Label babyDrawBodyJump = ILgen.DefineLabel();
            ILs[renderBodyIndex + 2].labels = new List<Label> { babyDrawBodyJump }; //Label entrance to the code block
            List<CodeInstruction> injection1 = new List<CodeInstruction> {
                new CodeInstruction (OpCodes.Ldarg_0),
                new CodeInstruction (OpCodes.Ldfld, AccessTools.Field(typeof(PawnRenderer), "pawn")),
                new CodeInstruction (OpCodes.Ldfld, AccessTools.Field(typeof(Pawn), "ageTracker")),
                new CodeInstruction (OpCodes.Call, typeof(Pawn_AgeTracker).GetProperty("CurLifeStageIndex").GetGetMethod()),
                new CodeInstruction (OpCodes.Ldc_I4_2),
                new CodeInstruction (OpCodes.Blt, babyDrawBodyJump) //Jump renderBody and enter block
            };
            ILs.InsertRange(renderBodyIndex, injection1);

            // Ensure pawn is a child or higher before drawing head
            // from if (this.graphics.headGraphic != null) to if (this.graphics.headGraphic != null && (!RaceUsesChildren(this.pawn) || EnsurePawnIsChildOrOlder(pawn)))
            int injectIndex2 = ILs.FindIndex(x => x.opcode == OpCodes.Ldfld && x.operand == AccessTools.Field(typeof(PawnGraphicSet), "headGraphic")) + 2;
            Label drawHeadJump = ILgen.DefineLabel();
            List<CodeInstruction> injection2 = new List<CodeInstruction> {
                new CodeInstruction (OpCodes.Ldarg_0),
                new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
                new CodeInstruction (OpCodes.Call, typeof(ChildrenUtility).GetMethod("RaceUsesChildren")),
                new CodeInstruction (OpCodes.Brfalse, drawHeadJump),
                new CodeInstruction (OpCodes.Ldarg_0),
                new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
                new CodeInstruction (OpCodes.Call, typeof(Children_Drawing).GetMethod("EnsurePawnIsChildOrOlder")),
                new CodeInstruction (OpCodes.Brfalse, ILs [injectIndex2 - 1].operand),
                new CodeInstruction (OpCodes.Nop){labels = new List<Label>{drawHeadJump}},
            };
            ILs.InsertRange(injectIndex2, injection2);

            // Modify the scale of a hat graphic when worn by a child
            int injectIndex3 = ILs.FindIndex(x => x.opcode == OpCodes.Call && x.operand == typeof(GenDraw).GetMethod("DrawMeshNowOrLater", AccessTools.all)) + 4;
            List<CodeInstruction> injection3 = new List<CodeInstruction> {
                new CodeInstruction (OpCodes.Ldloc_S, 19),
                new CodeInstruction (OpCodes.Ldarg_0),
                new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
                new CodeInstruction (OpCodes.Call, typeof(Children_Drawing).GetMethod("ModifyHatForChild")),
                new CodeInstruction (OpCodes.Stloc_S, 19),
            };
            ILs.InsertRange(injectIndex3, injection3);

            // Modify the scale of a hair graphic when drawn on a child
            int injectIndex4 = ILs.FindIndex(x => x.opcode == OpCodes.Callvirt && x.operand == AccessTools.Method(typeof(PawnGraphicSet), "HairMatAt")) + 2;
            List<CodeInstruction> injection4 = new List<CodeInstruction> {
                new CodeInstruction (OpCodes.Ldloc_S, 19),
                new CodeInstruction (OpCodes.Ldarg_0),
                new CodeInstruction (OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", AccessTools.all)),
                new CodeInstruction (OpCodes.Call, AccessTools.Method(typeof(Children_Drawing), "ModifyHairForChild")),
                new CodeInstruction (OpCodes.Stloc_S, 19),
            };
            ILs.InsertRange(injectIndex4, injection4);
            
            foreach (CodeInstruction IL in ILs) {
                yield return IL;
            }
        }
    }

    public static class Children_Drawing
    {
        [TweakValue("BnC", -2f, 2f)] private static float Clothing_TextureScaleX = 1f;
        [TweakValue("BnC", -2f, 2f)] private static float Clothing_TextureScaleY = 1.2356f;
        [TweakValue("BnC", -2f, 2f)] private static float Clothing_TextureOffsetX = 0.00643f;
        [TweakValue("BnC", -2f, 2f)] private static float Clothing_TextureOffsetY = -0.1219f;
        [TweakValue("BnC", -2f, 2f)] private static float Clothing_TextureOffsetEWX = -0.010f;
        [TweakValue("BnC", -2f, 2f)] private static float Hat_TextureScaleX = 1.251f;
        [TweakValue("BnC", -2f, 2f)] private static float Hat_TextureScaleY = 1.4f;
        [TweakValue("BnC", -2f, 2f)] private static float Hat_TextureOffsetX = -0.115f;
        [TweakValue("BnC", -2f, 2f)] private static float Hat_TextureOffsetY = -0.142f;

        internal static void ResolveAgeGraphics(PawnGraphicSet graphics) {
            LongEventHandler.ExecuteWhenFinished(delegate {

                if (!ChildrenUtility.RaceUsesChildren(graphics.pawn)) {
                    return;
                }
                
                if (graphics.pawn.story.hairDef != null) {
                        graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>(graphics.pawn.story.hairDef.texPath, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
                }

                // Reroute the graphics for children
                // For babies and toddlers
                if (ChildrenUtility.GetAgeStage(graphics.pawn) <= AgeStage.Baby) {
                    string toddler_hair = "Boyish";
                    if (graphics.pawn.gender == Gender.Female) {
                        toddler_hair = "Girlish";
                    }
                    graphics.hairGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Hairs/Child_" + toddler_hair, ShaderDatabase.Cutout, Vector2.one, graphics.pawn.story.hairColor);
                    graphics.headGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/null", ShaderDatabase.Cutout, Vector2.one, Color.white);
                    // The pawn is a baby
                    if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Baby) {
                        graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Single>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                        graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Newborn", ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                    }
                }

                // The pawn is a toddler
                if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Toddler) {
                    string upright = "";
                    if (graphics.pawn.ageTracker.AgeBiologicalYears >= 1) {
                        upright = "Upright";
                    }
                    graphics.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = GraphicDatabase.Get<Graphic_Multi>("Things/Pawn/Humanlike/Children/Bodies/Toddler" + upright, ShaderDatabase.CutoutSkin, Vector2.one, PawnGraphicSet.RottingColor);
                }
                // The pawn is a child
                else if (ChildrenUtility.GetAgeStage(graphics.pawn) == AgeStage.Child) {
                    graphics.nakedGraphic = Children_Drawing.GetChildBodyGraphics(graphics, ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                    graphics.rottingGraphic = Children_Drawing.GetChildBodyGraphics(graphics, ShaderDatabase.CutoutSkin, PawnGraphicSet.RottingColor);
                    graphics.headGraphic = Children_Drawing.GetChildHeadGraphics(ShaderDatabase.CutoutSkin, graphics.pawn.story.SkinColor);
                }
            });
        }

        // My own methods
        internal static Graphic GetChildHeadGraphics(Shader shader, Color skinColor) {
            Graphic_Multi graphic = null;
            string str = "Male_Child";
            string path = "Things/Pawn/Humanlike/Children/Heads/" + str;
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
            return graphic;
        }
        internal static Graphic GetChildBodyGraphics(PawnGraphicSet graphicSet, Shader shader, Color skinColor) {
            Graphic_Multi graphic = null;
            string str = "Naked_Boy";
            if (graphicSet.pawn.gender == Gender.Female) {
                str = "Naked_Girl";
            }
            string path = "Things/Pawn/Humanlike/Children/Bodies/" + str;
            graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, Vector2.one, skinColor) as Graphic_Multi;
            return graphic;
        }

        // Injected methods
        public static BodyTypeDef ModifyChildBodyType(Pawn pawn) {
            if (ChildrenUtility.RaceUsesChildren(pawn)) {
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child)
                    return BodyTypeDefOf.Thin;
                //can't use harmony to patch enum so far as I can tell so I'll use preexisting
                //thin and fat to represent the crawling and upright toddler types until a better
                //system can be implemented
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Toddler) {
                    if (pawn.ageTracker.AgeBiologicalYears >= 1) {
                        return BodyTypeDefOf.Fat;
                    }
                    return BodyTypeDefOf.Thin;
                }
            }
            return pawn.story.bodyType;
        }

        public static Vector3 ModifyChildYPosOffset(Vector3 pos, Pawn pawn, bool portrait) {
            Vector3 newPos = pos;
            if (ChildrenUtility.RaceUsesChildren(pawn)) {
                // move the draw target down to compensate for child shortness
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child && !pawn.InBed()) {
                    newPos.z -= 0.15f;
                }
                if (pawn.InBed() && !portrait && pawn.CurrentBed().def.size.z == 1) {
                    Building_Bed bed = pawn.CurrentBed();
                    // Babies and toddlers get drawn further down along the bed
                    if (ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child) {
                        Vector3 vector = new Vector3(0, 0, 0.5f).RotatedBy(bed.Rotation.AsAngle);
                        newPos -= vector;
                        // ... as do children, but to a lesser extent
                    }
                    else if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child) { // Are we in a crib?
                        Vector3 vector = new Vector3(0, 0, 0.2f).RotatedBy(bed.Rotation.AsAngle);
                        newPos -= vector;
                    }
                    newPos += new Vector3(0, 0, 0.2f);
                }
            }
            return newPos;
        }

        public static Material ModifyHairForChild(Material mat, Pawn pawn) {
            Material newMat = mat;
            if (ChildrenUtility.RaceUsesChildren(pawn)) {
                newMat.mainTexture.wrapMode = TextureWrapMode.Clamp;
                // Scale down the child hair to fit the head
                if (ChildrenUtility.GetAgeStage(pawn) <= AgeStage.Child) {
                    newMat.mainTextureScale = new Vector2(1.13f, 1.13f);
                    float benis = 0;
                    if (!pawn.Rotation.IsHorizontal) {
                        benis = -0.015f;
                    }
                    newMat.mainTextureOffset = new Vector2(-0.045f + benis, -0.045f);
                }
                // Scale down the toddler hair to fit the head
                if (ChildrenUtility.GetAgeStage(pawn) == AgeStage.Toddler) {
                    newMat.mainTextureOffset = new Vector2(-0.07f, 0.12f);
                }
            }
            return newMat;

        }
        public static bool EnsurePawnIsChildOrOlder(Pawn pawn)
        {
            if (ChildrenUtility.GetAgeStage(pawn) >= AgeStage.Child)
                return true;
            return false;
        }

        //public static Material ModifyClothingForChild(Material damagedMat, Pawn pawn, Rot4 bodyFacing) {
        //    Material newDamagedMat = damagedMat;
        //    if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child && pawn.RaceProps.Humanlike) {
        //        float TextureScaleX = 1;
        //        float TextureScaleY = 1.3f;
        //        float TextureOffsetX = 0;
        //        float TextureOffsetY = -0.2f;
        //        float TextureOffsetEWX = -0.015f;
        //        Material xDamagedMat = new Material(damagedMat);
        //        xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
        //        xDamagedMat.mainTextureScale = new Vector2(TextureScaleX, TextureScaleY);
        //        xDamagedMat.mainTextureOffset = new Vector2(TextureOffsetX, TextureOffsetY);
        //        if (bodyFacing == Rot4.West || bodyFacing == Rot4.East) {
        //            xDamagedMat.mainTextureOffset = new Vector2(TextureOffsetEWX, TextureOffsetY);
        //        }

        //        newDamagedMat = xDamagedMat;
        //    }
        //    return newDamagedMat;
        //}
        public static Material ModifyClothingForChild(Material damagedMat, Pawn pawn, Rot4 bodyFacing)
        {
            Material newDamagedMat = damagedMat;
            if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child && pawn.RaceProps.Humanlike)
            {
                //float TextureScaleX = 1;
                //float TextureScaleY = 1.3f;
                //float TextureOffsetX = 0;
                //float TextureOffsetY = -0.2f;
                //float TextureOffsetEWX = -0.015f;
                Material xDamagedMat = new Material(damagedMat);
                xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
                xDamagedMat.mainTextureScale = new Vector2(Clothing_TextureScaleX, Clothing_TextureScaleY);
                xDamagedMat.mainTextureOffset = new Vector2(Clothing_TextureOffsetX, Clothing_TextureOffsetY);
                if (bodyFacing == Rot4.West || bodyFacing == Rot4.East)
                {
                    xDamagedMat.mainTextureOffset = new Vector2(Clothing_TextureOffsetEWX, Clothing_TextureOffsetY);
                }

                newDamagedMat = xDamagedMat;
            }
            return newDamagedMat;
        }

        //public static Material ModifyHatForChild(Material mat, Pawn pawn)
        //{
        //    if (mat == null)
        //        return null;
        //    Material newMat = mat;
        //    if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) == AgeStage.Child)
        //    {
        //        newMat.mainTexture.wrapMode = TextureWrapMode.Clamp;
        //        newMat.mainTextureOffset = new Vector2(0, 0.018f);
        //    }
        //    return newMat;
        //}

        public static Material ModifyHatForChild(Material mat, Pawn pawn)
        {

            if (mat == null)
                return null;

            Material newDamagedMat = mat;
            if (ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex == AgeStage.Child && pawn.RaceProps.Humanlike)
            {
                //float TextureScaleX = 1.2f;
                //float TextureScaleY = 1.4f;
                //float TextureOffsetX = 0;
                //float TextureOffsetY = -0.4f;
                //float TextureOffsetEWX = -0.015f;
                Material xDamagedMat = new Material(mat);
                xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
                xDamagedMat.mainTextureScale = new Vector2(Hat_TextureScaleX, Hat_TextureScaleY);
                xDamagedMat.mainTextureOffset = new Vector2(Hat_TextureOffsetX, Hat_TextureOffsetY);
                newDamagedMat = xDamagedMat;
            }
            return newDamagedMat;

        }


    }
}

