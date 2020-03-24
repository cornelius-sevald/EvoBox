namespace evobox {

    /// <summary>
    /// Position and scale of an object.
    /// </summary>
    public struct Transform {

        public Vector2 position { get; set; }
        public Vector2 scale { get; set; }

        /// <summary>
        /// Construct a new transform given a position and scale.
        /// </summary>
        public Transform(Vector2 position, Vector2 scale) {
            this.position = position;
            this.scale = scale;
        }

        /// <summary>
        /// Move the transform by an amount in a direction.
        /// </summary>
        public void Translate(Vector2 v) {
            position += v;
        }

    }

}
