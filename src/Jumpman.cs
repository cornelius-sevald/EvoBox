using System;
using System.Collections;
using System.Collections.Generic;
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
        const double MUTATUIN_CHANCE = 0.05;
        /// <summary>
        /// Amount of energy use per second to stay alive.
        /// </summary>
        const double ENERGY_USAGE = 5;
        const double PROCREATION_ENERGY_THRESHOLD = 100;
        const double PROCREATION_ENERGY_COST = 50;

        public Vector2 velocity;
        public Environment environment;

        public double energy { get; private set; }

        private Texture[] sprites;
        private Random rand;
        private double speed     = 3.0;
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
        /// Construct a new jumpman with a scale of (1, 1) centered at (0, 0)
        /// with random genes.
        /// </summary>
        /// <param name="energy">Amount of energy to start with.</param>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(double energy, Random rand)
            : this(Vector2.zero, energy, rand) { }

        /// <summary>
        /// Construct a new jumpman with random genes.
        /// </summary>
        /// <param name="position">Position of the jumpman.</param>
        /// <param name="energy">Amount of energy to start with.</param>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Vector2 position, double energy, Random rand)
            : this(position, energy, Genome.RandomGenome(GENOME_LENGTH, rand), rand) {}

        /// <summary>
        /// Construct a new jumpman.
        /// </summary>
        /// <param name="position">Position of the jumpman.</param>
        /// <param name="energy">Amount of energy to start with.</param>
        /// <param name="genome">The genes of the jumpman.</param>
        /// <param name="rand">A RNG used for random movement.</param>
        public Jumpman(Vector2 position, double energy, Genome genome, Random rand)
            : base(position, Vector2.one, Z_INDEX)
        {
            this.sprites = new Texture[] {
                new Texture(Globals.RENDERER, RIGHT_TEXTURE_NAME),
                    new Texture(Globals.RENDERER, LEFT_TEXTURE_NAME),
                    new Texture(Globals.RENDERER, FRONT_TEXTURE_NAME)
            };
            this.energy = energy;
            this.genome = genome;
            this.rand = rand;
            this.velocity = speed * Vector2.FromAngle(rand.NextDouble() * 2 * Math.PI);

            SetColor();
        }

        /// <summary>
        /// Kill the jumpman, removing him from the environment.
        /// <param name="reason">A debug message as to the cause of death.</param>
        /// </summary>
        public void Die(string reason) {
            reason = reason == null || reason.Trim() == "" ? "unkown reasons" : reason;
            Console.WriteLine(String.Format("Jumpman died due to {0}.", reason));
            environment.RemoveObject(this);
        }

        /// <summary>
        /// Spawn a new jumpman as the child of this one with mutated genes.
        /// <param name="cost">The energy that will be transferred from this jumpman to his child</param>
        /// </summary>
        public void Procreate(double cost) {
            // Create a new RNG and clone the genome.
            Random cRand   = new Random(rand.Next());
            Genome cGenome = new Genome(genome);
            // Mutate the genome.
            cGenome.Mutate(MUTATUIN_CHANCE, cRand);

            // Create the child and add it io the environment.
            Jumpman child = new Jumpman(transform.position, cost, cGenome, cRand);
            environment.AddObject(child);

            this.energy -= cost;
        }

        public override void Update(double deltaTime) {
            if (energy < 0) {
                Die("starvation");
                return;
            }

            if (energy > PROCREATION_ENERGY_THRESHOLD) {
                Procreate(PROCREATION_ENERGY_COST);
            }

            Move(deltaTime);
            CheckCollisions();
            energy -= ENERGY_USAGE * deltaTime;
        }

        /// <summary>
        /// Move the jumpman, checking if out of bounds.
        /// </summary>
        private void Move(double deltaTime) {
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
        /// Check if the jumpman has collided with any food that he can eat.
        /// </summary>
        private void CheckCollisions() {
            List<Food> foods = environment.food;
            foreach (Food food in foods) {
                // Eat the food if jumpman is on top of it.
                if (Transform.OverlapAABB(this.transform, food.transform)) {
                    Eat(food);
                }
            }
        }

        /// <summary>
        /// Eat some food, obtaining its nutrition as energy.
        /// </summary>
        private void Eat(Food food) {
            environment.RemoveObject(food);
            energy += food.nutrition;
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
