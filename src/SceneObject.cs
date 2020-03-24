namespace evobox {

    /// <summary>
    /// An object in a scene.
    /// </summary>
    public abstract class SceneObject {

        public Transform transform { get; private set; }

        /// <summary>
        /// Construct a new scene object given a position and scale.
        /// </summary>
        /// <param name="position">The position of the object.</param>
        /// <param name="scale">The size of the object.</param>
        public SceneObject(Vector2 position, Vector2 scale) {
            this.transform = new Transform(position, scale);
        }

        /// <summary>
        /// The update function gets called every frame.
        /// </summary>
        /// <param name="deltaTime">The amount of seconds since the last frame.</param>
        public virtual void Update(double deltaTime) { }

    }

}
