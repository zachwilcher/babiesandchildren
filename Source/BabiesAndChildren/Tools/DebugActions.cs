﻿using RimWorld;
using System.Linq;
using BabiesAndChildren.Tools;
using Verse;

namespace BabiesAndChildren
{
    public static class DebugActions
    {
        
        [DebugAction("Pawns", "Reinitialize Children", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void Reinitialize()
        {
            ChildrenBase.ReinitializeChildren(Current.Game.CurrentMap);
        }

        [DebugAction("Pawns", "Change Child Backstory", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ChangeChildBackstory()
        {
            foreach (Thing thing in UI.MouseCell().GetThingList(Find.CurrentMap).ToList<Thing>())
            {
                Pawn pawn = thing as Pawn;
                if (pawn != null)
                {
                    if (RaceUtility.PawnUsesChildren(pawn) && AgeStage.GetAgeStage(pawn) < AgeStage.Teenager)
                    {
                        StoryUtility.ChangeChildBackstory(pawn);
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
                    if (RaceUtility.PawnUsesChildren(pawn) && AgeStage.GetAgeStage(pawn) < AgeStage.Child)
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