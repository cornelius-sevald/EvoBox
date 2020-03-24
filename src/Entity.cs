using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// An entity is a visible object in a scene.
    /// </summary>
    public abstract class Entity : SceneObject {

        public int zIndex;

        public Texture texture { get; }

        /// <summary>
        /// Construct a new entity given a position, scale, texture and z index.
        /// </summary>
        /// <param name="position">The position of the entity.</param>
        /// <param name="scale">The size of the entity.</param>
        /// <param name="texture">The entities texture.</param>
        /// <param name="zIndex">The z-order of the entity.
        /// Higher means 'closer' to the screen.</param>
        public Entity(Vector2 position, Vector2 scale, Texture texture, int zIndex)
            : base(position, scale) {
            this.texture = texture;
            this.zIndex = zIndex;
        }

    }

}
