using System;

namespace evobox {

    /// <summary>
    /// Position and scale of an object.
    /// </summary>
    public class Transform {

        public Vector2 position;
        public Vector2 scale;

        /// <summary>
        /// Return true if the Axis Aligned Bounding Box (AABB)
        /// of two transforms overlap.
        /// </summary>
        public static bool OverlapAABB(Transform t1, Transform t2) {
            if (Math.Abs(t1.position.x - t2.position.x) >
                    (t1.scale.x + t2.scale.x) / 2) { return false; }
            if (Math.Abs(t1.position.y - t2.position.y) >
                    (t1.scale.y + t2.scale.y) / 2) { return false; }

            return true;
        }

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
