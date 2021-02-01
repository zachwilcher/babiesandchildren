﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlienRace;
using BabiesAndChildren.Tools;
using HarmonyLib;
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
        static MethodInfo meshInfo = AccessTools.Method(AccessTools.TypeByName("MeshMakerPlanes"), "NewPlaneMesh", new[]
            { typeof(Vector2), typeof(bool), typeof(bool), typeof(bool) }, null);


        static GraphicMeshSet GetModifiedBodyMeshSet(float bodySizeFactor, Pawn pawn)
        {
            if (!humanlikeBodySetModified.ContainsKey(bodySizeFactor))
            {
                humanlikeBodySetModified.Add(bodySizeFactor, new GraphicMeshSet(1.5f * bodySizeFactor));
            }
            return humanlikeBodySetModified[bodySizeFactor];
        }

        static GraphicMeshSet GetModifiedHeadMeshSet(float headSizeFactor, Pawn pawn)
        {
            if (!humanlikeHeadSetModified.ContainsKey(headSizeFactor))
            {
                humanlikeHeadSetModified.Add(headSizeFactor, new GraphicMeshSet(1.5f * headSizeFactor));
            }
            return humanlikeHeadSetModified[headSizeFactor];
        }

        static GraphicMeshSet GetModifiedHairMeshSet(float hairSizeFactor, Pawn pawn)
        {
            GraphicMeshSet result;
            if (pawn.story.crownType == CrownType.Average)
            {
                if (!humanlikeHairAverageSetModified.ContainsKey(hairSizeFactor))
                {
                    humanlikeHairAverageSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.5f * hairSizeFactor));
                }
                result = humanlikeHairAverageSetModified[hairSizeFactor];
            }
            else
            {
                if (pawn.story.crownType == CrownType.Narrow)
                {
                    if (!humanlikeHairNarrowSetModified.ContainsKey(hairSizeFactor))
                    {
                        humanlikeHairNarrowSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.3f * hairSizeFactor, 1.5f * hairSizeFactor));
                    }
                    result = humanlikeHairNarrowSetModified[hairSizeFactor];
                }
                else
                {
                    if (!humanlikeHairAverageSetModified.ContainsKey(hairSizeFactor))
                    {
                        humanlikeHairAverageSetModified.Add(hairSizeFactor, new GraphicMeshSet(1.5f * hairSizeFactor));
                    }
                    result = humanlikeHairAverageSetModified[hairSizeFactor];
                }
            }
            return result;
        }

        [HarmonyPatch(typeof(HarmonyPatches), "GetPawnMesh")]
        static class GetPawnMesh_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Rot4 facing, bool wantsBody, ref Mesh __result)
            {
                try
                {
                    if (pawn != null && RaceUtility.PawnUsesChildren(pawn) && AgeStage.IsYoungerThan(pawn, AgeStage.Teenager))
                    {
                        float bodySizeFactor = ChildrenUtility.GetBodySize(pawn);
                        float headSizeFactor = ChildrenUtility.GetHeadSize(pawn);
                        if (pawn.GetComp<AlienPartGenerator.AlienComp>() != null)
                        {
                            if (portrait)
                            {
                                if (wantsBody)
                                {
                                    __result = GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                                }
                                else
                                {
                                    __result = GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                                }
                            }
                            else if (wantsBody)
                            {
                                __result = GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                            }
                            else
                            {
                                __result = GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                            }
                        }
                        else if (wantsBody)
                        {
                            __result = GetModifiedBodyMeshSet(bodySizeFactor, pawn).MeshAt(facing);
                        }
                        else
                        {
                            __result = GetModifiedHeadMeshSet(headSizeFactor, pawn).MeshAt(facing);
                        }
                        return false;
                    }
                }
                catch
                {
                    // Ignore
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(HarmonyPatches), "GetPawnHairMesh")]
        static class GetPawnHairMesh_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Rot4 headFacing, PawnGraphicSet graphics, ref Mesh __result)
            {
                try
                {
                    if (pawn != null && RaceUtility.PawnUsesChildren(pawn) && AgeStage.IsYoungerThan(pawn, AgeStage.Teenager))
                    {
                        //float bodySizeFactor = GetBodySize(pawn);
                        float hairSizeFactor = ChildrenUtility.GetHairSize(0, pawn);
                        if (pawn.GetComp<AlienPartGenerator.AlienComp>() != null)
                        {

                            if (pawn.story.crownType == CrownType.Narrow)
                            {
                                if (portrait)
                                {
                                    __result = GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                                }
                                else
                                {
                                    __result = GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                                }
                            }
                            else if (portrait)
                            {
                                __result = GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                            }
                            else
                            {
                                __result = GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                            }
                        }
                        else
                        {
                            __result = GetModifiedHairMeshSet(hairSizeFactor, pawn).MeshAt(headFacing);
                        }
                        return false;
                    }
                }
                catch
                {
                    // Ignore
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(HarmonyPatches), "DrawAddons")] 
        static class DrawAddons_Patch
        {
            static Dictionary<Vector2, Mesh> addonMeshs = new Dictionary<Vector2, Mesh>();
            static Dictionary<Vector2, Mesh> addonMeshsFlipped = new Dictionary<Vector2, Mesh>();

            [HarmonyPrefix]
            static bool Prefix(bool portrait, Pawn pawn, Vector3 vector, Quaternion quat, Rot4 rotation, bool invisible)
            {
                try
                {
                    if (!(pawn.def is ThingDef_AlienRace alienProps) || 
                        invisible || 
                        !RaceUtility.PawnUsesChildren(pawn) || 
                        !AgeStage.IsAgeStage(pawn, AgeStage.Child)) 
                        return false;
                    //only for children not babies and toddlers
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
                        if (BnCSettings.human_like_head_enabled && ChildrenUtility.HasHumanlikeHead(pawn) && ba.bodyPart.Contains("Head")) continue;

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

                        if (!addonMeshsFlipped.ContainsKey(ba.drawSize * bodySizeFactor))
                        {
                            addonMeshsFlipped.Add(ba.drawSize * bodySizeFactor, (Mesh)meshInfo.Invoke(null, new object[]
                                {new Vector2(bodySizeFactor * 1.5f, bodySizeFactor * 1.5f),true,false,false}));
                        }

                        if (!addonMeshs.ContainsKey(ba.drawSize * bodySizeFactor))
                        {
                            addonMeshs.Add(ba.drawSize * bodySizeFactor, (Mesh)meshInfo.Invoke(null, new object[]
                                {new Vector2(bodySizeFactor * 1.5f, bodySizeFactor * 1.5f),false,false,false}));
                        }

                        Mesh mesh = IsEastWestFlipped ? addonMeshsFlipped[ba.drawSize * bodySizeFactor] : addonMeshs[ba.drawSize * bodySizeFactor];

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

                        GenDraw.DrawMeshNowOrLater(mesh, vector + offsetVector.RotatedBy(Mathf.Acos(Quaternion.Dot(Quaternion.identity, quat)) * 2f * 57.29578f), Quaternion.AngleAxis(baAngle, Vector3.up) * quat, alienComp.addonGraphics[i].MatAt(rotation, null), portrait);

                    }
                    return false;
                }
                catch
                {
                    // Ignored
                }
                return true;
            }
        }
    }
}