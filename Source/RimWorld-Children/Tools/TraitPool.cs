using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorldChildren {
    /// <summary>
    /// This class represents a collection of potentially acquirable traits to be added to a pawn at some interval or milestone.
    /// </summary>
    public class TraitPool : IExposable, ICollection, IEnumerable, ICollection<AcquirableTrait>, IEnumerable<AcquirableTrait> {
        //Implement some interfaces so we can potentially change hashset later
        private HashSet<AcquirableTrait> traits;

        public void ExposeData() {
            Scribe_Collections.Look(ref traits, "traits", LookMode.Deep);
        }

        /// <summary>
        /// Creates an empty TraitPool
        /// </summary>
        public TraitPool() {
            traits = new HashSet<AcquirableTrait>();
        }

        public int Count => ((ICollection<AcquirableTrait>)traits).Count;

        public bool IsReadOnly => ((ICollection<AcquirableTrait>)traits).IsReadOnly;

        public object SyncRoot => ((ICollection)traits).SyncRoot;

        public bool IsSynchronized => ((ICollection)traits).IsSynchronized;

        public void Add(AcquirableTrait item) {
            ((ICollection<AcquirableTrait>)traits).Add(item);
        }

        public void Clear() {
            ((ICollection<AcquirableTrait>)traits).Clear();
        }

        public bool Contains(AcquirableTrait item) {
            return ((ICollection<AcquirableTrait>)traits).Contains(item);
        }

        public void CopyTo(AcquirableTrait[] array, int arrayIndex) {
            ((ICollection<AcquirableTrait>)traits).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public IEnumerator<AcquirableTrait> GetEnumerator() {
            return ((ICollection<AcquirableTrait>)traits).GetEnumerator();
        }

        public bool Remove(AcquirableTrait item) {
            return ((ICollection<AcquirableTrait>)traits).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((ICollection<AcquirableTrait>)traits).GetEnumerator();
        }
    }
}
