using RimWorld;
using System.Linq;
using Verse;

namespace RimWorldChildren {
    /// <summary>
    /// This class will be used to determine if a child pawn deserves certain traits based
    /// on life milestones and records.
    /// Bear in mind that these traits are most likely awarded at age 13, and therefore have a ton of potential time to work on these record counts.
    /// A trait being generated here also only qualifies the pawn for that trait.
    /// </summary>
    public static class GenMilestoneTrait {
        //TODO: Need a reliable way for others to extend here and add additional functions without having to patch

        /// <summary>
        /// This method will analyze the provided pawn against all configured milestone rules
        /// and generate a list of potential traits for that pawn to develop.
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>A traitpool containing zero or more traits</returns>
        public static TraitPool GetMilestoneTraits(Pawn pawn) {
            TraitPool pool = new TraitPool();
            GetCombatMilestones(pawn).Where(trait => !pool.Contains(trait)).ToList().ForEach(trait => pool.Add(trait));
            GetMentalMilestones(pawn).Where(trait => !pool.Contains(trait)).ToList().ForEach(trait => pool.Add(trait));
            GetSocialMilestones(pawn).Where(trait => !pool.Contains(trait)).ToList().ForEach(trait => pool.Add(trait));
            GetWorkMilestones(pawn).Where(trait => !pool.Contains(trait)).ToList().ForEach(trait => pool.Add(trait));
            return pool;
        }


        /// <summary>
        /// This method will generate a list of applicable traits based on the pawn's combat experience
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>A traitpool containing zero or more traits</returns>
        public static TraitPool GetCombatMilestones(Pawn pawn) {
            TraitPool pool = new TraitPool();

            if (pawn.records.GetValue(RecordDefOf.ShotsFired) > 1000 && (int)pawn.records.GetValue(RecordDefOf.PawnsDowned) == 0) {
                //Pawn shot a lot, but never managed to take anyone down
                pool.Add(new AcquirableTrait(TraitDefOf.ShootingAccuracy, -1, TraitDefOf.ShootingAccuracy.GetGenderSpecificCommonality(pawn.gender)));
            }
            else if (pawn.records.GetValue(RecordDefOf.ShotsFired) > 1000) {
                pool.Add(new AcquirableTrait(TraitDefOf.ShootingAccuracy, 1, TraitDefOf.ShootingAccuracy.GetGenderSpecificCommonality(pawn.gender)));
            }
            else if (pawn.records.GetValue(RecordDefOf.ShotsFired) < 100 && pawn.records.GetValue(RecordDefOf.PawnsDowned) > 1) {
                pool.Add(new AcquirableTrait(TraitDefOf.Brawler, 0, TraitDefOf.ShootingAccuracy.GetGenderSpecificCommonality(pawn.gender)));
            }
            return pool;
        }

        /// <summary>
        /// This method will generate a list of applicable traits based on the pawn's social interactions
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>A traitpool containing zero or more traits</returns>
        public static TraitPool GetSocialMilestones(Pawn pawn) {
            TraitPool pool = new TraitPool();
            
            int male_rivals = 0;
            int female_rivals = 0;
            foreach (Pawn colonist in Find.AnyPlayerHomeMap.mapPawns.AllPawnsSpawned) {
                if (pawn.relations.OpinionOf(colonist) <= -20) {
                    if (colonist.gender == Gender.Male) {
                        male_rivals++;
                    }
                    else {
                        female_rivals++;
                    }
                }
            }
            if (male_rivals > 3) {
                pool.Add(new AcquirableTrait(TraitDefOf.DislikesMen, 0, TraitDefOf.DislikesMen.GetGenderSpecificCommonality(pawn.gender)));
            }
            if (female_rivals > 3) {
                pool.Add(new AcquirableTrait(TraitDefOf.DislikesWomen, 0, TraitDefOf.DislikesWomen.GetGenderSpecificCommonality(pawn.gender)));
            }

            if (pawn.records.GetValue(RecordDefOf.TimesTendedOther) > 200 && (int)pawn.records.GetValue(RecordDefOf.Kills) < 50) {
                pool.Add(new AcquirableTrait(TraitDefOf.Kind, 0, TraitDefOf.Kind.GetGenderSpecificCommonality(pawn.gender)));
            }
            return pool;
        }

        /// <summary>
        /// This method will generate a list of applicable traits based on the pawn's mental condition
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>A traitpool containing zero or more traits</returns>
        public static TraitPool GetMentalMilestones(Pawn pawn) {
            TraitPool pool = new TraitPool();
            float psychoticMultiplier = 0.045f;
            float kills = pawn.records.GetValue(RecordDefOf.KillsAnimals) + pawn.records.GetValue(RecordDefOf.KillsHumanlikes);
            if (kills > 50) {
                pool.Add(new AcquirableTrait(TraitDefOf.Psychopath, 0, kills * psychoticMultiplier));
            }

            float mentalStates = pawn.records.GetValue(RecordDefOf.TimesInMentalState);
            if (mentalStates < 10) {
                pool.Add(new AcquirableTrait(TraitDefOf.Nerves, 2, TraitDefOf.Nerves.GetGenderSpecificCommonality(pawn.gender)));
            }
            else if (mentalStates < 20) {
                pool.Add(new AcquirableTrait(TraitDefOf.Nerves, 1, TraitDefOf.Nerves.GetGenderSpecificCommonality(pawn.gender)));
            }
            else if (mentalStates < 40) {
                pool.Add(new AcquirableTrait(TraitDefOf.Nerves, -1, TraitDefOf.Nerves.GetGenderSpecificCommonality(pawn.gender)));
            }
            else {
                pool.Add(new AcquirableTrait(TraitDefOf.Nerves, -2, TraitDefOf.Nerves.GetGenderSpecificCommonality(pawn.gender)));
            }
            return pool;
        }

        /// <summary>
        /// This method will generate a list of applicable traits based on the pawn's work history
        /// </summary>
        /// <param name="pawn">The pawn to be analyzed</param>
        /// <returns>A traitpool containing zero or more traits</returns>
        public static TraitPool GetWorkMilestones(Pawn pawn) {
            TraitPool pool = new TraitPool();
            if (pawn.records.GetValue(RecordDefOf.CellsMined) > 500) {
                pool.Add(new AcquirableTrait(TraitDefOf.Undergrounder, 0, TraitDefOf.Undergrounder.GetGenderSpecificCommonality(pawn.gender)));
            }
            return pool;
        }

    }
}
