﻿using HarmonyLib;
using HugsLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BabiesAndChildren
{
    public class ChildrenBase : ModBase
    {
        public static ChildrenBase Instance { get; private set; }
        public override string ModIdentifier
        {
            get { return "Babies_and_Children"; }
        }


        public static bool ModCSL_ON = false;
        public static bool ModRJW_ON = false;
        public static bool ModWIP_ON = false;

        public ChildrenBase() { Instance = this; }

        public override void MapLoaded(Map map)
        {
            if (map == Current.Game.CurrentMap)
            {
                if (!BnCSettings.OncePerGame)
                {
                    //Log.Message("=============BNC Once Per Game=============");
                    foreach (Map tmap in Current.Game.Maps)
                    {
                        foreach (Pawn pawn in tmap.mapPawns.AllPawnsSpawned)
                        {
                            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
                            {
                                ChildrenUtility.ChangeBodyType(pawn, true, false);
                                ChildrenUtility.ChangeChildBackstory(pawn);
                            }
                        }
                    }
                    BnCSettings.OncePerGame = true;
                }
                if (BnCSettings.GestationPeriodDays_Enable)
                {
                    foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
                    {
                        if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                        {
                            thingDef.race.gestationPeriodDays = (float)BnCSettings.GestationPeriodDays;
               
                        }
                    }
                }
            }
        }
    }

    // Handy for more readable code when age-checking
    public static class AgeStage
    {
        public const int Baby = 0;
        public const int Toddler = 1;
        public const int Child = 2;
        public const int Teenager = 3;
        public const int Adult = 4;
    }

    internal static class TranspilerHelper
    {
        internal static List<ThingStuffPair> FilterChildWeapons(Pawn pawn, List<ThingStuffPair> weapons)
        {
            var weapons_out = new List<ThingStuffPair>();
            if (weapons.Count > 0)
                foreach (ThingStuffPair weapon in weapons)
                {
                    if (weapon.thing.BaseMass < ChildrenUtility.ChildMaxWeaponMass(pawn))
                    {
                        weapons_out.Add(weapon);
                    }
                }
            return weapons_out;
        }

    }


}
