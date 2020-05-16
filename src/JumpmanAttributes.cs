using System.Collections;

using evobox.Genetic;
using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// Genetic attributes of instances of the <c>Jumpman</c> class.
    /// Jumpmen represent creatures with different genetics.
    /// </summary>
    public class JumpmanAttributes {

        public const int GENOME_LENGTH = 32;
        const double MIN_SPEED = 1;
        const double MAX_SPEED = 5;

        public Color color;
        public double speed;

        public JumpmanAttributes(Color color, double speed) {
            this.color = color;
            this.speed = speed;
        }

        public static JumpmanAttributes FromGenome(Genome genome) {
            // === CALCULATE THE COLOR ===
            Color color;
            {
                byte[] rgb = new byte[3];
                BitArray bits = genome.Slice(0, 24);
                bits.CopyTo(rgb, 0);
                color = new Color(rgb[0], rgb[1], rgb[2]);
            }

            // === CALCULATE THE SPEED ===
            double speed;
            {
                // Need an array to hold the byte.
                byte[] _speedFactor = new byte[1];
                // Slice the relevant genes.
                BitArray bits = genome.Slice(24, 8);

                // Copy the 8 bits to the byte array.
                bits.CopyTo(_speedFactor, 0);
                // Convert the byte to a value between 0 and 1.
                double speedFactor = (double) _speedFactor[0] / 256;
                speed = MIN_SPEED + MAX_SPEED * speedFactor;
            }

            return new JumpmanAttributes(color, speed);
        }

    }

}
