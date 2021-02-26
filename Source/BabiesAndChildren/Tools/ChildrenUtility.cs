﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using BabiesAndChildren.api;
using BabiesAndChildren.Tools;
using UnityEngine;
using Verse;
using Verse.AI;
using HealthUtility = BabiesAndChildren.Tools.HealthUtility;
using LifeStageUtility = BabiesAndChildren.Tools.LifeStageUtility;
using StatDefOf = RimWorld.StatDefOf;

namespace BabiesAndChildren
{
    public static class ChildrenUtility
    {
        private static List<ThingDef> bedDefsBestToWorst_CribRestEffectiveness;

        /// <summary>
        /// A list of all bed defs ranked from best to worst prioritizing
        /// cribs over ordinary beds.
        /// </summary>
        public static List<ThingDef> AllBedDefBestToWorstCribRest
        {
            get
            {
                if (bedDefsBestToWorst_CribRestEffectiveness != null)
                    return bedDefsBestToWorst_CribRestEffectiveness;
                
                bedDefsBestToWorst_CribRestEffectiveness = DefDatabase<ThingDef>.AllDefs.Where(def => def.IsBed).
                    OrderByDescending(IsBedCrib).
                    ThenByDescending(d => d.GetStatValueAbstract(StatDefOf.BedRestEffectiveness, null)).ToList();
                return bedDefsBestToWorst_CribRestEffectiveness;
            }
        }

        public static bool ShouldBeCaredFor(Pawn p)
        {
            if(p == null || 
               !p.Spawned || 
               (p.Faction != Faction.OfPlayer && p.HostFaction != Faction.OfPlayer) || 
               p.playerSettings.medCare == MedicalCareCategory.NoCare)
                return false;
            var bed = p.CurrentBed();
            if (bed == null || bed.Faction != Faction.OfPlayer)
                return false;
            
            var guest = p.guest;
            if (guest != null && guest.CanBeBroughtFood)
                return true;
            


            if (HealthAIUtility.ShouldSeekMedicalRest(p))
                return true;
            
            bool goodLayingStatusForTend = p.RaceProps.Humanlike ? p.InBed() : (uint) p.GetPosture() > 0U;
            var designationSlaughter = p.Map.designationManager.DesignationOn(p, DesignationDefOf.Slaughter);

            return goodLayingStatusForTend && designationSlaughter == null;

        }


        public static bool ShouldBeFed(Pawn p)
        {
            return p != null && ShouldBeCaredFor(p) && p.RaceProps != null &&p.RaceProps.EatsFood;
        }

        /// <summary>
        /// Returns true if a crying baby is nearby.
        /// </summary>
        /// <param name="pawn">pawn which looks for crying babies nearby</param>
        public static bool NearCryingBaby(Pawn pawn)
        {
            // Does not affect babies and toddlers
            if (AgeStages.IsYoungerThan(pawn, AgeStages.Child) || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing) <= 0.1f) { return false; }

