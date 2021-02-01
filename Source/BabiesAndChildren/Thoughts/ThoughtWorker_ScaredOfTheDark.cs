using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    public class ThoughtWorker_ScaredOfTheDark : ThoughtWorker_Dark
    {
        protected override ThoughtState CurrentStateInternal (Pawn p)
        {
            // Make sure it only gets applied to kids
            if (!AgeStage.IsAgeStage(p, AgeStage.Child))
                return false;
            return p.Awake () && p.needs.mood.recentMemory.TicksSinceLastLight > 800;
        }
    }
}