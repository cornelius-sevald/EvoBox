using System;
using System.Collections;
using evobox.Graphical;
using evobox.Genetic;

namespace evobox {

    /// <summary>
    /// An entity that moves around.
    /// </summary>
    public class Jumpman : Entity {

        const string RIGHT_TEXTURE_NAME = "sprites/jumpman_right.png";
        const string LEFT_TEXTURE_NAME = "sprites/jumpman_left.png";
        const string FRONT_TEXTURE_NAME = "sprites/jumpman_front.png";
        const int Z_INDEX = 0;
        const int GENOME_LENGTH = 24;

        public Vector2 velocity;
        public Environment environment;

        private Texture[] sprites;
        private Random rand;
        private double speed = 3.0;
        private double turnSpeed = 4.0;
        private Genome genome;

        public override Texture texture {
            get {
                if (velocity.x > 0) {
                    return sprites[0];
                } else if (velocity.x < 0) {
                    return sprites[1];
                } else {
                    return sprites[2];
                }
            }
        }

        /// <summary>
        /// Construct a new jumpman with a scale of (1, 1) centered at (0, 0).
        /// </summary>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Random rand)
            : this(Vector2.zero, rand) { }

        /// <summary>
        /// Construct a new jumpman.
        /// </summary>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Vector2 position, Random rand)
            : base(position, Vector2.one, Z_INDEX) {
            this.sprites = new Texture[] {
                new Texture(Globals.RENDERER, RIGHT_TEXTURE_NAME),
                new Texture(Globals.RENDERER, LEFT_TEXTURE_NAME),
                new Texture(Globals.RENDERER, FRONT_TEXTURE_NAME)
            };
            this.rand = rand;
            this.velocity = speed * Vector2.FromAngle(rand.NextDouble() * 2 * Math.PI);

            this.genome = Genome.RandomGenome(GENOME_LENGTH, rand);
            SetColor();
        }


        public override void Update(double deltaTime) {
            double angle = turnSpeed * (rand.NextDouble() * 2 * Math.PI - Math.PI) * deltaTime;
            velocity.Rotate(angle);
            this.transform.Translate(velocity * deltaTime);

            double minX = -environment.width / 2;
            double maxX = environment.width / 2;
            double minY = -environment.height / 2;
            double maxY = environment.height / 2;
            // Bounce jumpman if he is out of bounds.
            if (transform.position.x - transform.scale.x / 2 < minX) {
                transform.position.x = minX + transform.scale.x / 2;
                velocity.x *= -1;
            } else if (transform.position.x + transform.scale.x / 2 > maxX) {
                transform.position.x = maxX - transform.scale.x / 2;
                velocity.x *= -1;
            }

            if (transform.position.y - transform.scale.y / 2 < minY) {
                transform.position.y = minY + transform.scale.y / 2;
                velocity.y *= -1;
            } else if (transform.position.y + transform.scale.y / 2 > maxY) {
                transform.position.y = maxY - transform.scale.y / 2;
                velocity.y *= -1;
            }
        }

        /// <summary>
        /// Change the color of the jumpman based on his genes.
        /// </summary>
        private void SetColor() {
            byte[] rgb = new byte[3];
            BitArray bits = genome.Slice(0, 24);

            bits.CopyTo(rgb, 0);
            Color c = new Color(rgb[0], rgb[1], rgb[2]);
            foreach (Texture t in sprites) {
                t.SetColorMod(c);
            }
        }
    }
}
