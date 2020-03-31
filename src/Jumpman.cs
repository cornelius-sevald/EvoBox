using System;
using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// An entity that moves around.
    /// </summary>
    public class Jumpman : Entity {

        public Vector2 velocity;

        private Random rand;
        private double speed = 3.0;
        private double turnSpeed = 4.0;

        /// <summary>
        /// Construct a new jumpman with a scale of (1, 1) centered at (0, 0).
        /// </summary>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Texture texture, int zIndex, Random rand)
            : this(Vector2.zero, Vector2.one, texture, zIndex, rand) { }

        /// <summary>
        /// Construct a new jumpman.
        /// </summary>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Vector2 position, Vector2 scale, Texture texture, int zIndex, Random rand)
            : base(position, scale, texture, zIndex) {
            this.rand = rand;
            this.velocity = speed * Vector2.FromAngle(rand.NextDouble() * 2 * Math.PI);
        }


        public override void Update(double deltaTime) {
            double angle = turnSpeed * (rand.NextDouble() * 2 * Math.PI - Math.PI) * deltaTime;
            velocity.Rotate(angle);
            this.transform.Translate(velocity * deltaTime);

            // Bounce jumpman if he is out of bounds.
            if (transform.position.x - transform.scale.x / 2 < -5) {
                transform.position.x = -5 + transform.scale.x / 2;
                velocity.x *= -1;
            } else if (transform.position.x + transform.scale.x / 2 > 5) {
                transform.position.x = 5 - transform.scale.x / 2;
                velocity.x *= -1;
            }

            if (transform.position.y - transform.scale.y / 2 < -5) {
                transform.position.y = -5 + transform.scale.y / 2;
                velocity.y *= -1;
            } else if (transform.position.y + transform.scale.y / 2 > 5) {
                transform.position.y = 5 - transform.scale.y / 2;
                velocity.y *= -1;
            }
        }
    }

}
