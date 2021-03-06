using System;
using System.Linq;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {
    public class Environment : Entity {

        const int Z_INDEX = -100;
        const double MIN_FOOD_NUTRITION = 4;
        const double MAX_FOOD_NUTRITION = 16;

        /// <summary>
        /// The amount of seconds that has transpired in the environment.
        /// This does not necessarily correlate with the real-world time.
        /// </summary>
        public double Time { get; private set; }

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

        public bool paused = false;

        /// <summary>
        /// The time scaling of everything in the environment.
        /// Increasing this value makes everything faster.
        /// </summary>
        public int timeScale = 1;

        private Random rand;
        private Texture terrainTexture;

        private List<SceneObject> addPool;
        private List<SceneObject> removePool;

        /// <summary>
        /// Create a new environment with a given size centered at (0, 0).
        /// </summary>
        /// <param name="width">The width of the environment.</param>
        /// <param name="height">The height of the environment.</param>
        public Environment(double width, double height, Random rand)
            : base(Vector2.zero, new Vector2(width, height), Z_INDEX)
        {
            Time = 0;

            jumpmen = new List<Jumpman>();
            food = new List<Food>();
            sceneObjects = new List<SceneObject>();
            entities = new List<Entity>();

            addPool = new List<SceneObject>();
            removePool = new List<SceneObject>();

            this.rand = rand;

            // Create a tiling ground texture.
            var renderer = Globals.renderer;
            terrainTexture = new Texture(renderer, "sprites/terrain_grass.png");
            // The dimentions of the terrain texture.
            int _tx, _ty;
            terrainTexture.Query(out _tx, out _ty);
            // The dimentions of the whole texture.
            int tx = _tx * (int)Math.Ceiling(transform.scale.x);
            int ty = _tx * (int)Math.Ceiling(transform.scale.y);
            this.texture = new Texture(renderer, tx, ty,
                    SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET);
            renderer.SetTarget(this.texture);

            for (int x = 0; x < tx; x += _tx) {
                for (int y = 0; y < ty; y += _ty) {
                    Rect dst = new Rect(x, y, _tx, _ty);
                    renderer.RenderTexture(terrainTexture, dst, null);
                }
            }
            renderer.SetTarget(null);
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
            // Run more simulation frames between rendering.
            if (!paused) {
                for (int i = 0; i < timeScale; i++) {
                    UpdateSimulation(deltaTime);
                }
            }

            // Update non-simulation objects.
            foreach (SceneObject sceneObject in sceneObjects.Where(s => !s.SimulationObject)) {
                sceneObject.Update(deltaTime);
            }
        }

        private void UpdateSimulation(double deltaTime) {
            double foodSpawnRate = Simulation.Instance.settings.foodSpawnRate;
            Time += deltaTime;

            // The food spawn rate for the entire environment.
            for (int i = 0; i < (int)(transform.scale.x * transform.scale.y); i++) {
                if (rand.NextDouble() < foodSpawnRate * deltaTime) {
                    SpawnFood();
                }
            }

            foreach (SceneObject sceneObject in sceneObjects.Where(s => s.SimulationObject)) {
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

                // Send and event to any listeners.
                var args = new SceneObjectAddedOrRemovedEventArgs();
                args.Object = sceneObject;
                args.Time = Time;
                OnSceneObjectAdded(args);
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

                // Send and event to any listeners.
                var args = new SceneObjectAddedOrRemovedEventArgs();
                args.Object = sceneObject;
                args.Time = Time;
                OnSceneObjectRemoved(args);
            }
            removePool.Clear();
        }

        private void SpawnFood() {
            double nutrition = rand.NextDouble() * (MAX_FOOD_NUTRITION - MIN_FOOD_NUTRITION) + MIN_FOOD_NUTRITION;

            Food food = new Food(Vector2.zero, nutrition);

            double xPos = (0.5 - rand.NextDouble()) *
                (transform.scale.x - food.transform.scale.x) -
                transform.position.x;
            double yPos = (0.5 - rand.NextDouble()) *
                (transform.scale.y - food.transform.scale.y) -
                transform.position.y;

            food.transform.position = new Vector2(xPos, yPos);

            AddObject(food);
        }

        protected virtual void OnSceneObjectAdded(SceneObjectAddedOrRemovedEventArgs e) {
            EventHandler<SceneObjectAddedOrRemovedEventArgs> handler = SceneObjectAdded;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnSceneObjectRemoved(SceneObjectAddedOrRemovedEventArgs e) {
            EventHandler<SceneObjectAddedOrRemovedEventArgs> handler = SceneObjectRemoved;
            if (handler != null) {
                handler(this, e);
            }
        }

        public event EventHandler<SceneObjectAddedOrRemovedEventArgs> SceneObjectAdded;
        public event EventHandler<SceneObjectAddedOrRemovedEventArgs> SceneObjectRemoved;
    }
}
