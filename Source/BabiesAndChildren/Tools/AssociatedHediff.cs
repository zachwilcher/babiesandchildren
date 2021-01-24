using System;

namespace BabiesAndChildren {

    /// <summary>
    /// To simplify creation of subclass hediffs, we'll simply label the class internally
    /// with the name of the hediff def it relates to.
    /// </summary>
    public class AssociatedHediff : Attribute {
        public string defName { get; private set; }

        public AssociatedHediff(string defName) {
            this.defName = defName;
        }
    }
}
