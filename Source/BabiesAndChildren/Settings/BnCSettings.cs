﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;

namespace BabiesAndChildren
{
    public class BnCSettings : ModSettings
    {
        public const int MAX_TRAIT_COUNT = 3;
        public const double STILLBORN_CHANCE = 0.09;
        public const double GET_NEW_TYPE_CHANCE = 0.08;
        public const double GET_SPECIFIC_SEXUALITY = 0.08;
        public const double GET_GAY_SEXUALITY = 0.05;
        public const int MAX_ACCELRATED_GROWTH_FACTOR = 20;

        public static bool accelerated_growth = true;
        public static int accelerated_growth_end_age = 14;
        public static int baby_accelerated_growth = 7;
        public static int toddler_accelerated_growth = 7;
        public static int child_accelerated_growth = 7;
        public static int teenager_accelerated_growth = 1;

        public enum BabyInheritPercentageHandleEnum
        {
            _None,
            _25,
            _50,
            _75,
            _90,
            _Random,
        }
        public static BabyInheritPercentageHandleEnum baby_Inherit_percentage = BabyInheritPercentageHandleEnum._50;

        public static float option_child_max_weapon_mass = 2.0f;
        public static bool GestationPeriodDays_Enable = false;
        public static float GestationPeriodDays = 45f;
        public static bool enable_postpartum = true;
        public static float cryVolume = 0.8f;

        public static bool debug_and_gsetting = false;
        public static bool child_cute_act_enabled = true;
        public static bool human_like_head_enabled = true;
        public static bool Rabbie_Child_head_enabled = false;
        public static float HumanBodySize = 0.713f;
        public static float HumanHeadSize = 0.922f;
        public static float HumanHairSize = 0.730f;
        public static float HumanrootlocZ = -0.11f;
        public static float AlienBodySize = 0.842f;
        public static float AlienHeadSizeA = 0.767f;
        public static float AlienHeadSizeB = 0.767f;
        public static float AlienHairSize = 0.832f;
        public static float AlienrootlocZ = -0.038f;
        public static float ShowHairSize = 1f;
        public static float ShowHairLocY = -0.003f;
        public static float ShowHairHumanLocZ = 0.092f;
        public static float ShowHairAlienLocZ = 0.035f;
        public static float ShowHairAlienHFLocZ = 0.048f;
        public static bool OncePerGame = false;

        private static Vector2 scrollPosition;
        private static float height_modifier = 300f;

        public static void DoWindowContents(Rect inRect)
        {           
            //30f for top page description and bottom close button
            Rect outRect = new Rect(0f, 30f, inRect.width, inRect.height - 30f);
            //-16 for slider, height_modifier - additional height for options
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.maxOneColumn = true;
            listingStandard.ColumnWidth = viewRect.width / 2.05f;
            listingStandard.BeginScrollView(outRect, ref scrollPosition, ref viewRect);
            listingStandard.Begin(viewRect);
            listingStandard.Gap(4f);
            if (listingStandard.ButtonText("BnCSetting_Init".Translate()))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);              
                Reset_Settings();
            }
            listingStandard.Gap(5f);
            listingStandard.CheckboxLabeled("AcceleratedGrowth_title".Translate(), ref accelerated_growth, "AcceleratedGrowth_desc".Translate());
            if (accelerated_growth)
            {
                listingStandard.Gap(3f);
                listingStandard.Label("AcceleratedGrowthEndAge_title1".Translate() + ": " + accelerated_growth_end_age, -1f, "AcceleratedGrowthEndAge_desc1".Translate());
                accelerated_growth_end_age = (int)listingStandard.Slider(accelerated_growth_end_age, 0, 18);
                listingStandard.Gap(3f);
                listingStandard.Label("BabyAcceleratedGrowth_title".Translate() + ": " + baby_accelerated_growth, -1f, "BabyAcceleratedGrowth_desc".Translate());
                baby_accelerated_growth = (int)listingStandard.Slider(baby_accelerated_growth, 1, MAX_ACCELRATED_GROWTH_FACTOR);
                listingStandard.Gap(3f);
                listingStandard.Label("ToddlerAcceleratedGrowth_title".Translate() + ": " + toddler_accelerated_growth, -1f, "ToddlerAcceleratedGrowth_desc".Translate());
                toddler_accelerated_growth = (int)listingStandard.Slider(toddler_accelerated_growth, 1, MAX_ACCELRATED_GROWTH_FACTOR);
                listingStandard.Gap(3f);
                listingStandard.Label("ChildAcceleratedGrowth_title".Translate() + ": " + child_accelerated_growth, -1f, "ChildAcceleratedGrowth_desc".Translate());
                child_accelerated_growth = (int)listingStandard.Slider(child_accelerated_growth, 1, MAX_ACCELRATED_GROWTH_FACTOR);
                listingStandard.Gap(3f);
                listingStandard.Label("TeenagerAcceleratedGrowth_title".Translate() + ": " + teenager_accelerated_growth, -1f, "TeenagerAcceleratedGrowth_desc".Translate());
                teenager_accelerated_growth = (int)listingStandard.Slider(teenager_accelerated_growth, 1, MAX_ACCELRATED_GROWTH_FACTOR);
            }
            //else
            //{
            //    accelerated_growth_end_age = 14;
            //    baby_accelerated_growth = 7;
            //    toddler_accelerated_growth = 7;
            //    child_accelerated_growth = 7;
            //    teenager_accelerated_growth = 1;
            //}

