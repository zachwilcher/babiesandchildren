using RimWorld;
using System.Linq;
using Verse;

namespace BabiesAndChildren
{
    //public class BabyStillborn : Thought_Memory
    //{
    //    public override void Init ()
    //    {
    //        base.Init ();

    //        CLog.DevMessage("Thought_memory" + this.def);

    //        DeadBabyThoughts.RemoveChildDiedThought (pawn, otherPawn);
    //    }
    //}

    internal static class DeadBabyThoughts
    {

        internal static void RemoveChildDiedThought(Pawn pawn, Pawn child)
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