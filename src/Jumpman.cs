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
        const int GENOME_LENGTH = 32;
        const double MUTATUIN_CHANCE = 0.05;
        const double PROCREATION_ENERGY_THRESHOLD = 100;
        const double PROCREATION_ENERGY_COST = 50;
        const double MIN_SPEED = 1;
        const double MAX_SPEED = 5;

        public Vector2 velocity;

        public double energy { get; private set; }

        private Texture[] sprites;
        private Random rand;
        private double speed;
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
                new Texture(Globals.renderer, RIGHT_TEXTURE_NAME),
                    new Texture(Globals.renderer, LEFT_TEXTURE_NAME),
                    new Texture(Globals.renderer, FRONT_TEXTURE_NAME)
            };
            this.energy = energy;
            this.genome = genome;
            this.rand = rand;

            SetColor();
            SetSpeed();
            this.velocity = speed * Vector2.FromAngle(rand.NextDouble() * 2 * Math.PI);
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

        /// <summary>
        /// Calculate the energy used by the jumpman in one second.
        /// </summary>
        private double GetEnergyUsagePerSecond() {
            const double IDLE_ENERGY_COST  = 5;
            const double SPEED_ENERGY_COST = 1;

            return IDLE_ENERGY_COST + SPEED_ENERGY_COST * speed;
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
            energy -= GetEnergyUsagePerSecond() * deltaTime;
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

        private void SetSpeed() {
            // Need an array to hold the byte.
            byte[] _speedFactor = new byte[1];
            // Slice the relevant genes.
            BitArray bits = genome.Slice(24, 8);

            // Copy the 8 bits to the byte array.
            bits.CopyTo(_speedFactor, 0);
            // Convert the byte to a value between 0 and 1.
            double speedFactor = (double) _speedFactor[0] / 256;
            this.speed = MIN_SPEED + MAX_SPEED * speedFactor;
        }
    }
}
