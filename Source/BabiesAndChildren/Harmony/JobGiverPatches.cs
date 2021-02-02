using BabiesAndChildren.Tools;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace BabiesAndChildren.Harmony
{
    [HarmonyPatch(typeof(JobGiver_SocialFighting), "TryGiveJob")]
    internal static class JobGiver_SocialFighting_TryGiveJob_Patch
    {
        [HarmonyPostfix]
        static void Postfix(ref Pawn pawn, ref Job __result){
            Pawn other = ((MentalState_SocialFighting)pawn.MentalState).otherPawn;
            if (__result == null) return;
            
            if (AgeStage.IsYoungerThan(pawn, AgeStage.Child) && RaceUtility.PawnUsesChildren(pawn))
            {
                __result = null;
            }
            // Make sure kids don't start social fights with adults
            if (AgeStage.GetAgeStage(other) > AgeStage.Child && AgeStage.GetAgeStage(pawn) <= AgeStage.Child) {
                CLog.DevMessage("Child starting social fight with adult");
                // Adult will "start" the fight, following the code below
                other.interactions.StartSocialFight (pawn);
                __result = null;
            }

            // Make sure adults don't start social fights with kids (unless psychopaths)
            if (!AgeStage.IsOlderThan(other, AgeStage.Child) && 
                AgeStage.IsOlderThan(pawn, AgeStage.Child) && 
                !pawn.story.traits.HasTrait (TraitDefOf.Psychopath)) {
                
                CLog.DevMessage("Adult starting social fight with child");
                // If the pawn is not in a bad mood or is kind, they'll just tell them off
                if (pawn.story.traits.HasTrait (TraitDefOf.Kind) || pawn.needs.mood.CurInstantLevel > 0.45f || pawn.WorkTagIsDisabled(WorkTags.Violent)) {
                    //Log.Message ("Debug: Adult has decided to tell off the child");
                    JobDef chastise = DefDatabase<JobDef>.GetNamed ("ScoldChild", true);
                    __result = new Job (chastise, other);
                }
                // Otherwise the adult will smack the child around
                else if (other.health.summaryHealth.SummaryHealthPercent > 0.93f) {
                    //Log.Message ("Debug: Adult has decided to smack the child around, child health at " + other.health.summaryHealth.SummaryHealthPercent);
                    JobDef paddlin = DefDatabase<JobDef>.GetNamed ("DisciplineChild", true);
                    __result = new Job (paddlin, other);
                }

                pawn.MentalState.RecoverFromState ();
                __result = null;
            }
        }
    }
}