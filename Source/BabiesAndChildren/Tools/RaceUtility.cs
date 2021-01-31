using System.Collections.Generic;
using BabiesAndChildren.api;
using Verse;

namespace BabiesAndChildren.Tools
{
    public static class RaceUtility
    {

        private static Dictionary<ThingDef, bool> thingUsesChildrenCache = new Dictionary<ThingDef, bool>();
        
        /// <summary>
        /// Whether a thing with race will have it's children handled by this mod.
        /// </summary>
        public static bool ThingUsesChildren(ThingDef thingDef)
        {
            var raceProps = thingDef?.race;
            if (raceProps == null)
                return false;
            
            if (thingUsesChildrenCache.ContainsKey(thingDef))
                return thingUsesChildrenCache[thingDef];
            
            var usesChildren = 
                   raceProps.Humanlike &&
                   !ModTools.IsRobot(thingDef) &&
                   raceProps.lifeStageAges.Count >= 5 && //assume first 5 stages are baby, toddler, child, teen, adult
                   !Races.IsBlacklisted(thingDef);
            
            thingUsesChildrenCache[thingDef] = usesChildren;
            return usesChildren;
        }

        public static void ClearCache()
        {
            thingUsesChildrenCache.Clear();
        }


        /// <summary>
        /// Determines whether a pawn's race has it's children handled by this mod
        /// </summary>
        public static bool PawnUsesChildren(Pawn pawn)
        {
            return ThingUsesChildren(pawn?.def);
        }
    }
}