            listingStandard.Gap(10f);
            listingStandard.Label("BabyInheritPercentage_Title".Translate() + ": ", -1f, "BabyInheritPercentage_Desc".Translate());
            listingStandard.Gap(5f);
            string t = "BabyInheritPercentage_Desc" + baby_Inherit_percentage;
            if (listingStandard.ButtonText(t.Translate()))
            {
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                    {
                      new FloatMenuOption("BabyInheritPercentage_Desc_None".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._None)),
                      new FloatMenuOption("BabyInheritPercentage_Desc_25".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._25)),
                      new FloatMenuOption("BabyInheritPercentage_Desc_50".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._50)),
                      new FloatMenuOption("BabyInheritPercentage_Desc_75".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._75)),
                      new FloatMenuOption("BabyInheritPercentage_Desc_90".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._90)),
                      new FloatMenuOption("BabyInheritPercentage_Desc_Random".Translate(), (() => baby_Inherit_percentage = BabyInheritPercentageHandleEnum._Random)),
                    }));
            }


            listingStandard.Gap(5f);
            listingStandard.Label("OptionChildMaxWeaponMass".Translate() + ": " + Math.Round(option_child_max_weapon_mass, 1) + "Kg", -1f, "OptionChildMaxWeaponMass_desc".Translate());
            option_child_max_weapon_mass = listingStandard.Slider(option_child_max_weapon_mass, 0f, 5f);

            listingStandard.Gap(5f);
            listingStandard.CheckboxLabeled("GestationPeriodDays_Enable".Translate(), ref GestationPeriodDays_Enable, "GestationPeriodDays_Enable_Desc".Translate());  
            if (GestationPeriodDays_Enable)
            {
                listingStandard.Gap(3f);
                listingStandard.Label("GestationPeriodDays_Title".Translate() + ": " + Math.Round(GestationPeriodDays, 0) + "GestationPeriodDays_Days".Translate(), -1f, "GestationPeriodDays_desc".Translate());
                GestationPeriodDays = listingStandard.Slider(GestationPeriodDays, 1f, 50f);
            }
            listingStandard.Gap(5f);
            listingStandard.CheckboxLabeled("EnablePostPartum_title".Translate(), ref enable_postpartum, "EnablePostPartum_desc".Translate());
            listingStandard.Gap(5f);
            listingStandard.Label("CryVolumeSetting_title".Translate() + ": " + Math.Round(cryVolume * 100, 0) + "%", -1f, "CryVolumeSetting_desc".Translate());
            cryVolume = listingStandard.Slider(cryVolume, 0f, 1f);

            ////////////////////////////
            listingStandard.NewColumn();
            GUI.contentColor = Color.white;

            listingStandard.Gap(4f);            
            listingStandard.CheckboxLabeled("ChildCuteActEnabled_Title".Translate(), ref child_cute_act_enabled, "ChildCuteActEnabled_desc".Translate());
            listingStandard.Gap(5f);
            listingStandard.CheckboxLabeled("DebugInfoMessagesEnabled_Title".Translate(), ref debug_and_gsetting, "DebugInfoMessagesEnabled_desc".Translate());
            if (debug_and_gsetting)
            {                
                listingStandard.Gap(5f);
                listingStandard.CheckboxLabeled("humanlikeHeadEnabled_Title".Translate(), ref human_like_head_enabled, "humanlikeHeadEnabled_desc".Translate());
                listingStandard.Gap(5f);
                listingStandard.CheckboxLabeled("RabbieChildHeadEnabled_Title".Translate(), ref Rabbie_Child_head_enabled, "RabbieChildHeadEnabled_desc".Translate());
                listingStandard.Gap(5f);
                listingStandard.Label("HumanChildrenBodysize_Title".Translate() + ": " + Math.Round(HumanBodySize, 4), -1f, "HumanChildrenBodysize_desc".Translate());
                HumanBodySize = listingStandard.Slider(HumanBodySize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("HumanChildrenHeadsize_Title".Translate() + ": " + Math.Round(HumanHeadSize, 4), -1f, "HumanChildrenHeadsize_desc".Translate());
                HumanHeadSize = listingStandard.Slider(HumanHeadSize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("HumanChildrenHairsize_Title".Translate() + ": " + Math.Round(HumanHairSize, 4), -1f, "HumanChildrenHairsize_desc".Translate());
                HumanHairSize = listingStandard.Slider(HumanHairSize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("HumanChildrenZroc_Title".Translate() + ": " + Math.Round(HumanrootlocZ, 4), -1f, "HumanChildrenZroc_desc".Translate());
                HumanrootlocZ = listingStandard.Slider(HumanrootlocZ, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("AlienChildrenBodysize_Title".Translate() + ": " + Math.Round(AlienBodySize, 4), -1f, "AlienChildrenBodysize_desc".Translate());
                AlienBodySize = listingStandard.Slider(AlienBodySize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("AlienChildrenOriginalHeadsize_Title".Translate() + ": " + Math.Round(AlienHeadSizeA, 4), -1f, "AlienChildrenOriginalHeadsize_desc".Translate());
                AlienHeadSizeA = listingStandard.Slider(AlienHeadSizeA, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("AlienChildrenHumanlikeHeadsize_Title".Translate() + ": " + Math.Round(AlienHeadSizeB, 4), -1f, "AlienChildrenHumanlikeHeadsize_desc".Translate());
                AlienHeadSizeB = listingStandard.Slider(AlienHeadSizeB, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("AlienChildrenHairsize_Title".Translate() + ": " + Math.Round(AlienHairSize, 4), -1f, "AlienChildrenHairsize_desc".Translate());
                AlienHairSize = listingStandard.Slider(AlienHairSize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("AlienChildrenZroc_Title".Translate() + ": " + Math.Round(AlienrootlocZ, 4), -1f, "AlienChildrenZroc_desc".Translate());
                AlienrootlocZ = listingStandard.Slider(AlienrootlocZ, -2f, 2f);

                listingStandard.Gap(5f);
                listingStandard.Label("ShowHairSize_Title".Translate() + ": " + Math.Round(ShowHairSize, 4), -1f, "ShowHairSize_desc".Translate());
                ShowHairSize = listingStandard.Slider(ShowHairSize, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("ShowHairLocY_Title".Translate() + ": " + Math.Round(ShowHairLocY, 4), -1f, "ShowHairLocY_desc".Translate());
                ShowHairLocY = listingStandard.Slider(ShowHairLocY, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("ShowHairHumanZLoc_Title".Translate() + ": " + Math.Round(ShowHairHumanLocZ, 4), -1f, "ShowHairHumanZLoc_desc".Translate());
                ShowHairHumanLocZ = listingStandard.Slider(ShowHairHumanLocZ, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("ShowHairAlienZLoc_Title".Translate() + ": " + Math.Round(ShowHairAlienLocZ, 4), -1f, "ShowHairAlienZLoc_desc".Translate());
                ShowHairAlienLocZ = listingStandard.Slider(ShowHairAlienLocZ, -2f, 2f);
                listingStandard.Gap(5f);
                listingStandard.Label("ShowHairAlienHFLocZ_Title".Translate() + ": " + Math.Round(ShowHairAlienHFLocZ, 4), -1f, "ShowHairAlienHFLocZ_desc".Translate());
                ShowHairAlienHFLocZ = listingStandard.Slider(ShowHairAlienHFLocZ, -2f, 2f);
            }

            listingStandard.EndScrollView(ref viewRect);
            listingStandard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref accelerated_growth, "accelerated_growth");
            Scribe_Values.Look(ref accelerated_growth_end_age, "accelerated_growth_end_age");
            Scribe_Values.Look(ref baby_accelerated_growth, "baby_accelerated_growth");
            Scribe_Values.Look(ref toddler_accelerated_growth, "toddler_accelerated_growth");
            Scribe_Values.Look(ref child_accelerated_growth, "child_accelerated_growth");
            Scribe_Values.Look(ref teenager_accelerated_growth, "teenager_accelerated_growth");

            Scribe_Values.Look(ref baby_Inherit_percentage, "baby_Inherit_percentage");
            Scribe_Values.Look(ref option_child_max_weapon_mass, "option_child_max_weapon_mass");
            Scribe_Values.Look(ref GestationPeriodDays_Enable, "GestationPeriodDays_Enable");
            Scribe_Values.Look(ref GestationPeriodDays, "GestationPeriodDays");
            Scribe_Values.Look(ref enable_postpartum, "enable_postpartum");
            Scribe_Values.Look(ref cryVolume, "cryVolume");

            Scribe_Values.Look(ref debug_and_gsetting, "debug_and_gsetting");
            Scribe_Values.Look(ref child_cute_act_enabled, "child_cute_act_enabled");
            Scribe_Values.Look(ref human_like_head_enabled, "human_like_head_enabled");
            Scribe_Values.Look(ref Rabbie_Child_head_enabled, "Rabbie_Child_head_enabled");
            Scribe_Values.Look(ref HumanBodySize, "HumanBodySize");
            Scribe_Values.Look(ref HumanHeadSize, "HumanHeadSize");
            Scribe_Values.Look(ref HumanHairSize, "HumanHairSize");
            Scribe_Values.Look(ref HumanrootlocZ, "HumanrootlocZ");
            Scribe_Values.Look(ref AlienBodySize, "AlienBodySize");
            Scribe_Values.Look(ref AlienHeadSizeA, "AlienHeadSizeA");
            Scribe_Values.Look(ref AlienHeadSizeB, "AlienHeadSizeB");
            Scribe_Values.Look(ref AlienHairSize, "AlienHairSize");
            Scribe_Values.Look(ref AlienrootlocZ, "AlienrootlocZ");
            Scribe_Values.Look(ref ShowHairSize, "ShowHairSize");
            Scribe_Values.Look(ref ShowHairLocY, "ShowHairLocY");
            Scribe_Values.Look(ref ShowHairHumanLocZ, "ShowHairHumanLocZ");
            Scribe_Values.Look(ref ShowHairAlienLocZ, "ShowHairAlienLocZ");
            Scribe_Values.Look(ref ShowHairAlienHFLocZ, "ShowHairAlienHFLocZ");
            Scribe_Values.Look(ref OncePerGame, "OncePerGame");
        }

        public static void Reset_Settings()
        {
            accelerated_growth = true;
            accelerated_growth_end_age = 14;
            baby_accelerated_growth = 7;
            toddler_accelerated_growth = 7;
            child_accelerated_growth = 7;
            teenager_accelerated_growth = 1;
            baby_Inherit_percentage = BabyInheritPercentageHandleEnum._50;
            option_child_max_weapon_mass = 2.0f;
            GestationPeriodDays_Enable = false;
            GestationPeriodDays = 45f;
            enable_postpartum = true;
            cryVolume = 0.8f;
            debug_and_gsetting = false;
            child_cute_act_enabled = true;
            human_like_head_enabled = true;
            Rabbie_Child_head_enabled = false;
            HumanBodySize = 0.713f;
            HumanHeadSize = 0.922f;
            HumanHairSize = 0.730f;
            HumanrootlocZ = -0.11f;
            AlienBodySize = 0.842f;
            AlienHeadSizeA = 0.767f;
            AlienHeadSizeB = 0.767f;
            AlienHairSize = 0.832f;
            AlienrootlocZ = -0.038f;
            ShowHairSize = 1f;
            ShowHairLocY = -0.003f;
            ShowHairHumanLocZ = 0.092f;
            ShowHairAlienLocZ = 0.035f;
            ShowHairAlienHFLocZ = 0.048f;
            OncePerGame = false;
        }
    }
}
