using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimWorldChildren.Harmony
{
    public class RenderPawnInternal_AlienRace
    {
        [TweakValue("BnC", -2f, 2f)] public static float AHead_Scale = 0.8422f;

        private static Dictionary<float, GraphicMeshSet> humanlikeBodySetModified = new Dictionary<float, GraphicMeshSet>();
        private static Dictionary<float, GraphicMeshSet> humanlikeHeadSetModified = new Dictionary<float, GraphicMeshSet>();
        private static Dictionary<float, GraphicMeshSet> humanlikeHairAverageSetModified = new Dictionary<float, GraphicMeshSet>();
        private static Dictionary<float, GraphicMeshSet> humanlikeHairNarrowSetModified = new Dictionary<float, GraphicMeshSet>();
        private static GraphicMeshSet GetModifiedBodyMeshSet(float bodySizeFactor)
        {
            float t = bodySizeFactor;
            if (!RenderPawnInternal_AlienRace.humanlikeHeadSetModified.ContainsKey(bodySizeFactor))
            {
                RenderPawnInternal_AlienRace.humanlikeBodySetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
            }
            return RenderPawnInternal_AlienRace.humanlikeBodySetModified[bodySizeFactor];
        }

        // Token: 0x060000B2 RID: 178 RVA: 0x0000C4F1 File Offset: 0x0000A6F1
        private static GraphicMeshSet GetModifiedHeadMeshSet(float bodySizeFactor)
        {
            if (!RenderPawnInternal_AlienRace.humanlikeHeadSetModified.ContainsKey(bodySizeFactor))
            {
                RenderPawnInternal_AlienRace.humanlikeHeadSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
            }
            return RenderPawnInternal_AlienRace.humanlikeHeadSetModified[bodySizeFactor];
        }

        // Token: 0x060000B3 RID: 179 RVA: 0x0000C524 File Offset: 0x0000A724
        private static GraphicMeshSet GetModifiedHairMeshSet(float bodySizeFactor, Pawn pawn)
        {
            if (pawn.story.crownType == CrownType.Average)
            {
                if (!RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified.ContainsKey(bodySizeFactor))
                {
                    RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
                }
                return RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified[bodySizeFactor];
            }
            if (pawn.story.crownType == CrownType.Narrow)
            {
                if (!RenderPawnInternal_AlienRace.humanlikeHairNarrowSetModified.ContainsKey(bodySizeFactor))
                {
                    RenderPawnInternal_AlienRace.humanlikeHairNarrowSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.3f * bodySizeFactor, 1.5f * bodySizeFactor));
                }
                return RenderPawnInternal_AlienRace.humanlikeHairNarrowSetModified[bodySizeFactor];
            }
            if (!RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified.ContainsKey(bodySizeFactor))
            {
                RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
            }
            return RenderPawnInternal_AlienRace.humanlikeHairAverageSetModified[bodySizeFactor];
        }

        // Token: 0x060000B4 RID: 180 RVA: 0x0000C5E4 File Offset: 0x0000A7E4
        //public static float GetBodysizeScaling(float bodySizeFactor, Pawn pawn)
        //{
        //    float num = bodySizeFactor;
        //    float num2 = 1f;
        //    try
        //    {
        //        int curLifeStageIndex = pawn.ageTracker.CurLifeStageIndex;
        //        int num3 = pawn.RaceProps.lifeStageAges.Count - 1;
        //        LifeStageAge lifeStageAge = pawn.RaceProps.lifeStageAges[curLifeStageIndex];
        //        if (num3 == curLifeStageIndex && curLifeStageIndex != 0 && bodySizeFactor != 1f)
        //        {
        //            LifeStageAge lifeStageAge2 = pawn.RaceProps.lifeStageAges[curLifeStageIndex - 1];
        //            num = lifeStageAge2.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge.def.bodySizeFactor - lifeStageAge2.def.bodySizeFactor) / (lifeStageAge.minAge - lifeStageAge2.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge2.minAge)), 2);
        //        }
        //        else if (num3 == curLifeStageIndex)
        //        {
        //            num = bodySizeFactor;
        //        }
        //        else if (curLifeStageIndex == 0)
        //        {
        //            LifeStageAge lifeStageAge3 = pawn.RaceProps.lifeStageAges[curLifeStageIndex + 1];
        //            num = lifeStageAge.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge3.def.bodySizeFactor - lifeStageAge.def.bodySizeFactor) / (lifeStageAge3.minAge - lifeStageAge.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge.minAge)), 2);
        //        }
        //        else
        //        {
        //            LifeStageAge lifeStageAge3 = pawn.RaceProps.lifeStageAges[curLifeStageIndex + 1];
        //            num = lifeStageAge.def.bodySizeFactor + (float)Math.Round((double)((lifeStageAge3.def.bodySizeFactor - lifeStageAge.def.bodySizeFactor) / (lifeStageAge3.minAge - lifeStageAge.minAge) * (pawn.ageTracker.AgeBiologicalYearsFloat - lifeStageAge.minAge)), 2);
        //        }
        //        if (pawn.RaceProps.baseBodySize > 0f)
        //        {
        //            num2 = pawn.RaceProps.baseBodySize;
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return num * num2;
        //}

        // Token: 0x060000B5 RID: 181 RVA: 0x0000C7CC File Offset: 0x0000A9CC
        public static void RenderPawnInternalScaled(PawnRenderer __instance, Pawn pawn, float bodySizeFactor, PawnWoundDrawer woundOverlays, PawnHeadOverlays statusOverlays, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            if (!__instance.graphics.AllResolved)
            {
                __instance.graphics.ResolveAllGraphics();
            }
            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
            Mesh mesh = null;
            if (renderBody)
            {
                Vector3 loc = rootLoc;
                loc.y += 0.007575758f;
                if (bodyDrawType == RotDrawMode.Dessicated && !pawn.RaceProps.Humanlike && __instance.graphics.dessicatedGraphic != null && !portrait)
                {
                    __instance.graphics.dessicatedGraphic.Draw(loc, bodyFacing, pawn, angle);
                }
                else
                {
                    mesh = RenderPawnInternal_AlienRace.GetModifiedBodyMeshSet(bodySizeFactor).MeshAt(bodyFacing);
                    List<Material> list = __instance.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Material mat = RenderPawnInternal_AlienRace.OverrideMaterialIfNeeded(__instance, list[i], pawn);
                        GenDraw.DrawMeshNowOrLater(mesh, loc, quaternion, mat, portrait);
                        loc.y += 0.003787879f;
                    }
                    if (bodyDrawType == RotDrawMode.Fresh)
                    {
                        Vector3 drawLoc = rootLoc;
                        drawLoc.y += 0.01893939f;
                        woundOverlays.RenderOverBody(drawLoc, mesh, quaternion, portrait);
                    }
                }
            }
            Vector3 vector = rootLoc;
            Vector3 a = rootLoc;
            if (bodyFacing != Rot4.North)
            {
                a.y += 0.02651515f;
                vector.y += 0.02272727f;
            }
            else
            {
                a.y += 0.02272727f;
                vector.y += 0.02651515f;
            }
            if (__instance.graphics.headGraphic != null)
            {
                Vector3 vector2 = quaternion * __instance.BaseHeadOffsetAt(headFacing);
                vector2.z *= bodySizeFactor;
                vector2.x *= bodySizeFactor;
                Material material = __instance.graphics.HeadMatAt(headFacing, bodyDrawType, headStump);
                if (material != null)
                {
                    GraphicMeshSet gset = RenderPawnInternal_AlienRace.GetModifiedHeadMeshSet(bodySizeFactor);
                    if (pawn.def.defName == "Kurin_Race")
                    {                   
                        gset = new GraphicMeshSet(bodySizeFactor * 1.5f * AHead_Scale);
                    }
                    GenDraw.DrawMeshNowOrLater(gset.MeshAt(headFacing), a + vector2, quaternion, material, portrait);
                }
                Vector3 vector3 = rootLoc + vector2;
                vector3.y += 0.03030303f;
                bool flag = false;
                if (!portrait || !Prefs.HatsOnlyOnMap)
                {
                    Mesh mesh2 = RenderPawnInternal_AlienRace.GetModifiedHairMeshSet(bodySizeFactor, pawn).MeshAt(headFacing);
                    List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
                    for (int j = 0; j < apparelGraphics.Count; j++)
                    {
                        if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead)
                        {
                            if (!apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace)
                            {
                                flag = true;
                                Material mat2 = RenderPawnInternal_AlienRace.OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
                                GenDraw.DrawMeshNowOrLater(mesh2, vector3, quaternion, mat2, portrait);
                            }
                            else
                            {
                                Material mat3 = RenderPawnInternal_AlienRace.OverrideMaterialIfNeeded(__instance, apparelGraphics[j].graphic.MatAt(bodyFacing, null), pawn);
                                Vector3 loc2 = rootLoc + vector2;
                                loc2.y += ((bodyFacing == Rot4.North) ? 0.003787879f : 0.03409091f);
                                GenDraw.DrawMeshNowOrLater(mesh2, loc2, quaternion, mat3, portrait);
                            }
                        }
                    }
                }
                if (!flag && bodyDrawType != RotDrawMode.Dessicated && !headStump)
                {
                    Mesh mesh3 = RenderPawnInternal_AlienRace.GetModifiedHairMeshSet(bodySizeFactor, pawn).MeshAt(headFacing);
                    Material material2 = __instance.graphics.HairMatAt(headFacing);
                    Vector3 loc3 = vector3;
                    Quaternion quat = quaternion;
                    Material mat4 = material2;
                    int num = portrait ? 1 : 0;
                    GenDraw.DrawMeshNowOrLater(mesh3, loc3, quat, mat4, num != 0);
                }
            }
            if (renderBody)
            {
                for (int k = 0; k < __instance.graphics.apparelGraphics.Count; k++)
                {
                    ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[k];
                    if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell)
                    {
                        Material mat5 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                        mat5 = RenderPawnInternal_AlienRace.OverrideMaterialIfNeeded(__instance, apparelGraphicRecord.graphic.MatAt(bodyFacing, null), pawn);
                        GenDraw.DrawMeshNowOrLater(mesh, vector, quaternion, mat5, portrait);
                    }
                }
            }
            if (!portrait)
            {
                Traverse.Create(__instance).Method("DrawEquipment", new object[] { rootLoc }).GetValue();
                if (pawn.apparel != null)
                {
                    List<Apparel> wornApparel = pawn.apparel.WornApparel;
                    for (int l = 0; l < wornApparel.Count; l++)
                    {
                        wornApparel[l].DrawWornExtras();
                    }
                }
                Vector3 bodyLoc = rootLoc;
                bodyLoc.y += 0.04166667f;
                statusOverlays.RenderStatusOverlays(bodyLoc, quaternion, RenderPawnInternal_AlienRace.GetModifiedHeadMeshSet(bodySizeFactor).MeshAt(headFacing));
            }
        }

        // Token: 0x060000B6 RID: 182 RVA: 0x0000CCB7 File Offset: 0x0000AEB7
        private static Material OverrideMaterialIfNeeded(PawnRenderer PRinstance, Material original, Pawn pawn)
        {
            //original = RenderPawnInternal_AlienRace.ModifyAlienClothingForChild(original, pawn, pawn.Rotation);
            return PRinstance.graphics.flasher.GetDamagedMat(pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original);
        }

        //public static Material ModifyAlienClothingForChild(Material damagedMat, Pawn pawn, Rot4 bodyFacing)
        //{
        //    Material xDamagedMat = new Material(damagedMat);
        //    xDamagedMat.GetTexture("_MainTex").wrapMode = TextureWrapMode.Clamp;
        //    xDamagedMat.mainTextureScale = new Vector2(Children_Drawing.AlienClothing_ScaleX, Children_Drawing.AlienClothing_ScaleY);
        //    xDamagedMat.mainTextureOffset = new Vector2(Children_Drawing.AlienClothing_OffsetX, Children_Drawing.AlienClothing_OffsetY);
        //    if (bodyFacing == Rot4.West || bodyFacing == Rot4.East)
        //    {
        //        xDamagedMat.mainTextureOffset = new Vector2(Children_Drawing.AlienClothing_OffsetEWX, Children_Drawing.AlienClothing_OffsetY);
        //    }
        //    return xDamagedMat;
        //}

    }

}

