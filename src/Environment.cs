using System.Collections.Generic;

namespace evobox {
    public class Environment : SceneObject {

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
        /// Create a new environment with a given size centered at (0, 0).
        /// </summary>
        /// <param name="width">The width of the environment.</param>
        /// <param name="height">The height of the environment.</param>
        public Environment(double width, double height)
            : base(Vector2.zero, new Vector2(width, height)) {
            jumpmen = new List<Jumpman>();
            food = new List<Food>();
            sceneObjects = new List<SceneObject>();
            entities = new List<Entity>();
        }

        /// <summary>
        /// Add a scene object to the environment.
        /// <p>
        /// It will automatically be put into the appropriate lists.
        /// </p>
        /// </summary>
        public void AddObject(SceneObject sceneObject) {
            if (sceneObject is Jumpman j) {
                this.jumpmen.Add(j);
                j.environment = this;
            }
            if (sceneObject is Food f) {
                this.food.Add(f);
            }
            if (sceneObject is Entity e) {
                this.entities.Add(e);
            }
            this.sceneObjects.Add(sceneObject);
        }

        /// <summary>
        /// Remove a scene object from the environment.
        /// </summary>
        public void RemoveObject(SceneObject sceneObject) {
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

        /// <summary>
        /// Update the environment and all of the objects in it.
        /// </summary>
        /// <param name="deltaTime">The time in seconds since the last frame.</param>
        public override void Update(double deltaTime) {
            foreach (SceneObject sceneObject in sceneObjects) {
                sceneObject.Update(deltaTime);
            }
        }
    }
}
