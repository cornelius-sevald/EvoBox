using System.Collections;

using evobox.Genetic;
using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// Genetic attributes of instances of the <c>Jumpman</c> class.
    /// Jumpmen represent creatures with different genetics.
    /// </summary>
    public struct JumpmanAttributes {

        public const int GENOME_LENGTH = 40;
        public const double MIN_SPEED  = 1;
        public const double MAX_SPEED  = 5;
        public const double MIN_SIZE   = 0.7;
        public const double MAX_SIZE   = 1.3;

        public Color color;
        public double speed;
        public double size;

        /// <summary>
        /// Create new attributes.
        /// </summary>
        /// <param name="color">The color of the jumpman.
        ///                     Has no effect on survival.</param>
        /// <param name="speed">The speed of the jumpman.</param>
        /// <param name="size">The size of the jumpman.</param>
        public JumpmanAttributes(Color color, double speed, double size) {
            this.color = color;
            this.speed = speed;
            this.size  = size;
        }

        /// <summary>
        /// Create attributes using genetic information.
        /// </summary>
        public static JumpmanAttributes FromGenome(Genome genome) {
            Color color  = ColorFromGenome(genome);
            double speed = SpeedFromGenome(genome);
            double size  = SizeFromGenome(genome);

            return new JumpmanAttributes(color, speed, size);
        }

        private static Color ColorFromGenome(Genome genome) {
                byte[] rgb = new byte[3];
                BitArray bits = genome.Slice(0, 24);
                bits.CopyTo(rgb, 0);
                return new Color(rgb[0], rgb[1], rgb[2]);
        }

        private static double SpeedFromGenome(Genome genome) {
                // Need an array to hold the byte.
                byte[] _speedFactor = new byte[1];
                // Slice the relevant genes.
                BitArray bits = genome.Slice(24, 8);

                // Copy the 8 bits to the byte array.
                bits.CopyTo(_speedFactor, 0);
                // Convert the byte to a value between 0 and 1.
                double speedFactor = (double) _speedFactor[0] / 256;
                return MIN_SPEED + MAX_SPEED * speedFactor;
        }

        private static double SizeFromGenome(Genome genome) {
                // Need an array to hold the byte.
                byte[] _sizeFactor = new byte[1];
                // Slice the relevant genes.
                BitArray bits = genome.Slice(32, 8);

                // Copy the 8 bits to the byte array.
                bits.CopyTo(_sizeFactor, 0);
                // Convert the byte to a value between 0 and 1.
                double sizeFactor = (double) _sizeFactor[0] / 256;
                return MIN_SIZE + MAX_SIZE * sizeFactor;
        }
    }

}
