using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorldChildren
{

    // Play with baby
    //public class Hediff_GiveJobTest : HediffWithComps
    //{
    //    public override void Tick()
    //    {
    //        Pawn baby = null;
    //        // Try to find a baby
    //        foreach (Pawn colonist in pawn.Map.mapPawns.FreeColonists)
    //        {
    //            if (ChildrenUtility.GetAgeStage(colonist) == 0)
    //                baby = colonist;
    //        }
    //        if (baby != null)
    //        {
    //            Job playJob = new Job(DefDatabase<JobDef>.GetNamed("BreastFeedBaby"), baby);
    //            //pawn.QueueJob (playJob);
    //            pawn.jobs.StartJob(playJob, JobCondition.InterruptForced, null, true, true, null, null);
    //            Log.Message("Found baby " + baby.Name.ToStringShort + " and proceeding to breastfeed.");
    //        }
    //        else
    //            Log.Message("Failed to find any baby.");

    //        pawn.health.RemoveHediff(this);
    //    }
    //}

    //public static class DebugToolsChildren {
    //    [DebugAction("Pawns", null, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
    //    private static void AdvancePregnancy() {
    //        foreach (Thing thing in UI.MouseCell().GetThingList(Find.CurrentMap).ToList<Thing>()) {
    //            Pawn pawn = thing as Pawn;
    //            if (pawn != null) {
    //                Hediff preg = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("HumanPregnancy"));
    //                if (preg != null) {
    //		if(preg.Severity < .66f)
    //		{
    //			preg.Severity += .33f;
    //		}
    //		else if (preg.Severity < .75f)
    //		{
    //			preg.Severity = .76f;
    //		}
    //		else
    //		{
    //			preg.Severity = .99f;
    //		}
    //                }

    //            }
    //        }
    //    }
    //}

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

        //[DebugAction("Mods", "Change AllChild RjwBodypart", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        //private static void ChangeAllChildBodypart()
        //{
        //    foreach (Map map in Current.Game.Maps)
        //    {
        //        foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
        //        {
        //            if (ChildrenUtility.RaceUsesChildren(pawn) && ChildrenUtility.GetAgeStage(pawn) < AgeStage.Teenager) ChildrenUtility.ChangeBodyType(pawn, true, false);
        //        }
        //    }
        //}
    }
}