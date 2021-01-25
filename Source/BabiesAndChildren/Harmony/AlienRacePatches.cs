using AlienRace;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch] 
    internal static class AlienRacePatches
    {
        static Dictionary<float, GraphicMeshSet> humanlikeBodySetModified = new Dictionary<float, GraphicMeshSet>();
        static Dictionary<float, GraphicMeshSet> humanlikeHeadSetModified = new Dictionary<float, GraphicMeshSet>();
        static Dictionary<float, GraphicMeshSet> humanlikeHairAverageSetModified = new Dictionary<float, GraphicMeshSet>();
        static Dictionary<float, GraphicMeshSet> humanlikeHairNarrowSetModified = new Dictionary<float, GraphicMeshSet>();
        static MethodInfo meshInfo = AccessTools.Method(AccessTools.TypeByName("MeshMakerPlanes"), "NewPlaneMesh", new Type[]
            { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool) }, null);


        static GraphicMeshSet GetModifiedBodyMeshSet(float bodySizeFactor, Pawn pawn)
        {
            if (!AlienRacePatches.humanlikeBodySetModified.ContainsKey(bodySizeFactor))
            {
                AlienRacePatches.humanlikeBodySetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
            }
            return AlienRacePatches.humanlikeBodySetModified[bodySizeFactor];
        }

        static GraphicMeshSet GetModifiedHeadMeshSet(float headSizeFactor, Pawn pawn)
        {
            if (!AlienRacePatches.humanlikeHeadSetModified.ContainsKey(headSizeFactor))
            {
                AlienRacePatches.humanlikeHeadSetModified.Add(headSizeFactor, new GraphicMeshSet(1.5f * headSizeFactor));
            }
            return AlienRacePatches.humanlikeHeadSetModified[headSizeFactor];
        }

        static GraphicMeshSet GetModifiedHairMeshSet(float hairSizeFactor, Pawn pawn)
        {
            GraphicMeshSet result;
            if (pawn.story.crownType == CrownType.Average)
            {
                if (!AlienRacePatches.humanlikeHairAverageSetModified.ContainsKey(hairSizeFactor))
                {
                    AlienRacePatches.humanlikeHairAverageSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.5f * hairSizeFactor));
                }
                result = AlienRacePatches.humanlikeHairAverageSetModified[hairSizeFactor];
            }
            else
            {
                if (pawn.story.crownType == CrownType.Narrow)
                {
                    if (!AlienRacePatches.humanlikeHairNarrowSetModified.ContainsKey(hairSizeFactor))
                    {
                        AlienRacePatches.humanlikeHairNarrowSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.3f * hairSizeFactor, 1.5f * hairSizeFactor));
                    }
                    result = AlienRacePatches.humanlikeHairNarrowSetModified[hairSizeFactor];
                }
                else
                {
                    if (!AlienRacePatches.humanlikeHairAverageSetModified.ContainsKey(hairSizeFactor))
                    {
                        AlienRacePatches.humanlikeHairAverageSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.5f * hairSizeFactor));
                    }
                    result = AlienRacePatches.humanlikeHairAverageSetModified[hairSizeFactor];
                }
            }
            return result;
        }

        [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "GetPawnMesh")]
        static class GetPawnMesh_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Rot4 facing, bool wantsBody, ref Mesh __result)
            {
                try
                {
                    if (pawn != null && ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager)
                    {
                        float bodySizeFactor = ChildrenUtility.GetBodySize(pawn);
                        float headSizeFactor = ChildrenUtility.GetHeadSize(pawn);
                        if (pawn.GetComp<AlienPartGenerator.AlienComp>() != null)
                        {
                            if (portrait)
                            {
                                if (wantsBody)
                                {
                                    __result = AlienRacePatches.GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                                }
                                else
                                {
                                    __result = AlienRacePatches.GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                                }
                            }
                            else if (wantsBody)
                            {
                                __result = AlienRacePatches.GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                            }
                            else
                            {
                                __result = AlienRacePatches.GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                            }
                        }
                        else if (wantsBody)
                        {
                            __result = AlienRacePatches.GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                        }
                        else
                        {
                            __result = AlienRacePatches.GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                        }
                        return false;
                    }
                }
                catch
                {
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "GetPawnHairMesh")]
        static class GetPawnHairMesh_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Rot4 headFacing, PawnGraphicSet graphics, ref Mesh __result)
            {
                try
                {
                    if (pawn != null && ChildrenUtility.RaceUsesChildren(pawn) && pawn.ageTracker.CurLifeStageIndex < AgeStage.Teenager)
                    {
                        //float bodySizeFactor = GetBodySize(pawn);
                        float hairSizeFactor = ChildrenUtility.GetHairSize(0, pawn);
                        if (pawn.GetComp<AlienPartGenerator.AlienComp>() != null)
                        {

                            if (pawn.story.crownType == CrownType.Narrow)
                            {
                                if (portrait)
                                {
                                    __result = AlienRacePatches.GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                                }
                                else
                                {
                                    __result = AlienRacePatches.GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                                }
                            }
                            else if (portrait)
                            {
                                __result = AlienRacePatches.GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                            }
                            else
                            {
                                __result = AlienRacePatches.GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                            }
                        }
                        else
                        {
                            __result = AlienRacePatches.GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                        }
                        return false;
                    }
                }
                catch
                {
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(AlienRace.HarmonyPatches), "DrawAddons")] 
        static class DrawAddons_Patch
        {
            static Dictionary<Vector2, Mesh> addonMeshs = new Dictionary<Vector2, Mesh>();
            static Dictionary<Vector2, Mesh> addonMeshsFlipped = new Dictionary<Vector2, Mesh>();

            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Vector3 vector, Quaternion quat, Rot4 rotation, bool invisible)
            {
                try
                {
                    if (!(pawn.def is ThingDef_AlienRace alienProps) || invisible) return false;
                    if (pawn.ageTracker.CurLifeStageIndex <= AgeStage.Child)
                    {
                        if (pawn.ageTracker.CurLifeStageIndex < AgeStage.Child) return false;
                        //if (pawn.def.defName != "Human")
                        //{
                        float bodySizeFactor = ChildrenUtility.GetBodySize(pawn);
                        float moffsetZfb = 1f;
                        float moffsetXfb = 1f;
                        float moffsetZfa = 1f;
                        float moffsetXfa = 1f;
                        bool IsEastWestFlipped = false;

                        if (pawn.def.defName == "Alien_Orassan")
                        {
                            moffsetZfb = 1.4f;
                            moffsetXfb = 0.1f;
                            moffsetZfa = 1.4f;
                            moffsetXfa = 1.4f;

                            if (rotation == Rot4.West)
                            {
                                IsEastWestFlipped = true;
                            }
                        }
                        else
                        {
                            if (pawn.kindDef.race.ToString().ToLower().Contains("lizardman"))
                            {
                                if (rotation == Rot4.East)
                                {
                                    IsEastWestFlipped = true;
                                }
                            }
                            else
                            {
                                if (rotation == Rot4.West)
                                {
                                    IsEastWestFlipped = true;
                                }
                            }
                        }

                        List<AlienPartGenerator.BodyAddon> addons = alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons;
                        AlienPartGenerator.AlienComp alienComp = pawn.GetComp<AlienPartGenerator.AlienComp>();

                        for (int i = 0; i < addons.Count; i++)
                        {
                            AlienPartGenerator.BodyAddon ba = addons[index: i];
                            if (!ba.CanDrawAddon(pawn: pawn)) continue;
                            if (BnCSettings.human_like_head_enabled && ChildrenUtility.HumanFaceRaces(pawn) && ba.bodyPart.Contains("Head")) continue;

                            AlienPartGenerator.RotationOffset offset = rotation == Rot4.South ?
                                ba.offsets.south :
                                rotation == Rot4.North ?
                                    ba.offsets.north :
                                    rotation == Rot4.East ?
                                        ba.offsets.east :
                                        ba.offsets.west;

                            Vector2 bodyOffset = (portrait ? offset?.portraitBodyTypes ?? offset?.bodyTypes : offset?.bodyTypes)?.FirstOrDefault(predicate: to => to.bodyType == pawn.story.bodyType)
                                ?.offset ?? Vector2.zero;
                            Vector2 crownOffset = (portrait ? offset?.portraitCrownTypes ?? offset?.crownTypes : offset?.crownTypes)?.FirstOrDefault(predicate: to => to.crownType == alienComp.crownType)
                                ?.offset ?? Vector2.zero;

                            //Defaults for tails 
                            //south 0.42f, -0.3f, -0.22f
                            //north     0f,  0.3f, -0.55f
                            //east -0.42f, -0.3f, -0.22f   

                            float moffsetX = 0.42f;
                            float moffsetZ = -0.22f;
                            float moffsetY = ba.inFrontOfBody ? 0.3f + ba.layerOffset : -0.3f - ba.layerOffset;
                            float baAngle = ba.angle;

                            if (!AlienRacePatches.DrawAddons_Patch.addonMeshsFlipped.ContainsKey(ba.drawSize * bodySizeFactor))
                            {
                                AlienRacePatches.DrawAddons_Patch.addonMeshsFlipped.Add(ba.drawSize * bodySizeFactor, (Mesh)AlienRacePatches.meshInfo.Invoke(null, new object[]
                                    {new Vector2(bodySizeFactor * 1.5f, bodySizeFactor * 1.5f),true,false,false}));
                            }

                            if (!AlienRacePatches.DrawAddons_Patch.addonMeshs.ContainsKey(ba.drawSize * bodySizeFactor))
                            {
                                AlienRacePatches.DrawAddons_Patch.addonMeshs.Add(ba.drawSize * bodySizeFactor, (Mesh)AlienRacePatches.meshInfo.Invoke(null, new object[]
                                    {new Vector2(bodySizeFactor * 1.5f, bodySizeFactor * 1.5f),false,false,false}));
                            }

                            Mesh mesh;
                            if (IsEastWestFlipped)
                            {
                                mesh = AlienRacePatches.DrawAddons_Patch.addonMeshsFlipped[ba.drawSize * bodySizeFactor];
                            }
                            else
                            {
                                mesh = AlienRacePatches.DrawAddons_Patch.addonMeshs[ba.drawSize * bodySizeFactor];
                            }


                            if (rotation == Rot4.North)
                            {
                                moffsetX = 0f;
                                if (ba.layerInvert)
                                    moffsetY = -moffsetY;
                                moffsetZ = -0.55f;
                                baAngle = 0;
                            }

                            moffsetX += bodyOffset.x + crownOffset.x;
                            moffsetZ += bodyOffset.y + crownOffset.y;

                            if (ba.bodyPart.Contains("tail"))
                            {
                                moffsetX *= bodySizeFactor * moffsetXfa;
                                moffsetZ *= bodySizeFactor * moffsetZfa;
                            }
                            else
                            {
                                moffsetX *= bodySizeFactor * moffsetXfb;
                                moffsetZ *= bodySizeFactor * moffsetZfb;
                            }

                            if (rotation == Rot4.East)
                            {
                                moffsetX = -moffsetX;
                                baAngle = -baAngle; //Angle
                            }

                            Vector3 offsetVector = new Vector3(x: moffsetX, y: moffsetY, z: (moffsetZ  * Tweaks.G_offsetfac) + Tweaks.G_offset);

                            //                                                                                        Angle calculation to not pick the shortest, taken from Quaternion.Angle and modified
                            //GenDraw.DrawMeshNowOrLater(mesh: alienComp.addonGraphics[index: i].MeshAt(rot: rotation), loc: vector + offsetVector.RotatedBy(angle: Mathf.Acos(f: Quaternion.Dot(a: Quaternion.identity, b: quat)) * 2f * 57.29578f),
                            //    quat: Quaternion.AngleAxis(angle: baAngle, axis: Vector3.up) * quat, mat: alienComp.addonGraphics[index: i].MatAt(rot: rotation), drawNow: portrait);

                            GenDraw.DrawMeshNowOrLater(mesh, vector + offsetVector.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 2f * 57.29578f), Quaternion.AngleAxis(baAngle, Vector3.up) * quat, alienComp.addonGraphics[i].MatAt(rotation, null), portrait);

                        }
                        return false;
                    }
                }
                catch
                {
                }
                return true;
            }
        }
    }
}