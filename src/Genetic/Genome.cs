using System;
using System.Collections;

namespace evobox.Genetic {

    /// <summary>
    /// A genome class.
    /// <para>
    /// Individual genes are represented by bits.
    /// </para>
    /// </summary>
    public class Genome : ICollection {

        /// <summary>
        /// The internal bit array representing genes.
        /// </summary>
        public BitArray genes { get; private set; }

        // ICollection properties.
        public int Count { get { return genes.Count; } }
        public bool IsSynchronized { get { return genes.IsSynchronized; } }
        public object SyncRoot { get { return genes.SyncRoot; } }
        public bool this[int index] {
            get { return genes[index]; }
            set { genes[index] = value; }
        }

        /// <summary>
        /// Create a random genome.
        /// </summary>
        /// <param name="length">The amount of genes in the genome.</param>
        /// <param name="rand">Random number generator.</param>
        /// <returns></returns>
        public static Genome RandomGenome(int length, Random rand) {
            bool[] genes = new bool[length];
            for (int i = 0; i < genes.Length; i++) {
                genes[i] = rand.Next(2) == 0;
            }
            return new Genome(new BitArray(genes));
        }


        /// <summary>
        /// Copy a genome.
        /// </summary>
        public Genome(Genome genome) {
            this.genes = new BitArray(genome.genes);
        }

        /// <summary>
        /// Construct a genome from an array of bits.
        /// </summary>
        public Genome(BitArray genes) {
            this.genes = genes;
        }

        // ICollection methods.
        public void CopyTo(Array array, int index) {
            genes.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() {
            return genes.GetEnumerator();
        }

        /// <summary>
        /// Mutate a genome by randomly flipping bits.
        /// </summary>
        /// <param name="mutChance">The chance any given bit (gene) is flipped.</param>
        /// <param name="rand">Random number generator.</param>
        public void Mutate(double mutChance, Random rand) {
            bool[] _bitmask = new bool[genes.Length];
            for (int i = 0; i < _bitmask.Length; i++) {
                _bitmask[i] = rand.NextDouble() < mutChance;
            }
            BitArray bitmask = new BitArray(_bitmask);
            genes.Xor(bitmask);
        }

        /// <summary>
        /// Cross-over two genomes.
        /// </summary>
        public void OnePointCrossOver(Genome other, Random rand) {
            if (this.Count != other.Count) {
                throw new ArgumentException("Cannot cross over "
                                          + "genomes of different lengths.");
            }

            int split = rand.Next(1, this.Count);
            for (int i = 0; i < split; i++) {
                bool tmp = other[i];
                other[i] = this[i];
                this[i] = tmp;
            }
        }
    }

}
