using AlienRace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Random = System.Random;

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
                {
                    return bedDefsBestToWorst_CribRestEffectiveness;
                }
                else
                {
                    bedDefsBestToWorst_CribRestEffectiveness = DefDatabase<ThingDef>.AllDefs.Where(def => def.IsBed).OrderByDescending(def => IsBedCrib(def)).ThenByDescending(d => d.GetStatValueAbstract(StatDefOf.BedRestEffectiveness, null)).ToList();
                    return bedDefsBestToWorst_CribRestEffectiveness;
                }
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

        public static int GetAgeStage(Pawn pawn)
        {
            return pawn.ageTracker.CurLifeStageIndex;
        }

        /// <summary>
        /// Returns true if a crying baby is nearby.
        /// </summary>
        /// <param name="pawn">pawn which looks for crying babies nearby</param>
        public static bool NearCryingBaby(Pawn pawn)
        {
            // Does not affect babies and toddlers
            if (GetAgeStage(pawn) < AgeStage.Child || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Hearing) <= 0.1f) { return false; }

            // Find any crying babies in the vicinity
            foreach (Pawn mapPawn in pawn.MapHeld.mapPawns.AllPawnsSpawned)
            {
                if (RaceUsesChildren(mapPawn) &&
                    GetAgeStage(mapPawn) == 0 &&
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
            return GetAgeStage(pawn) <= AgeStage.Toddler;
        }

        // Returns the maximum possible mass of a weapon the specified child can use
        public static float ChildMaxWeaponMass(Pawn pawn)
        {
            if (ChildrenUtility.GetAgeStage(pawn) >= AgeStage.Teenager)
                return 999;
            //const float baseMass = 2.5f;
            //return (pawn.skills.GetSkill(SkillDefOf.Shooting).Level * 0.1f) + baseMass;
            return (pawn.ageTracker.AgeBiologicalYearsFloat * 0.1f) + BnCSettings.option_child_max_weapon_mass;
        }
        // Determines if a pawn is capable of currently breastfeeding
        public static bool CanBreastfeed(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(HediffDef.Named("Lactating"));
        }

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
                        Building_Bed find_crib = (Building_Bed)GenClosest.ClosestThingReachable(baby.Position, baby.Map, ThingRequest.ForDef(thingDef), PathEndMode.OnCell, TraverseParms.For(traveler), 9999f, (Thing b) => (RestUtility.IsValidBedFor(b, baby, traveler, false, false)), null);
                        if (find_crib != null) crib = find_crib;
                    }
                }
            }
            return crib;
        }

        public static bool IsBedCrib(Building_Bed bed)
        {
            return (bed.def.building.bed_humanlike && bed.def.building.bed_maxBodySize <= 0.6f);
        }

        public static bool IsBedCrib(ThingDef bed)
        {
            return (bed.building.bed_humanlike && bed.building.bed_maxBodySize <= 0.6f);
        }

        // Returns whether a race can become pregnant/have kids etc.
        public static bool RaceUsesChildren(Pawn pawn)
        {
            // This will eventually be changed to allow alien races to have children
            //if (pawn.def.defName == "Human")
            //    return true;
            if (pawn.RaceProps.Humanlike && pawn.RaceProps.lifeStageAges.Count == 5)

                return true;
            return false;
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

        /// <summary>
        /// Fetch a random body part matching the provided string
        /// </summary>
        /// <param name="pawn">The pawn whose parts will be searched</param>
        /// <param name="bodyPart">The string defName of the body part to be returned</param>
        /// <returns>A single bodypartrecord matching the provided string, or the only part if only one part exists</returns>
        internal static BodyPartRecord GetPawnBodyPart(Pawn pawn, String bodyPart)
        {
            //Get collection of parts matching the def, then get a random left or right
            return pawn.RaceProps.body.AllParts.FindAll(x => x.def == DefDatabase<BodyPartDef>.GetNamed(bodyPart, true)).RandomElement();
        }

        /// <summary>
        /// Returns a collection of BodyPartRecords based on the part name provided.
        /// This may be a collection containing a single element, or multiple for left and right parts
        /// </summary>
        /// <param name="pawn">The pawn whose parts will be searched</param>
        /// <param name="bodyPart">The string defName of the body part to be returned</param>
        /// <returns>A collection of bodypart records</returns>
        internal static List<BodyPartRecord> GetPawnBodyParts(Pawn pawn, String bodyPart)
        {
            return pawn.RaceProps.body.AllParts.FindAll(x => x.def == DefDatabase<BodyPartDef>.GetNamed(bodyPart, true));
        }


        /// <summary>
        /// Seed fixed Random fuction
        /// seed = pawn.ageTracker.AgeBiologicalTicks
        /// </summary>
        public class Fixed_Rand
        {
            private Random rand;
            private int nextindex;
            public Fixed_Rand(int seed)
            {
                rand = new Random(seed);
                nextindex = 0;
                CLog.DevMessage("   Seed = " + seed);
            }

            public bool Fixed_RandChance(double chance)
            {
                double t = rand.NextDouble();
                nextindex++;
                //CLog.DevMessage("  rand[" + nextindex + "] = " + t);
                if (t < chance) return true;
                else return false;
            }
            public bool Fixed_RandBool()
            {
                bool t = (rand.Next(0, 2) == 1);
                nextindex++;
                //CLog.DevMessage("  rand[" + nextindex + "] = " + t);
                return t;
            }
            public double Fixed_RandDouble(Double a, Double b)
            {
                double t = a + ((b - a) * rand.NextDouble());
                nextindex++;
                //CLog.DevMessage("  rand[" + nextindex + "] = " + t);
                return t;
            }
            public int Fixed_RandInt(int a, int b)
            {
                int t = rand.Next(a, b++);
                nextindex++;
                //CLog.DevMessage("   rand[" + nextindex + "] = " + t);
                return t;
            }
        }

        public static bool IsModOn(string modName)
        {
            foreach (ModMetaData modMetaData in ModLister.AllInstalledMods)
            {
                if ((modMetaData != null) && (modMetaData.enabled) && (modMetaData.Name.StartsWith(modName)))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HumanFaceRaces(Pawn pawn)
        {
            if (
                   pawn.def.defName == "Kurin_Race"
                || pawn.def.defName == "Ratkin"
                )
            {
                return true;
            }
            return false;
        }

        public static bool ToddlerIsUpright(Pawn pawn)
        {
            float a = pawn.def.race.lifeStageAges[AgeStage.Toddler].minAge + ((pawn.def.race.lifeStageAges[AgeStage.Child].minAge - pawn.def.race.lifeStageAges[AgeStage.Toddler].minAge) / 2);
            if (pawn.ageTracker.AgeBiologicalYearsFloat > a)
            {
                return true;
            }
            return false;
        }
        public static bool SetMakerTagCheck(Thing thing, string tag)
        {
            if (thing.def.thingSetMakerTags != null)
            {
                if (thing.def.thingSetMakerTags.Contains(tag)) return true;
            }
            return false;
        }

        public static void TryDrop(Pawn pawn, string tag)
        {
            if (tag == "BabyGear")
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int i = wornApparel.Count - 1; i >= 0; i--)
                {
                    if (ChildrenUtility.SetMakerTagCheck(wornApparel[i], "BabyGear"))
                    {
                        pawn.apparel.TryDrop(wornApparel[i], out Apparel resultingAp, pawn.Position, false);
                    }
                }
            }
            if (tag == "Toy")
            {
                ThingWithComps toy = pawn.equipment.Primary;
                if (toy != null && ChildrenUtility.SetMakerTagCheck(toy, "Toy"))
                {
                    pawn.equipment.TryDropEquipment(toy, out ThingWithComps thingWithComps, pawn.Position, false);
                }
            }
        }

        public static Hediff GetVag(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("vagina"));
        public static Hediff GetPen(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("penis"));
        public static Hediff GetBre(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("breasts"));
        public static Hediff GetAnu(Pawn pawn) => pawn.health.hediffSet.hediffs.Find((Hediff hed) => hed.def.defName.ToLower().Contains("anus"));

        public static void ChangeSize(Pawn pawn, float Maxsize, bool Is_SizeInit)
        {
            if (ChildrenBase.ModRJW_ON)
            {
                Hediff bodypart = GetAnu(pawn);
                if (bodypart == null) return;
                float size = (float)Rand.Range(0.01f, Maxsize);
                float cursize = bodypart.Severity;
                if (Is_SizeInit)
                {
                    bodypart.Severity = size;
                }
                else
                {
                    bodypart.Severity = ((cursize > size) ? cursize : size);
                }

                if (pawn.gender == Gender.Male)
                {
                    bodypart = GetPen(pawn);
                    if (bodypart == null) return;
                    size = (float)Rand.Range(0.01f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }
                }
                else if (pawn.gender == Gender.Female)
                {
                    bodypart = GetVag(pawn);
                    if (bodypart == null) return;
                    Maxsize = ((Maxsize < 0.35f) ? 0.35f : Maxsize);
                    size = (float)Rand.Range(0.02f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }

                    bodypart = GetBre(pawn);
                    if (bodypart == null) return;
                    if (GetAgeStage(pawn) < AgeStage.Teenager && Maxsize > 0.07f) Maxsize = 0.07f;
                    size = (float)Rand.Range(0.01f, Maxsize);
                    cursize = bodypart.Severity;
                    if (Is_SizeInit)
                    {
                        bodypart.Severity = size;
                    }
                    else
                    {
                        bodypart.Severity = ((cursize > size) ? cursize : size);
                    }
                }
            }
        }

        public static ThingComp GetCompByClassName(Pawn pawn, string compClassName)
        {
            foreach (ThingComp comp in pawn.AllComps)
            {
                if ((comp == null) || (comp.props == null) || (comp.props.compClass == null))
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
        public static void ChangeBodyType(Pawn pawn, bool Is_SizeInit, bool Is_ChangeSize_Skip)
        {
            float size = 0.01f;
            switch (GetAgeStage(pawn))
            {
                case AgeStage.Adult:
                    size = 1f;
                    goto default;

                case AgeStage.Teenager:
                    if (pawn.def.defName == "Human")
                    {
                        if (Rand.Value < 0.35f)
                        {
                            pawn.story.bodyType = BodyTypeDefOf.Thin;
                        }
                        else
                        {
                            pawn.story.bodyType = ((pawn.gender == Gender.Female) ? BodyTypeDefOf.Female : BodyTypeDefOf.Male);
                        }
                    }
                    size = 0.8f;
                    goto default;

                case AgeStage.Child:
                    if (pawn.def.defName == "Human")
                    {
                        pawn.story.bodyType = BodyTypeDefOf.Thin;
                    }
                    else
                    {
                        ThingDef_AlienRace thingDef_AlienRace = pawn.def as ThingDef_AlienRace;
                        if (thingDef_AlienRace != null && !thingDef_AlienRace.alienRace.generalSettings.alienPartGenerator.alienbodytypes.NullOrEmpty<BodyTypeDef>() && !thingDef_AlienRace.alienRace.generalSettings.alienPartGenerator.alienbodytypes.Contains(pawn.story.bodyType))
                        {
                            pawn.story.bodyType = thingDef_AlienRace.alienRace.generalSettings.alienPartGenerator.alienbodytypes.RandomElement<BodyTypeDef>();
                        }
                    }
                    size = 0.12f;
                    goto default;

                case AgeStage.Toddler:
                    if (ToddlerIsUpright(pawn))
                    {
                        pawn.story.bodyType = BodyTypeDefOf.Thin;
                        size = 0.10f;
                    }
                    else
                    {
                        pawn.story.bodyType = BodyTypeDefOf.Fat;
                        size = 0.08f;
                    }
                    goto default;

                case AgeStage.Baby:
                    pawn.story.bodyType = BodyTypeDefOf.Fat;
                    size = 0.07f;
                    goto default;

                default:
                    if (!Is_ChangeSize_Skip) ChangeSize(pawn, size, Is_SizeInit);
                    break;
            }
        }

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

        public static void RenamePawn(Pawn pawn, Pawn mother, Pawn father)
        {
            NameTriple NameTriple = (NameTriple)mother.Name;
            if (father != null)
            {
                NameTriple = (NameTriple)father.Name;
            }
            pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, NameTriple.Last);
        }

        public static void ChangeChildBackstory(Pawn pawn)
        {
                if (pawn != null)
                {
                    if (GetAgeStage(pawn) == AgeStage.Child)
                    {
                        if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"])
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
                                    ChildrenUtility.Fixed_Rand rand = new ChildrenUtility.Fixed_Rand((int)mother.ageTracker.AgeBiologicalTicks);
                                    BabyTools.SetBabySkillsAndPassions(pawn, mother, father, rand);
                                    List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
                                    for (int i = 0; i < allDefsListForReading.Count; i++)
                                    {
                                        SkillDef skillDef = allDefsListForReading[i];
                                        pawn.skills.Learn(skillDef, 100, true);
                                        CLog.DevMessage("Showbaby skill>> " + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));
                                    }
                                    Messages.Message("Successfully changed child's Backstory", MessageTypeDefOf.PositiveEvent);
                                }
                                //else Messages.Message("Mother Is Null", MessageTypeDefOf.NeutralEvent);
                            }
                        }
                        //else Messages.Message("Choose a child who has a backstory 'Baby'", MessageTypeDefOf.NeutralEvent);
                    }
                    //else Messages.Message("Choose a child (Not baby)", MessageTypeDefOf.NeutralEvent);
                }            
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
                if (BnCSettings.human_like_head_enabled && ChildrenUtility.HumanFaceRaces(pawn)) return BnCSettings.AlienHeadSizeB * AgeFactor(pawn);
                else return BnCSettings.AlienHeadSizeA * AgeFactor(pawn);
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
            if (pawn.ageTracker.CurLifeStageIndex != AgeStage.Child) return newPos;
            newPos.y += BnCSettings.ShowHairLocY;

            if (pawn.def.defName == "Human")
            {
                newPos.z += BnCSettings.ShowHairHumanLocZ * AgeFactor(pawn);
            }
            else
            {
                if (BnCSettings.human_like_head_enabled && ChildrenUtility.HumanFaceRaces(pawn))
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
            float agechild = pawn.def.race.lifeStageAges[AgeStage.Child].minAge;
            float ageteen = pawn.def.race.lifeStageAges[AgeStage.Teenager].minAge;
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



