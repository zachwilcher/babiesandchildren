using RimWorld;
using System;
using BabiesAndChildren.api;
using Verse;


namespace BabiesAndChildren
{
    /// <summary>
    /// This component is added and appropriately removed from pawns depending on their Age.
    /// It will keep track of a pawn's growth from birth to "adult" (see <see cref="AgeStage"/>).
    /// Adds and re-adds HeDiffs associated with pawn's age.
    ///
    /// See <see cref="Traits"/> for changing traits added at birth.
    /// </summary>
    class Growing_Comp : ThingComp
    {
        private int growthStage;
        private bool initialized;
        public Pawn pawn => (Pawn) parent;
        public CompProperties_Growing Props => (CompProperties_Growing) props;

        private static readonly Backstory Childhood_Disabled = BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"];
        private static readonly Backstory Rimchild = BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"];
        public override void PostExposeData()
        {
            Scribe_Values.Look(ref growthStage, "growthStage", 0);
            Scribe_Values.Look(ref initialized, "growthInitialized", false);

            base.PostExposeData();
        }



        /// <summary>
        /// Setup or reinitialize pawn growth settings
        /// </summary>
        public void Initialize(bool reinitialize = false)
        {
            if (initialized && !reinitialize) return;
            
            if (reinitialize)
            {
                CLog.DevMessage("Reinitializing: " + pawn.Name.ToStringShort);
            }
            else
            {
                CLog.DevMessage("Initializing: " + pawn.Name.ToStringShort);
            }
            
            
            if (pawn.ageTracker.AgeBiologicalTicks < 10000 && !pawn.health.hediffSet.HasHediff(HediffDef.Named("BabyState")))
            {
                //basically a call to Hediff_Baby:PostRemoved() in about 5 ticks
                BabyTools.TryAddHediff(pawn, HediffDef.Named("BabyState"));
                BabyTools.TryAddHediff(pawn, HediffDef.Named("NoManipulationFlag"));
            }

            initialized = true;

        }


        /// <summary>
        /// This method will handle all routine mood/hediff updates for this pawn.
        /// </summary>
        public void UpdateMood()
        {
            if(parent == null) return;
            
            if ((growthStage <= AgeStage.Toddler) &&
                Hediff_UnhappyBaby.CheckUnhappy(pawn))
            {
                BabyTools.TryAddHediff(pawn, ChildHediffDefOf.UnhappyBaby);
            }
        }

        /// <summary>
        /// When this child has grown fully, we no longer need this component and may therefor
        /// destroy it forever. This cleanup step will remove unnecessary ticks as well.
        /// </summary>
        public void Destroy()
        {
            if (parent == null) return;
            string name = pawn.Name.ToStringShort;
            CLog.DevMessage("Growing_Comp:GrowToStage attempting to destroy itself for: " + name);
            DestroyHediffs();
            parent.AllComps.Remove(this);
            //By removing this comp from AllComps
            //JecTools causes an out of bounds exception
            //Fixed this by adding a component that does nothing
            //allowing AllComps.Count to stay the same
            //... At least that's what I think is happening
            parent.AllComps.Add(new DummyComp());
            CLog.DevMessage("Growing_Comp removed for: " + name);
        }

