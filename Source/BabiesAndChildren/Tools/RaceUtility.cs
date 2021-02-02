using System.Collections.Generic;
using System.Linq;
using BabiesAndChildren.api;
using Verse;

namespace BabiesAndChildren.Tools
{
    /// <summary>
    /// Helper class for race properties
    /// </summary>
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

        public static bool IsHuman(Pawn pawn)
        {
            return pawn.def == DefDatabase<ThingDef>.GetNamed("Human");
        }
        

        public static bool HasHumanlikeHead(Pawn pawn)
        {
            //TODO have a setting to chose which races are effected by this
            string[] humanlikes = { "Kurin_Race", "Ratkin"};
            return humanlikes.Contains(pawn.def.defName);
        }
    }
}