using HarmonyLib;
using System.Linq;
using BabiesAndChildren.Harmony;
using Verse;

namespace BabiesAndChildren
{
    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {

            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (thingDef.race != null && thingDef.race.Humanlike && thingDef.race.lifeStageAges.Count == 5)
                {
                    thingDef.comps.Add(new CompProperties_Growing());         
                }
            }

            ChildrenBase.ModWIP_ON = ChildrenUtility.IsModOn("[NL] Facial Animation - WIP");
            ChildrenBase.ModCSL_ON = ChildrenUtility.IsModOn("Children, school and learning");
            ChildrenBase.ModRJW_ON = ChildrenUtility.IsModOn("RimJobWorld");

            if (ChildrenBase.ModWIP_ON)
            {
                FacialAnimationPatches.Patch();
            }
        }
    }

}
