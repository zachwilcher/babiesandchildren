using System.Collections.Generic;
using RimWorld;

namespace BabiesAndChildren.api
{
    /// <summary>
    /// Blacklist thoughts that babies and toddlers can have.
    /// </summary>
    public static class Thoughts
    {
        private static List<ThoughtDef> thoughtBlacklist = new List<ThoughtDef>
        {
                ThoughtDefOf.AteWithoutTable,
                ThoughtDefOf.KnowPrisonerDiedInnocent,
                ThoughtDefOf.KnowPrisonerSold,
                ThoughtDefOf.Naked,
                ThoughtDefOf.SleepDisturbed,
                ThoughtDefOf.SleptOnGround,
                ThoughtDef.Named("NeedOutdoors"),
                ThoughtDef.Named("SleptInBarracks"),
                ThoughtDef.Named("Expectations")
        };


        public static bool IsBlacklisted(ThoughtDef thoughtDef)
        {
            return thoughtBlacklist.Contains(thoughtDef);
        }

        public static bool Blacklist(ThoughtDef thoughtDef)
        {
            if (IsBlacklisted(thoughtDef))
                return false;
            
            thoughtBlacklist.Add(thoughtDef);
            return true;

        }

        public static bool UnBlacklist(ThoughtDef thoughtDef)
        {
            if (!IsBlacklisted(thoughtDef))
                return false;
            thoughtBlacklist.Remove(thoughtDef);
            return true;
        }
    }
}