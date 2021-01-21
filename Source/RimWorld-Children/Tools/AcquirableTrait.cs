using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimWorldChildren {
    /// <summary>
    /// This class defines a potentially acquirable trait to be added to a pool of possible traits
    /// a pawn may gain on birth or age up.
    /// Where applicable, weight may be used to compare traits when selecting randomly from a collection.
    /// </summary>
    public class AcquirableTrait : IExposable {
        private TraitDef traitDef;
        private int degree;
        //We don't default to traitDef's commonality because we may want to set custom weights under certain circumstances
        private float weight;

        public float Weight {
            get => weight;
            set => weight = value;
        }

        public int Degree {
            get => degree;
            set => degree = value;
        }

        public TraitDef TraitDef {
            get => traitDef;
        }

        public void ExposeData() {
            Scribe_Defs.Look(ref traitDef, "traitDef");
            Scribe_Values.Look(ref degree, "degree");
            Scribe_Values.Look(ref weight, "weight");
        }

        public AcquirableTrait(TraitDef def, int degree = 0, float weight = 1) {
            this.traitDef = def;
            this.degree = degree;
            this.weight = weight;
        }
    }
}
