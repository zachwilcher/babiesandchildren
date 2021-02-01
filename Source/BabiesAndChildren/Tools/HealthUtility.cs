using Verse;

namespace BabiesAndChildren.Tools
{
    public static class HealthUtility
    {
        public static bool TryAddHediff(Pawn pawn, HediffDef hediffDef, BodyPartRecord part = null, DamageInfo? damageInfo = null, bool force = false)
        {
            if (pawn == null )
                return false;

            if (pawn.health.hediffSet.HasHediff(hediffDef) && !force)
                return false;

            pawn.health.AddHediff(hediffDef, part, damageInfo);
            return true;
        }

        public static BodyPartRecord TryGetBodyPart(Pawn pawn, string bodyPartName)
        {
            return pawn?.RaceProps.body.AllParts.Find(x => x.def.defName == bodyPartName);
        }
    }
}