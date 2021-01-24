using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;

namespace BabiesAndChildren
{

    public class WorkGiver_TakeBabyToCrib : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode {
            get {
                return PathEndMode.Touch;
            }
        }
        public override ThingRequest PotentialWorkThingRequest {
            get {
                return ThingRequest.ForGroup (ThingRequestGroup.Pawn);
            }
        }
       
        public override bool HasJobOnThing (Pawn pawn, Thing t, bool forced = false)
        {
            if (t == null || t == pawn) {
                return false;
            }

            Pawn baby = t as Pawn;
            if (ChildrenUtility.GetAgeStage(baby) > AgeStage.Baby || !ChildrenUtility.RaceUsesChildren(baby) ) {
                return false;
            }
            if (!pawn.CanReserveAndReach (t, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, forced)) {
                return false;
            }
           
            //if(ChildrenUtility.GetAgeStage(baby) == AgeStage.Toddler){
            //    return false;
            //}
            Building_Bed crib = RestUtility.FindBedFor(baby, pawn, false, false);
            if (crib == null) {
                JobFailReason.Is("NoCrib".Translate());
                return false;
            }
            //If not in a crib, but can't find a crib to go to
            if (baby.InBed() && (ChildrenUtility.IsBedCrib(baby.CurrentBed()) || !ChildrenUtility.IsBedCrib(crib))) {
                return false;
            }
            // Is it time for the baby to go to bed?
            bool baby_sleep_time = baby.timetable.GetAssignment(GenLocalDate.HourInteger(baby.Map)).allowRest;
            if(!baby_sleep_time){
                return false;
            }           
            
            return true;
        }
        
        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var baby = (Pawn)t;
            Building_Bed crib = RestUtility.FindBedFor(baby, pawn, false, false);
            if (baby != null && crib != null){
                Job carryBaby = new Job(DefDatabase<JobDef>.GetNamed("TakeBabyToCrib"), baby, crib){ count = 1 };
                return carryBaby;
            }
            return null;
        }
    }
}