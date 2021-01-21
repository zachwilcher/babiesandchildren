using RimWorld;
using Verse;
using Verse.Sound;
using Verse.AI;
using System.Collections.Generic;

namespace RimWorldChildren
{
	public class Hediff_UnhappyBaby : HediffWithComps
	{
		private bool CanBabyCry(){
			if (pawn.health.capacities.CapableOf (PawnCapacityDefOf.Breathing) && pawn.health.capacities.CanBeAwake)
				return true;
			else
				return false;
		}

		public void WhineAndCry()
		{
			if (!IsBabyHungry() && !IsBabyUnhappy() && !IsBabyHurt()) {
				pawn.health.RemoveHediff (this);
			} else if(CanBabyCry()){
				// Whine and cry
				MoteMaker.ThrowMetaIcon(pawn.Position, pawn.Map, ThingDefOf.Mote_IncapIcon);
				SoundInfo info = SoundInfo.InMap (new TargetInfo (pawn.PositionHeld, pawn.MapHeld));
                info.volumeFactor = BnCSettings.cryVolume;
                SoundDef.Named ("Pawn_BabyCry").PlayOneShot(info);
			}
		}

		private bool IsBabyHurt()
		{
			return pawn.health.HasHediffsNeedingTend();
		}

		private bool IsBabyHungry(){
			return pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry;
		}

		private bool IsBabyUnhappy(){
			return pawn.needs.joy.CurLevelPercentage < 0.2f;
		}
        
		public override void PostMake ()
		{
			WhineAndCry ();
			base.PostMake ();
		}

		public override void Tick()
		{
			if (pawn.Spawned) {
				if (pawn.IsHashIntervalTick (1000)) {
					LongEventHandler.ExecuteWhenFinished (delegate {
						WhineAndCry ();
					});
				}
			}
		}

		// Hide the hediff
		public override bool Visible {
			get {
				return false;
			}
		}
	}
}