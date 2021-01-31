using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    /// <summary>
    /// class for BabyState HediffDef
    /// Added by Growing_Comp if pawn is a little baby
    /// </summary>
    public class Hediff_Baby : HediffWithComps
    {
        public override void PostRemoved()
        {
            var comp = pawn.TryGetComp<Growing_Comp>();
            if (comp == null) return;
            
            comp.GrowToStage(AgeStage.GetAgeStage(pawn));
            
            Pawn mother = pawn.GetMother();
            Pawn father = pawn.GetFather();
            MathTools.Fixed_Rand rand;
            if (mother != null)
            {
                
                rand = new MathTools.Fixed_Rand((int) mother.ageTracker.AgeBiologicalTicks);
                mother.ageTracker.AgeBiologicalTicks += 2; 
                mother.ageTracker.AgeChronologicalTicks += 2;
                
            }
            else if (father != null)
            {
                rand = new MathTools.Fixed_Rand((int) father.ageTracker.AgeBiologicalTicks);
                father.ageTracker.AgeBiologicalTicks += 2;
                father.ageTracker.AgeBiologicalTicks += 2;
            }
            else
            {
                rand = new MathTools.Fixed_Rand((int) pawn.ageTracker.AgeBiologicalTicks);
            }
            
            if (rand.Fixed_RandChance(BnCSettings.STILLBORN_CHANCE))
            {
                BabyTools.Miscarry(pawn, mother, father);
                return;
            }
            
            
            BabyTools.BabyProcess(pawn, mother, father, rand);
            comp.Props.ColonyBorn = true;
            
            //CLog.DevMessage("Calling Growing_Comp:Initialize from Hediff_Baby");
            //comp.Initialize(true);
            
            pawn.needs.mood.thoughts.memories.TryGainMemory(ThoughtDef.Named("JustBorn"));
        }

    }
}