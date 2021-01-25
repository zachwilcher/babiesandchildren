using BabiesAndChildren.Harmony;
using Verse;

namespace BabiesAndChildren
{
    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {

            ChildrenBase.ModWIP_ON = ModTools.IsModOn("[NL] Facial Animation - WIP");
            ChildrenBase.ModCSL_ON = ModTools.IsModOn("Children, school and learning");
            ChildrenBase.ModRJW_ON = ModTools.IsModOn("RimJobWorld");
            ChildrenBase.ModAT_ON = ModTools.IsModOn("Android tiers");
            if (ChildrenBase.ModWIP_ON)
            {
                CLog.Message("Patching Facial Animation");
                FacialAnimationPatches.Patch();
            }
        }
    }
}