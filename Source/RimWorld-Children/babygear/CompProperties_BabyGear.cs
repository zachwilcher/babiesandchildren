using System;
using Verse;

namespace RimWorldChildren.babygear
{
    public class CompProperties_BabyGear : CompProperties
    {
        public bool isBabyGear;
        public CompProperties_BabyGear()
        {
            this.compClass = typeof(CompBabyGear);
        }
    }

    public class CompProperties_Toy : CompProperties
    {
        public bool isToy;
        public CompProperties_Toy()
        {
            this.compClass = typeof(CompToy);
        }
    }
}
