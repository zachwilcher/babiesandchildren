using RimWorld;
using Verse;

namespace BabiesAndChildren
{
    /// <summary>
    /// Static accessors for child hediff defs
    /// </summary>
    [DefOf]
    public static class ChildHediffDefOf
    {
        public static HediffDef UnhappyBaby;
        public static HediffDef BabyState0;


        static ChildHediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ChildHediffDefOf));
        }

    }

    public static class ChildTraitDefOf
    {
        public static TraitDef Innocent = TraitDef.Named("Innocent");
        public static TraitDef Newtype = TraitDef.Named("Newtype");

    }
}
