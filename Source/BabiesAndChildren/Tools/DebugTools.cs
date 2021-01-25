using RimWorld;
using System.Linq;
using Verse;

namespace BabiesAndChildren
{


    public static class DebugToolsChildren
    {
        [DebugAction("Pawns", "Change Child Backstory", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ChangeChildBackstory()
        {
            foreach (Thing thing in UI.MouseCell().GetThingList(Find.CurrentMap).ToList<Thing>())
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null)
                {
                    if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager)
                    {
                        ChildrenUtility.ChangeChildBackstory(pawn);
                    }
                }
            }
        }

        [DebugAction("Pawns", "Change Baby Backstory", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ChangeBabyBackstory()
        {
            foreach (Thing thing in UI.MouseCell().GetThingList(Find.CurrentMap).ToList<Thing>())
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null)
                {
                    if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Child)
                    {
                        if (pawn.story.childhood == BackstoryDatabase.allBackstories["CustomBackstory_NA_Childhood_Disabled"])
                        {
                            pawn.story.childhood = BackstoryDatabase.allBackstories["CustomBackstory_Rimchild"];
                            pawn.Notify_DisabledWorkTypesChanged();
                            pawn.skills.Notify_SkillDisablesChanged();
                            MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
                        }
                        else Messages.Message("Choose a child who has a backstory 'Baby'", MessageTypeDefOf.NeutralEvent);
                    }
                    else Messages.Message("Choose a baby ", MessageTypeDefOf.NeutralEvent);
                }
            }
        }

    }
}