using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns a collection of BodyPartRecords based on the part name provided.
        /// This may be a collection containing a single element, or multiple for left and right parts
        /// </summary>
        /// <param name="pawn">The pawn whose parts will be searched</param>
        /// <param name="bodyPart">The string defName of the body part to be returned</param>
        /// <returns>A collection of bodypart records</returns>
        public static List<BodyPartRecord> GetPawnBodyParts(Pawn pawn, String bodyPart)
        {
            return pawn?.RaceProps.body.AllParts.FindAll(x => x.def == DefDatabase<BodyPartDef>.GetNamed(bodyPart, true));
        }

        /// <summary>
        /// Fetch a random body part matching the provided string
        /// </summary>
        /// <param name="pawn">The pawn whose parts will be searched</param>
        /// <param name="bodyPart">The string defName of the body part to be returned</param>
        /// <returns>A single bodypartrecord matching the provided string, or the only part if only one part exists</returns>
        public static BodyPartRecord GetPawnBodyPart(Pawn pawn, String bodyPart)
        {
            //Get collection of parts matching the def, then get a random left or right
            return GetPawnBodyParts(pawn, bodyPart).RandomElement();
        }
    }
}