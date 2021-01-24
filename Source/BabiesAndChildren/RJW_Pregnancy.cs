using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace BabiesAndChildren
{
    public class Hediff_Baby : HediffWithComps
    {
        public override void PostMake()
        {
            //if (!pawn.RaceProps.Humanlike) return;
            //if (pawn.def.defName != "Human")
            //{
            //    pawn.def.defName = "Human";
            //}

            //if (pawn.TryGetComp<Growing_Comp>() != null)
            //{
            //    NewBaby.BabyProcess(pawn);

            //    //pawn.story.traits.allTraits.RemoveRange(0, (pawn.story.traits.allTraits.Count));
            //    Growing_Comp comp = pawn.TryGetComp<Growing_Comp>();
            //    comp.Props.ColonyBorn = true;
            //    //comp.Props.geneticTraits = traitpool;
            //    comp.Initialize(true);
            //    //comp.ApplyGeneticTraits();
            //}

            base.PostMake();
        }

        public override void PostRemoved()
        {
            if (pawn.TryGetComp<Growing_Comp>() != null)
            {
                Pawn mother = pawn.GetMother();
                Pawn father = pawn.GetFather();
                if (mother != null)
                {
                    ChildrenUtility.Fixed_Rand rand = new ChildrenUtility.Fixed_Rand((int)mother.ageTracker.AgeBiologicalTicks);
                    mother.ageTracker.AgeBiologicalTicks += 2; // for twins
                    mother.ageTracker.AgeChronologicalTicks += 2;
                    if (rand.Fixed_RandChance(BnCSettings.STILLBORN_CHANCE))
                    {
                        NewBaby.Miscarry(pawn, mother, father);
                        return;
                    }
                    NewBaby.BabyProcess(pawn, mother, father, rand);
                    //pawn.story.traits.allTraits.RemoveRange(0, (pawn.story.traits.allTraits.Count));
                    Growing_Comp comp = pawn.TryGetComp<Growing_Comp>();
                    comp.Props.ColonyBorn = true;
                    //comp.Props.geneticTraits = traitpool;
                    comp.Initialize(true);
                    //comp.ApplyGeneticTraits();
                    pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("JustBorn"));
                }
            }
        }
    }
    public class NoFlag : HediffWithComps
    {
        public override void PostRemoved()
        {
            List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                SkillDef skillDef = allDefsListForReading[i];
                pawn.skills.Learn(skillDef, 100, true);
                CLog.DevMessage("Showbaby skill>> " + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));
            }
            if (pawn.story.traits.HasTrait(ChildTraitDefOf.Newtype))
            {
                pawn.skills.GetSkill(SkillDefOf.Shooting).Level += 5;
                pawn.skills.GetSkill(SkillDefOf.Melee).Level += 5;
            }
            if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
            {
                pawn.skills.GetSkill(SkillDefOf.Shooting).Level -= 4;
                pawn.skills.GetSkill(SkillDefOf.Melee).Level += 4;
            }
            //pawn.Notify_DisabledWorkTypesChanged();
            //pawn.skills.Notify_SkillDisablesChanged();
            //MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
            base.PostRemoved();
        }
    }
    public static class NewBaby
    {
        public static void BabyProcess(Pawn pawn, Pawn mother, Pawn father, ChildrenUtility.Fixed_Rand rand)
        {
            if (mother != null)
            {
                mother.health.AddHediff(HediffDef.Named("PostPregnancy"), null, null);
                mother.health.AddHediff(HediffDef.Named("Lactating"), mother.RaceProps.body.AllParts.Find(x => x.def.defName == "Torso"), null);
                if (ChildrenBase.ModRJW_ON && BnCSettings.enable_postpartum)
                {
                    mother.health.AddHediff(HediffDef.Named("BnC_RJW_PostPregnancy"), null, null);
                }

                //Make crying sound when baby is born
                SoundInfo info = SoundInfo.InMap(new TargetInfo(pawn.PositionHeld, pawn.MapHeld));
                SoundDef.Named("Pawn_BabyCry").PlayOneShot(info);

                ChildrenUtility.ChangeBodyType(pawn, true, false);
                ChildrenUtility.ClearImplantAndAddiction(pawn);
                ChildrenUtility.RenamePawn(pawn, mother, father);

                //For rabbie
                if (pawn.def.defName == "Rabbie")
                {
                    pawn.health.AddHediff(HediffDef.Named("PlanetariumAddiction"), null, null);
                }

                if (!ChildrenBase.ModCSL_ON)
                {
                    
                    SetBabyTraits(pawn, mother, father, rand);
                    SetBabySkillsAndPassions(pawn, mother, father, rand);
                }
            }
        }

        public static void Miscarry(Pawn baby, Pawn mother, Pawn father)
        {
            //baby.health.hediffSet.Clear();
            TaggedString t = "Unnamed".Translate();
            baby.Name = new NameSingle(t, false);
            baby.SetFaction(null, null);
            baby.health.AddHediff(HediffDef.Named("DefectStillborn"));

            if (father != null)
            {
                father.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("BabyStillborn"), baby);
                DeadBabyThoughts.RemoveChildDiedThought(father, baby);
            }
            mother.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("BabyStillborn"), baby);
            DeadBabyThoughts.RemoveChildDiedThought(mother, baby);
            Find.LetterStack.ReceiveLetter("WordStillborn".Translate(), TranslatorFormattedStringExtensions.Translate("MessageStillborn", mother.LabelIndefinite()), LetterDefOf.Death, mother);
        }

        public static void SetBabySkillsAndPassions(Pawn pawn, Pawn mother, Pawn father, ChildrenUtility.Fixed_Rand rand)
        {
            try
            {
                if (mother != null)
                {
                    List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;

                    //all skill to zero
                    //for (int i = 0; i < allDefsListForReading.Count; i++)
                    //{
                    //    SkillDef skillDef = allDefsListForReading[i];
                    //    SkillRecord skill = pawn.skills.GetSkill(skillDef);
                    //    //skill.Level = 0;
                    //    //skill.passion = Passion.None;
                    //}

                    if (BnCSettings.baby_Inherit_percentage == BnCSettings.BabyInheritPercentageHandleEnum._Random)
                    {
                        CLog.DevMessage(": Skills set random [GenerateSkills]");
                        try
                        {
                            Traverse.CreateWithType("PawnGenerator").Method("GenerateSkills", new object[]
                            {
                                pawn
                            }).GetValue();
                        }
                        catch
                        {
                        }
                        return;
                    }

                    for (int i = 0; i < allDefsListForReading.Count; i++)
                    {
                        SkillDef skillDef = allDefsListForReading[i];

                        int babyskill_level = 0;
                        int motherskill = mother.skills.GetSkill(skillDef).Level;
                        int fatherskill = 0;
                        CLog.DevMessage("mother's " + skillDef.defName + " Skills level =" + motherskill);

                        if (father != null)
                        {
                            fatherskill = father.skills.GetSkill(skillDef).Level;
                            CLog.DevMessage("father's " + skillDef.defName + " Skills level =" + fatherskill);
                            babyskill_level = (int)Math.Ceiling((double)(motherskill + fatherskill / 2));
                        }

                        switch (BnCSettings.baby_Inherit_percentage)
                        {
                            case BnCSettings.BabyInheritPercentageHandleEnum._None:
                                break;

                            case BnCSettings.BabyInheritPercentageHandleEnum._25:
                                babyskill_level = (int)Math.Floor((double)babyskill_level * 0.25);
                                break;

                            case BnCSettings.BabyInheritPercentageHandleEnum._50:
                                babyskill_level = (int)Math.Floor((double)babyskill_level * 0.5);
                                break;

                            case BnCSettings.BabyInheritPercentageHandleEnum._75:
                                babyskill_level = (int)Math.Floor((double)babyskill_level * 0.75);
                                break;

                            case BnCSettings.BabyInheritPercentageHandleEnum._90:
                                babyskill_level = (int)Math.Floor((double)babyskill_level * 0.90);
                                break;
                            case BnCSettings.BabyInheritPercentageHandleEnum._Random:

                            default:
                                break;
                        }

                        babyskill_level = (int)Math.Floor((double)babyskill_level * rand.Fixed_RandDouble(0.7, 1.1));
                        SkillRecord babyskill1 = pawn.skills.GetSkill(skillDef);
                        //babyskill1.Level = babyskill_level;
                        float sk_xp = 0;
                        for (int j = 1; j <= babyskill_level; j++)
                        {
                            sk_xp += j * 1000;
                        }
                        babyskill1.xpSinceLastLevel = sk_xp;

                        CLog.DevMessage("" + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));


                        if (pawn.story.traits.HasTrait(ChildTraitDefOf.Newtype))
                        {
                            if (skillDef == SkillDefOf.Shooting)
                            {
                                babyskill_level += 5;
                            }

                            if (skillDef == SkillDefOf.Melee)
                            {
                                babyskill_level += 5;
                            }
                        }

                        if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
                        {
                            if (skillDef == SkillDefOf.Shooting)
                            {
                                babyskill_level -= 4;
                            }

                            if (skillDef == SkillDefOf.Melee)
                            {
                                babyskill_level += 4;
                            }
                        }

                        if (!babyskill1.TotallyDisabled)
                        {

                            bool isconflictingPassion = false;
                            bool isforcedPassion = false;
                            foreach (Trait trait in pawn.story.traits.allTraits)
                            {
                                if (trait.def.ConflictsWithPassion(skillDef))
                                {
                                    isconflictingPassion = true;
                                    isforcedPassion = false;
                                    break;
                                }
                                if (trait.def.RequiresPassion(skillDef))
                                {
                                    isforcedPassion = true;
                                }
                            }
                            double num3 = (double)babyskill_level * 0.077;  //0.11 orginal
                            double passionChance = rand.Fixed_RandDouble(0, 1);
                            if (isforcedPassion || passionChance < num3 && !isconflictingPassion)
                            {
                                babyskill1.passion = ((passionChance >= num3 * 0.200000002980232) ? Passion.Minor : Passion.Major);
                            }
                            babyskill1.xpSinceLastLevel += (float)rand.Fixed_RandDouble(babyskill1.XpRequiredForLevelUp * 0.1, babyskill1.XpRequiredForLevelUp * 0.9);

                        }
                    }

                }
                else
                {
                    CLog.DevMessage(": No mother!");
                }

            }
            catch (Exception ex)
            {
                CLog.Error(": Skill set error: " + ex.Message);
            }
        }

        public static void SetBabyTraits(Pawn pawn, Pawn mother, Pawn father, ChildrenUtility.Fixed_Rand rand)
        {
            try
            {
                if (mother != null)
                {
                    //Fixed seed random variable
                    //ChildrenUtility.Fixed_Rand rand = new ChildrenUtility.Fixed_Rand(mother);
                    pawn.story.traits.allTraits.Clear();
                    if (BnCSettings.baby_Inherit_percentage == BnCSettings.BabyInheritPercentageHandleEnum._Random)
                    {

                        try
                        {
                            PawnGenerationRequest request = new PawnGenerationRequest(pawn.kindDef, pawn.Faction);
                            Traverse.CreateWithType("PawnGenerator").Method("GenerateTraits", new object[]
                            { pawn,request }).GetValue();
                        }
                        catch
                        {
                        }
                        return;
                    }
                    if (BnCSettings.baby_Inherit_percentage != BnCSettings.BabyInheritPercentageHandleEnum._None)
                    {
                        //add newtype and sexuality
                        GetNewtypeAndSexuality(pawn, rand);
                        int father_trait_count = father.story.traits.allTraits.Count;
                        CLog.DevMessage("Traits cleared for parent inherit");

                        List<Trait> father_trait_list = new List<Trait>();
                        if (father != null && father_trait_count > 0)
                        {
                            for (int i = 0; i < father_trait_count; i++)
                            {
                                father_trait_list.Add(father.story.traits.allTraits[i]);
                                CLog.DevMessage("Father trait[" + i + "]: " + father_trait_list[i].def);
                            }
                        }
                        else
                        {
                            CLog.DevMessage("Birth: Father is null or has no traits!");
                        }

                        int mother_trait_count = mother.story.traits.allTraits.Count;
                        List<Trait> mother_trait_list = new List<Trait>();
                        if (mother_trait_count > 0)
                        {
                            for (int i = 0; i < mother_trait_count; i++)
                            {
                                mother_trait_list.Add(mother.story.traits.allTraits[i]);
                                CLog.DevMessage("Mother trait[" + i + "]: " + mother_trait_list[i].def);
                            }
                        }

                        int maxt = BnCSettings.MAX_TRAIT_COUNT;
                        double inhertChance = 1;
                        switch (BnCSettings.baby_Inherit_percentage)
                        {

                            case BnCSettings.BabyInheritPercentageHandleEnum._90:
                                inhertChance = 0.9 + 0.1;
                                goto default;
                            case BnCSettings.BabyInheritPercentageHandleEnum._75:
                                inhertChance = 0.75 + 0.1;
                                goto default;
                            case BnCSettings.BabyInheritPercentageHandleEnum._50:
                                inhertChance = 0.5 + 0.1;
                                maxt -= 1;
                                goto default;

                            case BnCSettings.BabyInheritPercentageHandleEnum._25:
                                inhertChance = 0.25 + 0.1;
                                maxt -= 2;
                                goto default;

                            default:
                                Trait to_give_trait;
                                int got_countnow;
                                int to_get_trait_num;
                                int gotcountall = 0;

                                if (pawn.story.traits.allTraits.Count < BnCSettings.MAX_TRAIT_COUNT)
                                {
                                    if (mother_trait_count > 0)
                                    {
                                        got_countnow = 0;
                                        to_get_trait_num = rand.Fixed_RandInt(1, 2);

                                        for (int i = 0; (gotcountall < maxt) && got_countnow < to_get_trait_num && i < mother_trait_count; i++)
                                        {
                                            to_give_trait = mother_trait_list[rand.Fixed_RandInt(0, mother_trait_count--)];
                                            if (ApplyTraitToPawn(pawn, to_give_trait, rand, inhertChance))
                                            {
                                                got_countnow++;
                                                gotcountall++;
                                            }
                                        }
                                    }

                                    if (father_trait_count > 0 && pawn.story.traits.allTraits.Count < BnCSettings.MAX_TRAIT_COUNT)
                                    {
                                        got_countnow = 0;
                                        to_get_trait_num = rand.Fixed_RandInt(1, 2);

                                        for (int i = 0; (gotcountall < maxt) && (got_countnow < to_get_trait_num) && (i < father_trait_count); i++)
                                        {
                                            to_give_trait = father_trait_list[rand.Fixed_RandInt(0, father_trait_count--)];
                                            if (ApplyTraitToPawn(pawn, to_give_trait, rand, inhertChance + 0.2))
                                            {
                                                got_countnow++;
                                                gotcountall++;
                                            }
                                        }

                                    }
                                }
                                break;
                        }

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
                    else
                    {
                        //add newtype and sexuality
                        GetNewtypeAndSexuality(pawn, rand);
                    }
                }

            }
            catch (Exception ex3)
            {
                CLog.Error("Birth: Traits set error: " + ex3.Message);
            }
        }

        public static void GiveARandomTrait(Pawn pawn, ChildrenUtility.Fixed_Rand rand)
        {
            List<TraitDef> traitpool = PregnancyUtility.GetGeneticTraits();
            TraitDef to_give_trait;
            for (int i = 0; i < traitpool.Count; i++)
            {
                to_give_trait = traitpool[rand.Fixed_RandInt(0, traitpool.Count - 1)];
                int degree = PawnGenerator.RandomTraitDegree(to_give_trait);
                Trait trait2 = new Trait(to_give_trait, degree, false);
                if (ApplyTraitToPawn(pawn, trait2,rand, 1)) return;                
            }           
        }

        /// <summary>
        /// This method will check trait conflict, already exist
        /// and no conflict will apply pawn trait
        /// </summary>
        public static bool ApplyTraitToPawn(Pawn pawn, Trait trait_to_give, ChildrenUtility.Fixed_Rand rand, double chance)
        {
            if (rand.Fixed_RandDouble(0, 1) > chance) return false;

            if (trait_to_give != null && !pawn.story.traits.HasTrait(trait_to_give.def))
            {
                foreach (Trait itrait in pawn.story.traits.allTraits)
                {
                    if (trait_to_give.def.ConflictsWith(itrait))
                    {
                        return false;
                    }
                }
                pawn.story.traits.GainTrait(trait_to_give);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// This method will roll for a chance to make the pawn Asexual, Bisexual, or Gay
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>An AcquirableTrait for sexuality</returns>
        public static void GetNewtypeAndSexuality(Pawn pawn, ChildrenUtility.Fixed_Rand rand)
        {
            int traitcount = pawn.story.traits.allTraits.Count;
            if (traitcount < BnCSettings.MAX_TRAIT_COUNT)
            {

                if (rand.Fixed_RandChance(BnCSettings.GET_NEW_TYPE_CHANCE))
                {
                    pawn.story.traits.GainTrait(new Trait(ChildTraitDefOf.Newtype, 0, true));
                    Find.LetterStack.ReceiveLetter("Newtype".Translate(), TranslatorFormattedStringExtensions.Translate("MessageNewtype", pawn.LabelIndefinite()), LetterDefOf.PositiveEvent, pawn);
                }

                if (rand.Fixed_RandChance(BnCSettings.GET_SPECIFIC_SEXUALITY) && (traitcount < BnCSettings.MAX_TRAIT_COUNT))
                {
                    if (rand.Fixed_RandBool()) pawn.story.traits.GainTrait(new Trait(TraitDefOf.Asexual, 0, true));
                    else pawn.story.traits.GainTrait(new Trait(TraitDefOf.Bisexual, 0, true));

                }
                else if (rand.Fixed_RandChance(BnCSettings.GET_GAY_SEXUALITY) && (traitcount < BnCSettings.MAX_TRAIT_COUNT))
                {
                    pawn.story.traits.GainTrait(new Trait(TraitDefOf.Gay, 0, true));
                }
            }
        }


    }

}
