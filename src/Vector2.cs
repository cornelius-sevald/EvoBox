using System;

namespace evobox {

    /// <summary>
    /// 2D Vectors and points.
    /// </summary>
    public struct Vector2 {
        public double x, y;

        /// <summary>
        /// Construct a new vector with a x and y coordinate.
        /// </summary>
        public Vector2(double x, double y) {
            this.x = x;
            this.y = y;
        }

        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 one = new Vector2(1, 1);
        public static Vector2 up = new Vector2(0, 1);
        public static Vector2 down = new Vector2(0, -1);
        public static Vector2 left = new Vector2(-1, 0);
        public static Vector2 right = new Vector2(1, 0);

        /// <summary>
        /// The squared length of this vector.
        /// </summary>
        public double SqrtMagnitude {
            get {
                return x * x + y * y;
            }
        }

        /// <summary>
        /// The length of this vector.
        /// </summary>
        public double Magnitude {
            get {
                return Math.Sqrt(this.SqrtMagnitude);
            }
        }

        /// <summary>
        /// This vector with a length of 1.
        /// </summary>
        public Vector2 Normalized {
            get {
                return this / this.Magnitude;
            }
        }

        public double this[int i] {
            get {
                switch (i) {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException("Index " + i +
                                " out of bounds.");
                }
            }
            set {
                switch (i) {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default:
                        throw new IndexOutOfRangeException("Index " + i +
                                " out of bounds.");
                }
            }
        }

        public override string ToString() {
            return "(" + x + ", " + y + ")";
        }

        public static Vector2 operator +(Vector2 v) => v;
        /// <summary>
        /// Negate a vector
        /// </summary>
        public static Vector2 operator -(Vector2 v) => new Vector2(-v.x, -v.y);

        /// <summary>
        /// Add two vectors.
        /// </summary>
        public static Vector2 operator +(Vector2 v, Vector2 u) =>
            new Vector2(v.x + u.x, v.y + u.y);
        /// <summary>
        /// Subsract one vector from another.
        /// </summary>
        public static Vector2 operator -(Vector2 v, Vector2 u) => v + (-u);

        /// <summary>
        /// Scale a vector.
        /// </summary>
        public static Vector2 operator *(Vector2 v, double a) =>
            new Vector2(v.x * a, v.y * a);
        /// <summary>
        /// Inverse scale a vector.
        /// </summary>
        public static Vector2 operator /(Vector2 v, double a) => v * (1 / a);

        /// <summary>
        /// The dot product of two vectors.
        /// </summary>
        public static double operator *(Vector2 v, Vector2 u) =>
            v.x * u.x + v.y * u.y;
    }

}