            // Find any crying babies in the vicinity
            foreach (Pawn mapPawn in pawn.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (RaceUtility.PawnUsesChildren(mapPawn) && 
                    AgeStages.IsAgeStage(pawn, AgeStages.Baby) &&
                    mapPawn.health.hediffSet.HasHediff(HediffDef.Named("UnhappyBaby")) &&
                    mapPawn.PositionHeld.InHorDistOf(pawn.PositionHeld, 24) &&
                    mapPawn.PositionHeld.GetRoomOrAdjacent(mapPawn.MapHeld).ContainedAndAdjacentThings.Contains(pawn))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Helper method to determine if a pawn ought to use a crib based on age/life stage
        /// </summary>
        /// <param name="pawn">maybe a baby?</param>
        /// <returns>True if the pawn should be using a crib</returns>
        public static bool ShouldUseCrib(Pawn pawn)
        {
            //pawn.BodySize;
            //Probably ought to change this based on size at some point, age stages are unreliable
            return RaceUtility.PawnUsesChildren(pawn) && (AgeStages.IsYoungerThan(pawn, AgeStages.Child));
        }

        /// <summary>
        /// Returns the maximum possible mass of a weapon the specified child can use
        /// </summary>
        public static float GetMaxWeaponMass(Pawn pawn)
        {
            if (!AgeStages.IsYoungerThan(pawn, AgeStages.Teenager))
                return float.PositiveInfinity;
            
            return (pawn.ageTracker.AgeBiologicalYearsFloat * 0.1f) + BnCSettings.option_child_max_weapon_mass;
        }

        /// <summary>
        /// Determines if a pawn is capable of currently breastfeeding
        /// </summary>
        public static bool CanBreastfeed(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(HediffDef.Named("Lactating"));
        }

        
        /// <summary>
        /// Finds a bed for a pawn prioritizing cribs over normal beds.
        /// </summary>
        /// <param name="baby"></param>
        /// <param name="traveler"></param>
        /// <returns></returns>
        public static Building_Bed FindCribFor(Pawn baby, Pawn traveler)
        {
            Building_Bed crib = null;
            // Is a crib already assigned to the baby?
            if (baby.ownership?.OwnedBed != null && ChildrenUtility.IsBedCrib(baby.ownership.OwnedBed))
            {
                Building_Bed bedThing = baby.ownership.OwnedBed;
                if (RestUtility.IsValidBedFor(bedThing, baby, traveler, false, false, false, true))
                {
                    crib = baby.ownership.OwnedBed;
                }
            }
            // If not, let's look for one
            else
            {
                foreach (var thingDef in RestUtility.AllBedDefBestToWorst)
                {
                    if (RestUtility.CanUseBedEver(baby, thingDef) && IsBedCrib(thingDef))
                    {
                        Building_Bed find_crib = (Building_Bed)GenClosest.ClosestThingReachable(
                            baby.Position, 
                            baby.Map, 
                            ThingRequest.ForDef(thingDef), 
                            PathEndMode.OnCell, 
                            TraverseParms.For(traveler), 
                            9999f, 
                            (Thing b) => (RestUtility.IsValidBedFor(b, baby, traveler, false, false)), null);
                        if (find_crib != null) crib = find_crib;
                    }
                }
            }
            return crib;
        }

        public static bool IsBedCrib(Building_Bed bed)
        {
            return IsBedCrib(bed.def);
        }

        public static bool IsBedCrib(ThingDef bed)
        {
            return (bed.building.bed_humanlike && bed.building.bed_maxBodySize <= BnCSettings.MAX_CRIB_BODYSIZE);
        }

        /// <summary>
        /// Returns the accelerated growth factor setting value for a given growth stage.
        /// </summary>
        public static int SettingAcceleratedFactor(int growthStage)
        {
            switch (growthStage)
            {
                case 0: return BnCSettings.baby_accelerated_growth - 1;
                case 1: return BnCSettings.toddler_accelerated_growth - 1;
                case 2: return BnCSettings.child_accelerated_growth - 1;
                case 3: return BnCSettings.teenager_accelerated_growth - 1;
            }
            return 1;
        }

        public static bool ToddlerIsUpright(Pawn pawn)
        {
            if (!RaceUtility.PawnUsesChildren(pawn))
                return false;
            float childAge = LifeStageUtility.GetLifeStageAge(pawn, AgeStages.Child).minAge;
            float toddlerAge = LifeStageUtility.GetLifeStageAge(pawn, AgeStages.Toddler).minAge;
            float toddlerDuration = childAge - toddlerAge;

            //second half of toddler agestage is upright
            return pawn.ageTracker.AgeBiologicalYearsFloat >= toddlerAge + toddlerDuration / 2;
        }

        public static bool SetMakerTagCheck(Thing thing, string tag)
        {
            return thing.def.thingSetMakerTags != null && thing.def.thingSetMakerTags.Contains(tag);
        }

        public static void TryDropInvalidEquipment(Pawn pawn)
        {
                ThingWithComps toy = pawn.equipment.Primary;
                if (toy != null && ChildrenUtility.SetMakerTagCheck(toy, "Toy"))
                { pawn.equipment.TryDropEquipment(toy, out _, pawn.Position, false);
                }
            
        }

        public static void TryDropInvalidApparel(Pawn pawn)
        {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                foreach (var apparel in pawn.apparel.WornApparel)
                {
                    if (AgeStages.IsOlderThan(pawn, AgeStages.Baby) && SetMakerTagCheck(apparel, "BabyGear1"))
                    {
                        
                    }
                }
                for (int i = wornApparel.Count - 1; i >= 0; i--)
                {
                    if (ChildrenUtility.SetMakerTagCheck(wornApparel[i], "BabyGear"))
                    {
                        pawn.apparel.TryDrop(wornApparel[i], out _, pawn.Position, false);
                    }
                }
            
        }

        /// <summary>
        /// Try's to drop pawn's toy or baby gear if they are too old for it
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="tag">tag of thing to be dropped (BabyGear or Toy)</param>
        public static void TryDropInvalidEquipmentAndApparel(Pawn pawn)
        {
            TryDropInvalidApparel(pawn);
            TryDropInvalidEquipment(pawn);
        }
        
        /// <summary>
        /// Searches for a comp in a ThingWithComps by comp name
        /// </summary>
        public static ThingComp GetCompByClassName(ThingWithComps thing, string compClassName)
        {
            if (compClassName == null)
            {
                return null;
            }
            
            foreach (ThingComp comp in thing.AllComps)
            {
                if (comp?.props == null || (comp.props.compClass == null))
                {
                    continue;
                }

                if (comp.props.compClass.ToString() == compClassName)
                {
                    return comp;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Sets a pawn's name to a new random name with Last time as father's or if
        /// father is null, the mothers.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="mother"></param>
        /// <param name="father"></param>
        public static void RenamePawn(Pawn pawn, Pawn mother, Pawn father)
        {
            NameTriple NameTriple = null;
            if (mother != null)
                NameTriple = NameTriple.FromString(mother.Name.ToStringFull);
            if (father != null)
                NameTriple = NameTriple.FromString(father.Name.ToStringFull);
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, NameTriple?.Last);
        }

        //Modified version of Children.PawnRenderer_RenderPawnInternal_Patch:GetBodysizeScaling
        public static float GetHairSize(float n, Pawn pawn)
        {
            if (AgeStages.GetAgeStage(pawn) > AgeStages.Child) return 1f;
            if (n != 0)
            {
                if (RaceUtility.IsHuman(pawn))
                {
                    return BnCSettings.HumanHairSize * BnCSettings.ShowHairSize * AgeFactor(pawn);
                }
                else return BnCSettings.AlienHairSize * BnCSettings.ShowHairSize * AgeFactor(pawn);
            }

            if (RaceUtility.IsHuman(pawn))
            {
                return BnCSettings.HumanHairSize * AgeFactor(pawn);
            }
            else return BnCSettings.AlienHairSize * AgeFactor(pawn);
        }

        public static float GetHeadSize(Pawn pawn)
        {
            if (RaceUtility.IsHuman(pawn))
            {
                return BnCSettings.HumanHeadSize * AgeFactor(pawn);
            }
            else
            {
                if (BnCSettings.human_like_head_enabled && RaceUtility.HasHumanlikeHead(pawn)) 
                    return BnCSettings.AlienHeadSizeB * AgeFactor(pawn);
                else 
                    return BnCSettings.AlienHeadSizeA * AgeFactor(pawn);
            }
        }
        /// <summary>
        /// A BodySize different from pawn.BodySize calculated based on AgeFactor
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static float GetBodySize(Pawn pawn)
        {
            if (RaceUtility.IsHuman(pawn))
            {
                return BnCSettings.HumanBodySize * AgeFactor(pawn);
            }
            else return BnCSettings.AlienBodySize * AgeFactor(pawn);
        }

        public static Vector3 ModifiedHairLoc(Vector3 pos, Pawn pawn)
        {
            Vector3 newPos = new Vector3(pos.x, pos.y, pos.z);
            if (!AgeStages.IsAgeStage(pawn, AgeStages.Child)) return newPos;
            newPos.y += BnCSettings.ShowHairLocY;

            if (RaceUtility.IsHuman(pawn))
            {
                newPos.z += BnCSettings.ShowHairHumanLocZ * AgeFactor(pawn);
            }
            else
            {
                if (BnCSettings.human_like_head_enabled && RaceUtility.HasHumanlikeHead(pawn))
                {
                    newPos.z += BnCSettings.ShowHairAlienHFLocZ * AgeFactor(pawn);
                }
                else
                {
                    newPos.z += BnCSettings.ShowHairAlienLocZ * AgeFactor(pawn);
                }
            }
            return newPos;
        }
        
        /// <summary>
        /// Magic number between .8 and 1.6 depending on age of pawn
        /// </summary>
        public static float AgeFactor(Pawn pawn)
        {   
            //Age factor only relevant for children and teens
            if (!RaceUtility.PawnUsesChildren(pawn) || AgeStages.IsYoungerThan(pawn, AgeStages.Child) || AgeStages.IsOlderThan(pawn, AgeStages.Teenager))
                return 1f; 
            
            
            float agechild = LifeStageUtility.GetLifeStageAge(pawn, AgeStages.Child).minAge;
            float ageteen = LifeStageUtility.GetLifeStageAge(pawn, AgeStages.Teenager).minAge;

            float childLifeStageDuration = ageteen - agechild;
            
            float now = pawn.ageTracker.AgeBiologicalYearsFloat + 0.1f; // prevent 0 + 0.1f

            float yearsSinceToddler = now - agechild;

            const float offset = 0.8f;
            const float scalar = 0.3f;
            
            //x = yearsSinceToddler / childLifeStageDuration
            //0 < x < 2
            //* scalar -> 0 < x < 0.6
            //+ offset -> 0.8 < x < 1.4
            
            float agefac = offset + scalar * yearsSinceToddler / childLifeStageDuration;
            
            return agefac;
        }

        /// <summary>
        /// Get the most appropriate bed list for a pawn. Child pawns will receive 
        /// a bed list sorted to prioritize cribs
        /// </summary>
        /// <param name="pawn">The pawn being evaluated</param>
        /// <returns>Sorted list of beds </returns>
        public static List<ThingDef> GetSortedBeds_RestEffectiveness(Pawn pawn) {
            return (ChildrenUtility.ShouldUseCrib(pawn)) ? ChildrenUtility.AllBedDefBestToWorstCribRest : RestUtility.AllBedDefBestToWorst;
        }


        public static float GetPainShockThreshold(Pawn pawn)
        {
            float percentage = 1f;

            if (AgeStages.IsYoungerThan(pawn, AgeStages.Teenager))
            {
                percentage = 0.75f;
            }

            return pawn.GetStatValue(StatDefOf.PainShockThreshold, true) * percentage;
        }

        public static bool ApplyRecoil(Verb_Shoot verb)
        {

            Pawn pawn = (Pawn) verb?.caster;
            
            if (pawn == null || verb.EquipmentSource.def.BaseMass > GetMaxWeaponMass(pawn))
            {
                return false;
            }
            pawn.equipment.TryDropEquipment(verb.EquipmentSource, out _, pawn.Position, false);

            float recoilForce = verb.EquipmentSource.def.BaseMass - 3;

            if (recoilForce <= 0)
            {
                return false;
            }

            string[] hitPart =
            {
                "Torso",
                "Shoulder",
                "Arm",
                "Hand",
                "Head",
                "Neck",
                "Eye",
                "Nose",
            };
            int hits = Rand.Range(1, 4);
            while (hits > 0)
            {
                pawn.TakeDamage(new DamageInfo(DamageDefOf.Blunt,
                    (int) ((recoilForce + Rand.Range(0f, 3f)) / hits), 0, -1,
                    verb.EquipmentSource,
                    HealthUtility.GetPawnBodyPart(pawn, hitPart.RandomElement()), null));
                hits--;
            }

            return true;
        }
    }
}