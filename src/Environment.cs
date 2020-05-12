using System;
using System.Collections.Generic;

namespace evobox {
    public class Environment : SceneObject {

        const double MIN_FOOD_NUTRITION = 4;
        const double MAX_FOOD_NUTRITION = 16;

        /// <summary>
        /// All of the jumpmen in this environment.
        /// <p>
        /// The elements of this list overlaps with `sceneObjects` and `entities`.
        /// </p>
        /// </summary>
        public List<Jumpman> jumpmen { get; private set; }

        /// <summary>
        /// All of the food in this environment.
        /// <p>
        /// The elements of this list overlaps with `sceneObjects` and `entities`.
        /// </p>
        /// </summary>
        public List<Food> food { get; private set; }

        public List<SceneObject> sceneObjects { get; private set; }
        public List<Entity> entities { get; private set; }

        public double width {
            get { return transform.scale.x; }
        }
        public double height {
            get { return transform.scale.y; }
        }

        /// <summary>
        /// The chance of food spawning.
        /// </summary>
        private double foodSpawnRate = 10;
        private Random rand;

        private List<SceneObject> addPool;
        private List<SceneObject> removePool;

        /// <summary>
        /// Create a new environment with a given size centered at (0, 0).
        /// </summary>
        /// <param name="width">The width of the environment.</param>
        /// <param name="height">The height of the environment.</param>
        public Environment(double width, double height, Random rand)
            : base(Vector2.zero, new Vector2(width, height))
        {
            jumpmen = new List<Jumpman>();
            food = new List<Food>();
            sceneObjects = new List<SceneObject>();
            entities = new List<Entity>();

            addPool = new List<SceneObject>();
            removePool = new List<SceneObject>();

            this.rand = rand;
        }

        /// <summary>
        /// Add a scene object to the environment.
        /// <p>
        /// It will automatically be put into the appropriate lists.
        /// </p>
        /// </summary>
        public void AddObject(SceneObject sceneObject) {
            addPool.Add(sceneObject);
        }

        /// <summary>
        /// Remove a scene object from the environment.
        /// </summary>
        public void RemoveObject(SceneObject sceneObject) {
            removePool.Add(sceneObject);
        }

        /// <summary>
        /// Update the environment and all of the objects in it.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        public override void Update(double deltaTime) {
            if (rand.NextDouble() < foodSpawnRate * deltaTime) {
                SpawnFood();
            }

            foreach (SceneObject sceneObject in sceneObjects) {
                sceneObject.Update(deltaTime);
            }

            AddPoolObjects();
            RemovePoolObjects();
        }

        /// <summary>
        /// Add all of the items from the <c>addPool</c> list to the environment.
        /// </summary>
        private void AddPoolObjects() {
            foreach (SceneObject sceneObject in addPool) {
                if (sceneObject is Jumpman j) {
                    this.jumpmen.Add(j);
                }
                if (sceneObject is Food f) {
                    this.food.Add(f);
                }
                if (sceneObject is Entity e) {
                    this.entities.Add(e);
                }
                this.sceneObjects.Add(sceneObject);
                sceneObject.environment = this;
            }
            addPool.Clear();
        }

        /// <summary>
        /// Remove all of the items from the <c>addPool</c> list from the environment.
        /// </summary>
        private void RemovePoolObjects() {
            foreach (SceneObject sceneObject in removePool) {
                if (sceneObject is Jumpman j) {
                    this.jumpmen.Remove(j);
                }
                if (sceneObject is Food f) {
                    this.food.Remove(f);
                }
                if (sceneObject is Entity e) {
                    this.entities.Remove(e);
                }
                this.sceneObjects.Remove(sceneObject);
            }
            removePool.Clear();
        }

        private void SpawnFood() {
            double nutrition = rand.NextDouble() * (MAX_FOOD_NUTRITION - MIN_FOOD_NUTRITION) + MIN_FOOD_NUTRITION;
            double xPos = (0.5 - rand.NextDouble()) * transform.scale.x - transform.position.x;
            double yPos = (0.5 - rand.NextDouble()) * transform.scale.y - transform.position.y;
            Vector2 pos = new Vector2(xPos, yPos);

            Food food = new Food(pos, nutrition);
            AddObject(food);
        }
    }
}
