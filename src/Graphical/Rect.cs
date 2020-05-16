using System;

using SDL2;

namespace evobox.Graphical {

    /// <summary>
    /// A rectangle of the screen
    /// </summary>
    public class Rect {

        /// <summary>
        /// The horizontal offset from the left screen edge
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The vertical offset from the top screen edge
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The width of the rectangle
        /// </summary>
        public int W { get; set; }

        /// <summary>
        /// The height of the rectangle
        /// </summary>
        public int H { get; set; }

        public Rect(int x, int y, int w, int h) {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        /// <summary>
        /// Copy the elements of <c>other</c> to this rectangle.
        /// </summary>
        public void Set(Rect other) {
            this.X = other.X;
            this.Y = other.Y;
            this.W = other.W;
            this.H = other.H;
        }

        /// <summary>
        /// Return a squared version of this rectangle.
        /// </summary>
        public Rect Square() {
            int s = Math.Min(W, H);
            int x = (int) (X + (W - s) / 2.0);
            int y = (int) (Y + (H - s) / 2.0);

            return new Rect(x, y, s, s);
        }

        /// <summary>
        /// Get a SDL_Rect struct from this rectangle
        /// </summary>
        /// <value>A SDL_Rect struct</value>
        public SDL.SDL_Rect Rct {
            get {
                return new SDL.SDL_Rect { x=X, y=Y, w=W, h=H };
            }
        }
    }

}
