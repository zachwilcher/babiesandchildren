using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    public class Hediff_Baby : HediffWithComps
    {
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
                        BabyTools.Miscarry(pawn, mother, father);
                        return;
                    }
                    BabyTools.BabyProcess(pawn, mother, father, rand);
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
}