        /// <summary>
        /// Removes hediffs added for non adult pawns that simulate child behavior.
        /// </summary>
        public void DestroyHediffs()
        {
            if (parent == null) return;
            
            if (pawn.health.hediffSet.HasHediff(ChildHediffDefOf.BabyState0))
            {
                pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.BabyState0));
            }
            if (pawn.health.hediffSet.HasHediff(ChildHediffDefOf.UnhappyBaby))
            {
                pawn.health.hediffSet.hediffs.Remove(pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.UnhappyBaby));
            }

            if (pawn.health.hediffSet.HasHediff(ChildHediffDefOf.NoManipulationFlag))
            {
                pawn.health.hediffSet.hediffs.Remove(
                    pawn.health.hediffSet.GetFirstHediffOfDef(ChildHediffDefOf.NoManipulationFlag));
            }
        }


        
        /// <summary>
        /// This method will setup all appropriate hediffs, bodytype and backstories for a give lifestage index
        /// </summary>
        /// <param name="stage">The new lifestage index</param>
        public void GrowToStage(int stage)
        {
            if (parent == null) return;
             
            growthStage = stage;
            
            Backstory currentBackstory = pawn.story.childhood;
            
            //If we are still a child, we will find the most appropriate hediff stage and update the severity to match
            if (growthStage < AgeStage.Adult)
            {
                DestroyHediffs();
                var growingHediff = HediffMaker.MakeHediff(ChildHediffDefOf.BabyState0, pawn);
                growingHediff.Severity = ChildHediffDefOf.BabyState0.stages[growthStage].minSeverity;
                pawn.health.AddHediff(growingHediff);
            }

            //update bodytype and backstory
            switch (growthStage)
            {
                case AgeStage.Baby:
                    pawn.story.childhood = Childhood_Disabled;
                    ChildrenUtility.ChangeBodyType(pawn, true, false);
                    break;
                case AgeStage.Toddler:
                    pawn.story.childhood = Childhood_Disabled;
                    ChildrenUtility.ChangeBodyType(pawn, false, false);
                    break;
                case AgeStage.Child:
                {
                    if (currentBackstory == Childhood_Disabled)
                    {
                        pawn.story.childhood = Rimchild;
                        
                        pawn.Notify_DisabledWorkTypesChanged();
                        pawn.skills.Notify_SkillDisablesChanged();
                        
                        MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                        
                        if (pawn.Faction.IsPlayer)
                            Messages.Message("MessageGrewUpChild".Translate(pawn.Name.ToStringShort), MessageTypeDefOf.PositiveEvent);
                        ChildrenUtility.ChangeBodyType(pawn, false, false);
                    }
                    else ChildrenUtility.ChangeBodyType(pawn, true, false);

                    ChildrenUtility.TryDrop(pawn, "BabyGear");
                    break;
                }
                case AgeStage.Teenager:
                {
                    if (currentBackstory == Childhood_Disabled)
                    {
                        pawn.story.childhood = Rimchild;
                        pawn.Notify_DisabledWorkTypesChanged();
                        pawn.skills.Notify_SkillDisablesChanged();
                        
                        MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                        
                    }

                    if (currentBackstory == Rimchild)
                    {
                        ChildrenUtility.ChangeBodyType(pawn, false, false);
                        if (pawn.Faction.IsPlayer)
                            Messages.Message("MessageGrewUpTeenager".Translate(pawn.Name.ToStringShort),
                                MessageTypeDefOf.PositiveEvent);
                    }

                    ChildrenUtility.TryDrop(pawn, "Toy");
                    break;
                }
                //>= AgeStage.Adult
                default:
                    
                    if (currentBackstory == Rimchild)
                    {
                        ChildrenUtility.ChangeBodyType(pawn, false, false);
                    }
                    
                    Destroy();
                    break;
            }
        }


        public override void CompTickRare()
        {
            if(parent == null) return;
            
            if (!pawn.Spawned || pawn.Dead) return;
            
            //Doing this here instead of PostSpawnSetup due to an odd null reference in MakeDowned when adding a hediff during setup
            //This is one way to ensure this only executes once the pawn is truly spawned
            Initialize();

            bool graphicsDirty = false;
            
            int age = pawn.ageTracker.AgeBiologicalYears;
            
            int agestage = AgeStage.GetAgeStage(pawn);
            
            if (growthStage != agestage)
            {
                graphicsDirty = true;
                PortraitsCache.SetDirty(pawn);
                GrowToStage(agestage);
            }

            if (agestage == AgeStage.Toddler)
            {
                if (pawn.story.bodyType != BodyTypeDefOf.Thin)
                {
                    if (ChildrenUtility.ToddlerIsUpright(pawn))
                    {
                        ChildrenUtility.ChangeBodyType(pawn);
                        graphicsDirty = true;
                    }
                }
            }
            
            UpdateMood();
            

            //Accelerated Growth Factor
            if (BnCSettings.accelerated_growth && (age < BnCSettings.accelerated_growth_end_age))
            {
                long acceleratedFactor = ChildrenUtility.SettingAcceleratedFactor(growthStage);
                //Can the biological age be greater than the chronological age?
                pawn.ageTracker.AgeBiologicalTicks += (acceleratedFactor) * 250;
                pawn.ageTracker.AgeChronologicalTicks += (acceleratedFactor) * 250;
            }
            

            if (graphicsDirty)
            {
                pawn.Drawer.renderer.graphics.ResolveAllGraphics(); 
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
        public bool ColonyBorn { get; set; }
        public CompProperties_Growing()
        {
            this.compClass = typeof(Growing_Comp);
        }
        public CompProperties_Growing(Type compClass) : base(compClass)
        {
        }

    }
}