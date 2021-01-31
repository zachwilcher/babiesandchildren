using BabiesAndChildren.api;
using BabiesAndChildren.Tools;
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

    public class ThoughtWorker_NearParents : ThoughtWorker
    {
        const int maxDist = 8;
        protected override ThoughtState CurrentStateInternal (Pawn p)
        {
            if (AgeStage.IsOlderThan(p, AgeStage.Toddler) || !RaceUtility.PawnUsesChildren(p))
                return false;
            Pawn mother = p.relations.GetFirstDirectRelationPawn (PawnRelationDefOf.Parent, x => x.gender == Gender.Female);
            Pawn father = p.relations.GetFirstDirectRelationPawn (PawnRelationDefOf.Parent, x => x.gender == Gender.Male);
            if (ArePawnsNear(p, mother) && ArePawnsNear(p, father)){
                return ThoughtState.ActiveAtStage(2);
            }
            else if(ArePawnsNear(p, mother))
            {
                return ThoughtState.ActiveAtStage(0);
            }
            else if(ArePawnsNear(p, father))
            {
                return ThoughtState.ActiveAtStage(1);
            }
            else
            {
                return false;
            }
        }

        protected bool ArePawnsNear(Pawn a, Pawn b)
        {
            if (a == null || b == null) return false;

            return (a.GetRoom() == b.GetRoom() && 
                    a.Position.DistanceTo(b.Position) < maxDist);
        }
    }
}