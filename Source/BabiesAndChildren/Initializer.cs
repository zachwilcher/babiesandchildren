using BabiesAndChildren.Harmony;
using Verse;

namespace BabiesAndChildren
{
    [StaticConstructorOnStartup]
    public static class Initializer
    {
        static Initializer()
        {

            ChildrenBase.ModWIP_ON = ChildrenUtility.IsModOn("[NL] Facial Animation - WIP");
            ChildrenBase.ModCSL_ON = ChildrenUtility.IsModOn("Children, school and learning");
            ChildrenBase.ModRJW_ON = ChildrenUtility.IsModOn("RimJobWorld");
                    
            if (ChildrenBase.ModWIP_ON)
            {
                CLog.Message("Patching Facial Animation");
                FacialAnimationPatches.Patch();
            }
        }
    }
}