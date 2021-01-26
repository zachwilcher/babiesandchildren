using RimWorld;
using System;
using Verse;


namespace BabiesAndChildren
{
    /// <summary>
    /// This component will keep track of a pawn's growth from birth to "adult."
    /// By using a component instead of a hediff, we have the added advantage of tracking state
    /// in a less volatile manner, as our state object is no longer subject to other mods
    /// removing the state hediff. If our expected hediffs are removed, this comp can readd them as necessary.
    /// 
    /// potentialTraits are traits that have been submitted by one mod or another to be randomly selected from
    /// when the pawn matures to "adult" stage.
    /// 
    /// requiredTraits are traits that have been submitted by one mod or another to be forced upon the pawn
    /// when the pawn matures. These will still respect normal trait rules, however. Required traits will
    /// be added to the pawn starting with the highest weight.
    /// 
    /// geneticTraits are only applied to a pawn at stage 0 - when they are first born. These traits
    /// are applied and never saved.
    /// </summary>
    class Growing_Comp : ThingComp
    {
        private int growthStage;
        private int acceleratedFactor;
        //private bool colonyBorn;
        private bool initialized;

        public CompProperties_Growing Props
        {
            get
            {
                return (CompProperties_Growing)props;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref growthStage, "growthStage", 0);
            Scribe_Values.Look(ref initialized, "growthInitialized", false);
            //Scribe_Values.Look(ref colonyBorn, "colonyBorn", false);

            base.PostExposeData();
        }

        /// <summary>
        /// Setup or reinitialize pawn growth settings
        /// </summary>
        public void Initialize(bool reinitialize)
        {
            CLog.DevMessage("Executing GrowingComp Initialize: reinit = " + reinitialize);
            if (!initialized || reinitialize)
            {
                Pawn pawn = (Pawn)parent;
                initialized = true;
                growthStage = ChildrenUtility.GetAgeStage(pawn);
                //colonyBorn = Props.ColonyBorn;
                //if (reinitialize)
                //{
                // Clean out randomly generated drug addictions
                //  pawn.health.hediffSet.Clear();
                //}
                DestroyHediffs();
                GrowToStage(growthStage);
            }

        }


        /// <summary>
        /// This method will update an infant pawn's mood/hediff based on predetermined criteria
        /// </summary>
        protected void UpdateInfantMood()
        {
            Pawn pawn = (Pawn)parent;
            if (pawn.needs.food != null && (pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry ||
                                            pawn.needs.joy?.CurLevelPercentage < 0.1f) || pawn.health.HasHediffsNeedingTend() && !pawn.health.hediffSet.HasHediff(ChildHediffDefOf.UnhappyBaby))
            {
                pawn.health.AddHediff(ChildHediffDefOf.UnhappyBaby, null, null);
            }
        }

        /// <summary>
        /// This method will handle all routine mood/hediff updates for this pawn.
        /// </summary>
        public void UpdateMood()
        {
            if (growthStage <= AgeStage.Toddler)
            {
                UpdateInfantMood();
            }
        }

        /// <summary>
        /// When this child has grown fully, we no longer need this component and may therefor
        /// destroy it forever. This cleanup step will remove unnecessary ticks as well.
        /// </summary>
        public void Destroy()
        {
            DestroyHediffs();
            if (this != null)
            {
                parent.AllComps.Remove(this);
                //By removing this comp from AllComps
                //JecTools causes an out of bounds exception
                //Fixed this by adding a component that does nothing
                //allowing AllComps.Count to stay the same
                //... At least that's what I think is happening
                parent.AllComps.Add(new DummyComp());
            }
        }

