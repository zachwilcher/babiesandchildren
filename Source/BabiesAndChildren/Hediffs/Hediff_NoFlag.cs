using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    public class Hediff_NoFlag : HediffWithComps
    {
        public override void PostRemoved()
        {
            List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                SkillDef skillDef = allDefsListForReading[i];
                pawn.skills.Learn(skillDef, 100, true);
                CLog.DevMessage("Showbaby skill>> " + pawn.Name + "'s " + skillDef.defName + " Skills set =" + pawn.skills.GetSkill(skillDef));
            }
            if (pawn.story.traits.HasTrait(ChildTraitDefOf.Newtype))
            {
                pawn.skills.GetSkill(SkillDefOf.Shooting).Level += 5;
                pawn.skills.GetSkill(SkillDefOf.Melee).Level += 5;
            }
            if (pawn.story.traits.HasTrait(TraitDefOf.Brawler))
            {
                pawn.skills.GetSkill(SkillDefOf.Shooting).Level -= 4;
                pawn.skills.GetSkill(SkillDefOf.Melee).Level += 4;
            }
            //pawn.Notify_DisabledWorkTypesChanged();
            //pawn.skills.Notify_SkillDisablesChanged();
            //MeditationFocusTypeAvailabilityCache.ClearFor(pawn);
            base.PostRemoved();
        }
    }
}