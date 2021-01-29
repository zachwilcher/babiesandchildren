using System;
using System.Collections.Generic;
using System.Linq;
using BabiesAndChildren.api;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BabiesAndChildren
{
    public static class BabyTools
    {
        public static void BabyProcess(Pawn pawn, Pawn mother, Pawn father, MathTools.Fixed_Rand rand)
        {
            TryAddHediff(mother, HediffDef.Named("PostPregnancy"));
            TryAddHediff(mother, HediffDef.Named("Lactating"), mother.RaceProps.body.AllParts.Find(x => x.def.defName == "Torso"));
            
            if (ChildrenBase.ModRJW_ON && BnCSettings.enable_postpartum)
            {
                TryAddHediff(mother, HediffDef.Named("BnC_RJW_PostPregnancy"));
            }
            

            //Make crying sound when baby is born
            SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn.PositionHeld, pawn.MapHeld));
            SoundDef.Named("Pawn_BabyCry").PlayOneShot(info);

            //ChildrenUtility.ChangeBodyType(pawn, true, false);
            ChildrenUtility.ClearImplantAndAddiction(pawn);
            ChildrenUtility.RenamePawn(pawn, mother, father);

            //For rabbie
            if (pawn.def.defName == "Rabbie")
            {
                TryAddHediff(pawn, HediffDef.Named("PlanetariumAddiction"));
            }

            if (!ChildrenBase.ModCSL_ON)
            {
                SetBabyTraits(pawn, mother, father, rand);
                SetBabySkillsAndPassions(pawn, mother, father, rand);
            }
        }
    
        public static void TryAddHediff(Pawn pawn, HediffDef hediffDef, BodyPartRecord part = null, DamageInfo? damageInfo = null, bool force = false)
        {
            if (pawn == null )
                return;

            if (pawn.health.hediffSet.HasHediff(hediffDef) && !force)
                return;

            pawn.health.AddHediff(hediffDef, part, damageInfo);
        }

        public static void Miscarry(Pawn baby, Pawn mother, Pawn father)
        {
            baby.Name = new NameSingle("Unnamed".Translate(), false);
            baby.SetFaction(null, null);
            baby.health.AddHediff(HediffDef.Named("DefectStillborn"));

            if (father != null)
            {
                father.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("BabyStillborn"), baby);
                RemoveChildDiedThought(father, baby);
            }

            if (mother != null)
            {
                mother.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("BabyStillborn"), baby);
                RemoveChildDiedThought(mother, baby);
                Find.LetterStack.ReceiveLetter("WordStillborn".Translate(), TranslatorFormattedStringExtensions.Translate("MessageStillborn", mother.LabelIndefinite()), LetterDefOf.Death, mother);
            }
        }
        /// <summary>
        /// Verse.PawnGenerator:GenerateSkills
        /// Effectively generates random skills and passions
        /// </summary>
        /// <param name="pawn">pawn whose skills will be set</param>
        public static void PawnGenerator_GenerateSkills(Pawn pawn)
        {
            try
            {
                Traverse.CreateWithType("PawnGenerator").Method("GenerateSkills", pawn).GetValue();
                CLog.DevMessage("Skills for: " + pawn.Name.ToStringShort + " randomly generated.");
            }
            catch
            {
                CLog.DevMessage("Skills for: " + pawn.Name.ToStringShort + " failed to randomly generate.");
            }
        }

        /// <summary>
        /// Verse.PawnGenerator:GenerateTraits
        /// Effectively generates random traits
        /// </summary>
        /// <param name="pawn">pawn whose traits will be set</param>
        public static void PawnGenerator_GenerateTraits(Pawn pawn)
        {
            try
            {
                PawnGenerationRequest request = new PawnGenerationRequest(pawn.kindDef, pawn.Faction);
                Traverse.CreateWithType("PawnGenerator").Method("GenerateTraits", pawn, request).GetValue();
                CLog.DevMessage("Traits for: " + pawn.Name.ToStringShort + " randomly generated.");
            }
            catch 
            {
                CLog.DevMessage("Traits for: " + pawn.Name.ToStringShort + " failed to randomly generate.");
            }
        }

        /// <summary>
        /// Sets a SkillRecord passion in similar fashion to vanilla
        /// </summary>
        /// <param name="skillRecord"></param>
        /// <param name="rand"></param>
        /// <param name="always">Whether a passion should always be enabled</param>
        public static void SetPassion(SkillRecord skillRecord, MathTools.Fixed_Rand rand, bool always = false)
        {
            double threshold =  skillRecord.Level * 0.11;
            double randomDouble = rand.Fixed_RandDouble(0, 1);
            if (always || (randomDouble < threshold))
                skillRecord.passion = randomDouble >=  threshold * 0.2 ? Passion.Minor : Passion.Major;
            else
                skillRecord.passion = Passion.None;

        }
        
        /// <summary>
        /// Sets a pawn's skills and passions based on:
        /// 1. mother and father's skills
        /// 2. pawn's current traits
        /// </summary>
        /// <param name="pawn">pawn whose skills and passions will be set</param>
        /// <param name="mother">pawn's mother</param>
        /// <param name="father">pawn's father</param>
        /// <param name="rand">random number generator</param>
        public static void SetBabySkillsAndPassions(Pawn pawn, Pawn mother, Pawn father, MathTools.Fixed_Rand rand)
        {

            if (father == null || mother == null)
            {
                CLog.Warning("Father or Mother is null. Randomly generating skills.");
                PawnGenerator_GenerateSkills(pawn);
                return;
            }


            if (BnCSettings.baby_Inherit_percentage == BnCSettings.BabyInheritPercentageHandleEnum._Random)
            {
                PawnGenerator_GenerateSkills(pawn);
                return;
            }
            
            //change skills and passions based on mommy and daddy
            foreach (var skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
            {
                    
                int motherSkillLevel = mother.skills.GetSkill(skillDef).Level;
                CLog.DevMessage("mother's " + skillDef.defName + " Skills level =" + motherSkillLevel);

                int fatherSkillLevel = father.skills.GetSkill(skillDef).Level;
                CLog.DevMessage("father's " + skillDef.defName + " Skills level =" + fatherSkillLevel);

                SkillRecord babySkillRecord = pawn.skills.GetSkill(skillDef);
                
                if (babySkillRecord.TotallyDisabled)
                    continue;
                
                //baby's skill level is genetic
                int babySkillLevel = MathTools.Avg(fatherSkillLevel, motherSkillLevel);
                    
                //TODO have setting be variable instead of a dropdown
                switch (BnCSettings.baby_Inherit_percentage)
                {
                    case BnCSettings.BabyInheritPercentageHandleEnum._None:
                        break;

                    case BnCSettings.BabyInheritPercentageHandleEnum._25:
                        babySkillLevel = (int)Math.Floor(babySkillLevel * 0.25);
                        break;

                    case BnCSettings.BabyInheritPercentageHandleEnum._50:
                        babySkillLevel = (int)Math.Floor(babySkillLevel * 0.5);
                        break;

                    case BnCSettings.BabyInheritPercentageHandleEnum._75:
                        babySkillLevel = (int)Math.Floor(babySkillLevel * 0.75);
                        break;

                    case BnCSettings.BabyInheritPercentageHandleEnum._90:
                        babySkillLevel = (int)Math.Floor(babySkillLevel * 0.90);
                        break;

                }
                
                //randomize the final level a bit               
                babySkillLevel = (int) (babySkillLevel * rand.Fixed_RandDouble(0.7, 1.1));
               
                //level up
                babySkillRecord.EnsureMinLevelWithMargin(babySkillLevel);

                //add between 10% and 90% of xp required for next level up
                babySkillRecord.xpSinceLastLevel += (float)rand.Fixed_RandDouble(babySkillRecord.XpRequiredForLevelUp * 0.1, babySkillRecord.XpRequiredForLevelUp * 0.9);
               
                SetPassion(babySkillRecord, rand);
                
                CLog.DevMessage("" + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));

            }
            //change skills and passions based on pawn's current traits
            foreach(Trait trait in pawn.story.traits.allTraits) 
            {
                //enable forced passions
                foreach (SkillDef skillDef in trait.def.forcedPassions)
                {
                    SetPassion(pawn.skills.GetSkill(skillDef), rand, true);
                }
                
                //disable conflicting passions
                foreach (SkillDef skillDef in trait.def.conflictingPassions)
                {
                    pawn.skills.GetSkill(skillDef).passion = Passion.None;
                }

                if (trait.CurrentData?.skillGains == null)
                    continue;
                
                //add levels based on trait's skillGains
                foreach (var  kvp in trait.CurrentData.skillGains)
                {
                    
                    SkillRecord babySkillRecord = pawn.skills.GetSkill(kvp.Key);
                    babySkillRecord.Level += kvp.Value;
                }


            }
            
        }
        /// <summary>
        /// Sets the babies traits either randomly or based on mother and father
        /// depending on mod settings and luck
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="mother"></param>
        /// <param name="father"></param>
        /// <param name="rand"></param>
        public static void SetBabyTraits(Pawn pawn, Pawn mother, Pawn father, MathTools.Fixed_Rand rand)
        {
            //clear traits
            pawn.story.traits.allTraits.Clear();
            CLog.DevMessage("Traits cleared for parent inherit");
            
            if (mother == null || father == null)
            {
                CLog.Warning("Mother or father of: " + pawn.Name.ToStringShort + " is null, generating random traits.");
                PawnGenerator_GenerateTraits(pawn);
                return;
            }

            if (BnCSettings.baby_Inherit_percentage == BnCSettings.BabyInheritPercentageHandleEnum._Random)
            {
                PawnGenerator_GenerateTraits(pawn);
                return;
            }

            //add new type and sexuality
            GetNewTypeAndSexuality(pawn, rand);
            
            if (BnCSettings.baby_Inherit_percentage == BnCSettings.BabyInheritPercentageHandleEnum._None)
                return;

            int fatherTraitCount = father.story.traits.allTraits.Count;
            List<Trait> fatherTraitList = father.story.traits.allTraits;
            
            if (fatherTraitCount <= 0)
                CLog.DevMessage("Father has no traits!");

            int motherTraitCount = mother.story.traits.allTraits.Count;
            List<Trait> motherTraitList = mother.story.traits.allTraits;
            if (motherTraitCount <= 0)
                CLog.DevMessage("Mother has no traits!");

            if (BnCSettings.debug_and_gsetting)
            {
                foreach (var trait in fatherTraitList)
                {
                    CLog.DevMessage("Father trait: " + trait.def.defName);
                }

                foreach (var trait in motherTraitList)
                {
                    CLog.DevMessage("Mother trait: " + trait.def.defName);
                }
            }

            double inheritChance = 1;
            switch (BnCSettings.baby_Inherit_percentage)
            {
                case BnCSettings.BabyInheritPercentageHandleEnum._90:
                    inheritChance = 0.9 + 0.1;
                    break;
                case BnCSettings.BabyInheritPercentageHandleEnum._75:
                    inheritChance = 0.75 + 0.1;
                    break;
                case BnCSettings.BabyInheritPercentageHandleEnum._50:
                    inheritChance = 0.5 + 0.1;
                    break;
                case BnCSettings.BabyInheritPercentageHandleEnum._25:
                    inheritChance = 0.25 + 0.1;
                    break;
            }


            InheritTraits(pawn, mother, rand, inheritChance);
            InheritTraits(pawn, father, rand, inheritChance + 0.2);

            //Add more traits for some reason
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (BnCSettings.MAX_TRAIT_COUNT > 2)
            {
                // give random trait
                if (pawn.story.traits.allTraits.Count == 2)
                {
                    if (rand.Fixed_RandChance(0.15)) GiveARandomTrait(pawn, rand);
                }
                else if (pawn.story.traits.allTraits.Count <= 1)
                {
                    if (rand.Fixed_RandChance(0.4)) GiveARandomTrait(pawn, rand);
                    if (rand.Fixed_RandChance(0.15)) GiveARandomTrait(pawn, rand);
                }
            }
            
        }

        /// <summary>
        /// Adds traits from parent to child based on inheritChance, mod settings, and luck.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <param name="rand"></param>
        /// <param name="inheritChance"></param>
        public static void InheritTraits(Pawn child, Pawn parent, MathTools.Fixed_Rand rand, double inheritChance)
        {
            if (child == null || parent == null)
                return;

            if (parent.story.traits.allTraits.Count <= 0 || child.story.traits.allTraits.Count > BnCSettings.MAX_TRAIT_COUNT)
                return;
            
            List<Trait> parentTraits = new List<Trait>();

            foreach (Trait trait in parent.story.traits.allTraits)
            {
                if (PregnancyUtility.IsGeneticTrait(trait.def))
                {
                    parentTraits.Add(trait);
                }
            }
            
            if(parentTraits.Count <= 0) return;
            

            int traitsToGive = rand.Fixed_RandInt(1, 2);
            int traitsAdded = 0;
            
            int attempts = 0;
            int maxAttempts = parentTraits.Count;
            
            while ((traitsAdded < BnCSettings.MAX_TRAIT_COUNT) &&
                   (traitsAdded < traitsToGive) &&
                   (attempts++ < maxAttempts))
            {
                var traitToGive = (Trait) rand.Fixed_RandElement(parentTraits);
                if (ApplyTraitToPawn(child, traitToGive, rand, inheritChance))
                {
                    traitsAdded++;
                }

            }

        }
        /// <summary>
        /// Tries to apply a random trait from PregnancyUtility.GetGeneticTraits()
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="rand"></param>
        /// <returns>whether a trait was added</returns>
        public static bool GiveARandomTrait(Pawn pawn, MathTools.Fixed_Rand rand)
        {
            List<TraitDef> geneticTraits = PregnancyUtility.GetGeneticTraits();

            int attempts = 0;
            int maxAttempts = geneticTraits.Count;
            
            while(attempts++ < maxAttempts)
            {
                TraitDef def = (TraitDef) rand.Fixed_RandElement(geneticTraits);
                Trait trait2 = new Trait(def, PawnGenerator.RandomTraitDegree(def), false);
                if (ApplyTraitToPawn(pawn, trait2,rand, 1)) 
                    return true;                
            }

            return false;
        }

        /// <summary>
        /// This method will check trait conflict, already exist
        /// and no conflict will apply pawn trait
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="traitToGive"></param>
        /// <param name="rand"></param>
        /// <param name="chance"></param>
        /// <returns>Whether the trait was applied</returns>
        public static bool ApplyTraitToPawn(Pawn pawn, Trait traitToGive, MathTools.Fixed_Rand rand, double chance)
        {
            if ((traitToGive == null) || (rand.Fixed_RandDouble(0, 1) > chance) ||  pawn.story.traits.HasTrait(traitToGive.def)) 
                return false;

            foreach (Trait itrait in pawn.story.traits.allTraits)
            {
                if (traitToGive.def.ConflictsWith(itrait))
                    return false;
            }

            pawn.story.traits.GainTrait(traitToGive);
            return true;
        }

        /// <summary>
        /// This method will roll for a chance to make the pawn Asexual, Bisexual, or Gay
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <param name="rand">fixed random number generator</param>
        /// <returns>An AcquirableTrait for sexuality</returns>
        public static void GetNewTypeAndSexuality(Pawn pawn, MathTools.Fixed_Rand rand)
        {
            int traitsCount = pawn.story.traits.allTraits.Count;
            if (traitsCount >= BnCSettings.MAX_TRAIT_COUNT) return;
            
            if (rand.Fixed_RandChance(BnCSettings.GET_NEW_TYPE_CHANCE))
            {
                pawn.story.traits.GainTrait(new Trait(ChildTraitDefOf.Newtype, 0, true));
                Find.LetterStack.ReceiveLetter("Newtype".Translate(), 
                    "MessageNewtype".Translate(pawn.LabelIndefinite()), 
                    LetterDefOf.PositiveEvent, pawn);
            }

            if (rand.Fixed_RandChance(BnCSettings.GET_SPECIFIC_SEXUALITY))
            {
                pawn.story.traits.GainTrait(rand.Fixed_RandBool()
                    ? new Trait(TraitDefOf.Asexual, 0, true)
                    : new Trait(TraitDefOf.Bisexual, 0, true));
            }
            else if (rand.Fixed_RandChance(BnCSettings.GET_GAY_SEXUALITY))
            {
                pawn.story.traits.GainTrait(new Trait(TraitDefOf.Gay, 0, true));
            }
        }


        public static void RemoveChildDiedThought(Pawn pawn, Pawn child)
        {
            // Does the pawn have a "my child died thought"?
            MemoryThoughtHandler mems = pawn.needs.mood.thoughts.memories;
            if (mems.NumMemoriesOfDef(ThoughtDef.Named("MySonDied")) > 0 || mems.NumMemoriesOfDef(ThoughtDef.Named("MyDaughterDied")) > 0)
            {
                // Let's look through the list of memories
                foreach (Thought_Memory thought in mems.Memories.ToList())
                {
                    // Check if it's one of the right defs
                    if (thought.def == ThoughtDef.Named("MySonDied") || thought.def == ThoughtDef.Named("MyDaughterDied") || thought.def == ThoughtDef.Named("PawnWithGoodOpinionDied"))
                    {
                        // We found the thought
                        if (thought.otherPawn == child)
                        {
                            // Let's remove it
                            mems.Memories.Remove(thought);
                        }
                    }
                    if (thought.def == ThoughtDef.Named("WitnessedDeathFamily") || thought.def == ThoughtDef.Named("WitnessedDeathNonAlly"))
                    {
                        // Let's remove it
                        mems.Memories.Remove(thought);
                    }
                }
            }
        }
    }

}