        /// <summary>
        /// Remove any hediffs that were added in order to simulate child growth.
        /// Birth defects and similar hediffs will not be removed.
        /// </summary>
        public void DestroyHediffs()
        {
            Pawn pawn = (Pawn)parent;
            if (pawn.health.hediffSet.HasHediff(ChildHediffDefOf.BabyState0))
            {
                pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.BabyState0));
            }
            if (pawn.health.hediffSet.HasHediff(ChildHediffDefOf.UnhappyBaby))
            {
                pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.UnhappyBaby));
            }
        }

        /// <summary>
        /// This method will setup all appropriate hediffs and other settings for a given life stage index.
        /// </summary>
        /// <param name="stage">The new lifestage index</param>
        public void GrowToStage(int stage)
        {
            Pawn pawn = (Pawn)parent;
            growthStage = stage;
            Hediff growingHediff;
            PortraitsCache.SetDirty(pawn);
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                pawn.Drawer.renderer.graphics.ResolveAllGraphics();
            });

            //If we are still a child, we will find the most appropriate hediff stage and update the severity to match
            if (growthStage < AgeStage.Adult)
            {

                if (!pawn.health.hediffSet.HasHediff(ChildHediffDefOf.BabyState0))
                {
                    growingHediff = HediffMaker.MakeHediff(ChildHediffDefOf.BabyState0, pawn);
                    growingHediff.Severity = ChildHediffDefOf.BabyState0.stages[growthStage].minSeverity;
                    pawn.health.AddHediff(growingHediff);
                }
                else
                {
                    growingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.BabyState0);
                }
                growingHediff.Severity = ChildHediffDefOf.BabyState0.stages[growthStage].minSeverity;
                if (growthStage == AgeStage.Baby)
                {                    
                    pawn.story.childhood = BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"];
                    ChildrenUtility.ChangeBodyType(pawn, true, false);
                }
                else if (growthStage == AgeStage.Toddler)
                {                    
                    ChildrenUtility.ChangeBodyType(pawn, false, false);
                }
                else if (growthStage == AgeStage.Child)
                {                    
                    if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"])
                    {
                        pawn.story.childhood = BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"];
                        pawn.Notify_DisabledWorkTypesChanged();
                        pawn.skills.Notify_SkillDisablesChanged();
                        MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                        if (pawn.Faction.IsPlayer) Messages.Message("MessageGrewUpChild".Translate(pawn.Name.ToStringShort), MessageTypeDefOf.PositiveEvent);
                        ChildrenUtility.ChangeBodyType(pawn, false, false);
                    }
                    else ChildrenUtility.ChangeBodyType(pawn, true, false);
                    ChildrenUtility.TryDrop(pawn, "BabyGear");
                }
                else if (growthStage == AgeStage.Teenager)
                {                    
                    if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"])
                    {
                        pawn.story.childhood = BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"];
                        pawn.Notify_DisabledWorkTypesChanged();
                        pawn.skills.Notify_SkillDisablesChanged();
                        MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                    }
                    if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"])
                    {
                        ChildrenUtility.ChangeBodyType(pawn, false, false);
                        if (pawn.Faction.IsPlayer) Messages.Message("MessageGrewUpTeenager".Translate(pawn.Name.ToStringShort), MessageTypeDefOf.PositiveEvent);
                    }                    
                    ChildrenUtility.TryDrop(pawn, "Toy");
                }
            }
            else
            {
                if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"])
                {
                    ChildrenUtility.ChangeBodyType(pawn, false, false);
                }
                Destroy();
            }
        }

        public override void CompTickRare()
        {
            Pawn pawn = (Pawn)parent;
            int age = pawn.ageTracker.AgeBiologicalYears;
            int agestage = ChildrenUtility.GetAgeStage(pawn);
            if (parent.Spawned && !pawn.Dead)
            {
                if (!initialized)
                {
                    if (pawn.ageTracker.AgeBiologicalTicks < 10000 && !pawn.health.hediffSet.HasHediff(HediffDef.Named("BabyState")))
                    {
                        pawn.health.hediffSet.Clear();
                        pawn.health.AddHediff(HediffDef.Named("BabyState"));
                        pawn.health.AddHediff(HediffDef.Named("NoManipulationFlag"), null, null);
                    }

                    //Doing this here instead of PostSpawnSetup due to an odd null reference in MakeDowned when adding a hediff during setup
                    //This is one way to ensure this only executes once the pawn is truly spawned
                    Initialize(false);
                }

                if (agestage == AgeStage.Toddler)
                {
                    if (pawn.story.bodyType != BodyTypeDefOf.Thin)
                    {
                        if (ChildrenUtility.ToddlerIsUpright(pawn))
                        {
                            ChildrenUtility.ChangeBodyType(pawn, false, false);
                            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                        }
                    }                  
                }
                UpdateMood();
                if (growthStage < agestage)
                {
                    GrowToStage(agestage);
                }

                //Accelerated Growth Factor
                if (BnCSettings.accelerated_growth && (age < BnCSettings.accelerated_growth_end_age))
                {
                    acceleratedFactor = ChildrenUtility.SettingAcceleratedFactor(growthStage);
                    pawn.ageTracker.AgeBiologicalTicks += (long)(acceleratedFactor) * 250;
                    pawn.ageTracker.AgeChronologicalTicks += (long)(acceleratedFactor) * 250;
                }
            }
        }

    }

    /// <summary>
    /// This properties class is necessary to reference Growing_Comp when adding the comp to a child, but
    /// is otherwise not used.
    /// geneticTraits may be set before a call to initialize, and the traits will be applied 
    /// to a new born pawn on initialization based on random weight.
    /// </summary>
    public class CompProperties_Growing : CompProperties
    {
        public bool ColonyBorn;
        //public TraitPool geneticTraits;

        public CompProperties_Growing()
        {
            this.compClass = typeof(Growing_Comp);
        }
        public CompProperties_Growing(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}