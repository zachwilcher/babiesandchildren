using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    /// <summary>
    /// No Manipulation Hediff.
    /// </summary>
    public class Hediff_NoFlag : HediffWithComps
    {
        public override void PostRemoved()
        {
            
            List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            foreach (var skillDef in allDefsListForReading)
            {
                pawn.skills.Learn(skillDef, 100, true);
                CLog.DevMessage("Showbaby skill>> " + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));
            }
            base.PostRemoved();
        }
    }
}