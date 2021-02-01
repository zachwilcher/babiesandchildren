using AlienRace;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using BabiesAndChildren.Tools;
using UnityEngine;
using Verse;
using Verse.AI;
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


        public static bool ShouldBeFed(Pawn p)
        {
            if (p.GetPosture() == PawnPosture.Standing)
            {
                return false;
            }
            if (p.NonHumanlikeOrWildMan())
            {
                Building_Bed building_Bed = p.CurrentBed();
                if (building_Bed == null || building_Bed.Faction != Faction.OfPlayer)
                {
                    return false;
                }
            }
            else
            {
                if (p.Faction != Faction.OfPlayer && p.HostFaction != Faction.OfPlayer)
                {
                    return false;
                }
                if (!p.InBed())
                {
                    return false;
                }
            }
            if (!p.RaceProps.EatsFood)
            {
                return false;
            }
            if (p.HostFaction != null)
            {
                if (p.HostFaction != Faction.OfPlayer)
                {
                    return false;
                }
                if (p.guest != null && !p.guest.CanBeBroughtFood)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if a crying baby is nearby.
        /// </summary>
        /// <param name="pawn">pawn which looks for crying babies nearby</param>
        public static bool NearCryingBaby(Pawn pawn)
        {
            // Does not affect babies and toddlers
            if (AgeStage.IsYoungerThan(pawn, AgeStage.Child) || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing) <= 0.1f) { return false; }

            // Find any crying babies in the vicinity
            foreach (Pawn mapPawn in pawn.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (RaceUtility.PawnUsesChildren(mapPawn) && 
                    AgeStage.IsAgeStage(pawn, AgeStage.Baby) &&
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
            //Probably ought to change this based on size at some point, age stages are unreliable
            return RaceUtility.PawnUsesChildren(pawn) && (AgeStage.IsYoungerThan(pawn, AgeStage.Child));
        }

        /// <summary>
        /// Returns the maximum possible mass of a weapon the specified child can use
        /// </summary>
        public static float ChildMaxWeaponMass(Pawn pawn)
        {
            if (!AgeStage.IsYoungerThan(pawn, AgeStage.Teenager))
                return 999;
            //const float baseMass = 2.5f;
            //return (pawn.skills.GetSkill(SkillDefOf.Shooting).Level * 0.1f) + baseMass;
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
                    if (RestUtility.CanUseBedEver(baby, thingDef) && thingDef.building.bed_maxBodySize <= 0.6f)
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
            return (bed.building.bed_humanlike && bed.building.bed_maxBodySize <= 0.6f);
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
            float a = AgeStage.GetLifeStageAge(pawn, AgeStage.Toddler).minAge + ((AgeStage.GetLifeStageAge(pawn, AgeStage.Child).minAge - AgeStage.GetLifeStageAge(pawn, AgeStage.Toddler).minAge) / 2);
            return pawn.ageTracker.AgeBiologicalYearsFloat > a;
        }

        public static bool SetMakerTagCheck(Thing thing, string tag)
        {
            return thing.def.thingSetMakerTags != null && thing.def.thingSetMakerTags.Contains(tag);
        }

        /// <summary>
        /// Try's to drop pawn's toy or baby gear if they are too old for it
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="tag">tag of thing to be dropped (BabyGear or Toy)</param>
        public static void TryDrop(Pawn pawn, string tag)
        {
            if (tag == "BabyGear")
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int i = wornApparel.Count - 1; i >= 0; i--)
                {
                    if (ChildrenUtility.SetMakerTagCheck(wornApparel[i], "BabyGear"))
                    {
                        pawn.apparel.TryDrop(wornApparel[i], out _, pawn.Position, false);
                    }
                }
            }
            if (tag == "Toy")
            {
                ThingWithComps toy = pawn.equipment.Primary;
                if (toy != null && ChildrenUtility.SetMakerTagCheck(toy, "Toy"))
                {
                    pawn.equipment.TryDropEquipment(toy, out _, pawn.Position, false);
                }
            }
        }
        
        /// <summary>
        /// Searches for a comp in pawn with matching class name
        /// </summary>
        public static ThingComp GetCompByClassName(Pawn pawn, string compClassName)
        {
            foreach (ThingComp comp in pawn.AllComps)
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
        /// Changes pawn's body type based on it's AgeStage
        /// </summary>
        /// <param name="pawn">pawn to be altered</param>
        /// <param name="Is_SizeInit">Whether to randomly initialize the size (if appropriate) of the pawn's heDiffs</param>
        /// <param name="Is_ChangeSize_Skip">Whether to change the size (if appropriate) of the pawn's heDiffs</param>
        public static void ChangeBodyType(Pawn pawn, bool Is_SizeInit = false, bool Is_ChangeSize_Skip = true)
        {
            
            
            float size = 0.01f;
            
            switch (AgeStage.GetAgeStage(pawn))
            {
                case AgeStage.Adult:
                    size = 1f;
                    break;
                
                case AgeStage.Teenager:
                    //35% chance of thin body type
                    if (Rand.Value < 0.35f)
                    {
                        TrySetPawnBodyType(pawn, BodyTypeDefOf.Thin);
                    }
                    else if(pawn.gender == Gender.Male)
                    {
                        TrySetPawnBodyType(pawn, BodyTypeDefOf.Male);
                    }
                    else if (pawn.gender == Gender.Female)
                    {
                        TrySetPawnBodyType(pawn, BodyTypeDefOf.Female);
                    }
                    size = 0.8f;
                    break;
                case AgeStage.Child:
                    TrySetPawnBodyType(pawn, BodyTypeDefOf.Thin);
                    size = 0.12f;
                    break;

                case AgeStage.Toddler:
                if (ToddlerIsUpright(pawn))
                {
                    TrySetPawnBodyType(pawn, BodyTypeDefOf.Thin);
                    size = 0.10f;
                }
                else
                {
                    TrySetPawnBodyType(pawn, BodyTypeDefOf.Fat);
                    size = 0.08f;
                }
                break;

                case AgeStage.Baby:
                    TrySetPawnBodyType(pawn, BodyTypeDefOf.Fat);
                    size = 0.07f;
                    break;

            }
            if (!Is_ChangeSize_Skip) 
                ModTools.ChangeSize(pawn, size, Is_SizeInit);
        }

        public static bool TrySetPawnBodyType(Pawn pawn, BodyTypeDef bodyTypeDef, bool force = false)
        {
            if (pawn.def == DefDatabase<ThingDef>.GetNamed("Human") || force)
            {
                pawn.story.bodyType = bodyTypeDef;
                return true;
            } 
            if (pawn.def is ThingDef_AlienRace thingDef)
            {
                List<BodyTypeDef> bodyTypes = thingDef.alienRace.generalSettings.alienPartGenerator.alienbodytypes;
                if (bodyTypes.NullOrEmpty())
                    return false;

                if (bodyTypes.Contains(bodyTypeDef))
                {
                    pawn.story.bodyType = bodyTypeDef;
                    return true;
                }

                //can't set to desired to body type but leaving an invalid one
                //leads to pink boxes
                if (!bodyTypes.Contains(pawn.story.bodyType))
                {
                    pawn.story.bodyType = bodyTypes.RandomElement<BodyTypeDef>();
                    return false;
                }
                
            }
            //pawn's which are not humans or alien races is out of the scope of this mod
            return false;
        }

        /// <summary>
        /// Removes hediffs of types: Hediff_Implant, Hediff_Addiction, and Hediff_MissingPart
        /// </summary>
        /// <param name="pawn">Pawn to be altered</param>
        public static void ClearImplantAndAddiction(Pawn pawn)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (hediffs[i] is Hediff_Implant || hediffs[i] is Hediff_Addiction || hediffs[i] is Hediff_MissingPart)
                {
                    pawn.health.hediffSet.hediffs.Remove(hediffs[i]);
                }
            }
            pawn.health.Notify_HediffChanged(null);
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
                NameTriple = (NameTriple) mother.Name;
            if (father != null)
                NameTriple = (NameTriple)father.Name;
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, NameTriple?.Last);
        }

        public static void ChangeChildBackstory(Pawn pawn)
        {
            if (pawn == null) return;
            if (AgeStage.IsAgeStage(pawn, AgeStage.Child) && pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"])
            {
                pawn.story.childhood = BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"];
                pawn.Notify_DisabledWorkTypesChanged();
                pawn.skills.Notify_SkillDisablesChanged();
                MeditationFocusTypeAvailabilityCache.ClearFor(pawn);

                if (pawn.TryGetComp<Growing_Comp>() != null)
                {
                    Pawn mother = pawn.GetMother();
                    Pawn father = pawn.GetFather();
                    if (mother != null)
                    {
                        MathTools.Fixed_Rand rand = new MathTools.Fixed_Rand((int)mother.ageTracker.AgeBiologicalTicks);
                        BabyTools.SetBabySkillsAndPassions(pawn, mother, father, rand);
                        List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
                        foreach (var skillDef in allDefsListForReading)
                        {
                            pawn.skills.Learn(skillDef, 100, true);
                            CLog.DevMessage("Showbaby skill>> " + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));
                        }
                        Messages.Message("Successfully changed child's Backstory", MessageTypeDefOf.PositiveEvent);
                    }
                    //else Messages.Message("Mother Is Null", MessageTypeDefOf.NeutralEvent);
                }

                //else Messages.Message("Choose a child who has a backstory 'Baby'", MessageTypeDefOf.NeutralEvent);
            }
            //else Messages.Message("Choose a child (Not baby)", MessageTypeDefOf.NeutralEvent);
        }

        //Modified version of Children.PawnRenderer_RenderPawnInternal_Patch:GetBodysizeScaling
        public static float GetHairSize(float n, Pawn pawn)
        {
            if (pawn.ageTracker.CurLifeStageIndex > AgeStage.Child) return 1f;
            if (n != 0)
            {
                if (pawn.def.defName == "Human")
                {
                    return BnCSettings.HumanHairSize * BnCSettings.ShowHairSize * AgeFactor(pawn);
                }
                else return BnCSettings.AlienHairSize * BnCSettings.ShowHairSize * AgeFactor(pawn);
            }

            if (pawn.def.defName == "Human")
            {
                return BnCSettings.HumanHairSize * AgeFactor(pawn);
            }
            else return BnCSettings.AlienHairSize * AgeFactor(pawn);
        }

        public static float GetHeadSize(Pawn pawn)
        {
            if (pawn.def.defName == "Human")
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

        public static float GetBodySize(Pawn pawn)
        {
            if (pawn.def.defName == "Human")
            {
                return BnCSettings.HumanBodySize * AgeFactor(pawn);
            }
            else return BnCSettings.AlienBodySize * AgeFactor(pawn);
        }

        public static Vector3 ModifiedHairLoc(Vector3 pos, Pawn pawn)
        {
            Vector3 newPos = new Vector3(pos.x, pos.y, pos.z);
            if (!AgeStage.IsAgeStage(pawn, AgeStage.Child)) return newPos;
            newPos.y += BnCSettings.ShowHairLocY;

            if (pawn.def.defName == "Human")
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

        public static float AgeFactor(Pawn pawn)
        {
            if (pawn.ageTracker.CurLifeStageIndex < AgeStage.Child) return 1f;
            float agechild = AgeStage.GetLifeStageAge(pawn, AgeStage.Child).minAge;
            float ageteen = AgeStage.GetLifeStageAge(pawn, AgeStage.Teenager).minAge;
            float now = pawn.ageTracker.AgeBiologicalYearsFloat + 0.1f; // prevent 0 + 0.1f

            float agefac = 0.8f + (0.3f * (now - agechild) / (ageteen - agechild));
            //if (agefac < 0.7f) return 0.7f;
            return agefac;
        }

        /// <summary>
        /// Get the most appropriate bed list for a pawn. Child pawns will recieve 
        /// a bed list sorted to prioritize cribs
        /// </summary>
        /// <param name="pawn">The pawn being evaluated</param>
        /// <returns>Sorted list of beds </returns>
        public static List<ThingDef> GetSortedBeds_RestEffectiveness(Pawn pawn) {
            return (ChildrenUtility.ShouldUseCrib(pawn)) ? ChildrenUtility.AllBedDefBestToWorstCribRest : RestUtility.AllBedDefBestToWorst;
        }
        
        

    }
}