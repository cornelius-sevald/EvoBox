using System;
using System.Linq;

using SDL2;

using evobox.Graphical;

namespace evobox.UI {

    /// <summary>
    /// A UI text label
    /// </summary>
    public class Label {
        public double x, y, w, h;
        private string text;
        private Font font;
        private Texture textTexture = null;

        /// <summary>
        /// Construct a new label
        /// </summary>
        /// <param name="x">The horizontal offset of the label [0 ; 1]</param>
        /// <param name="y">The vertical offset of the label [0 ; 1]</param>
        /// <param name="w">The width of the label [0 ; 1]</param>
        /// <param name="h">The height of the label [0 ; 1]</param>
        /// <param name="text">The label text</param>
        /// <param name="font">The font of the label</param>
        public Label(double x, double y, double w, double h,
        string text, Font font) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.text = text;
            this.font = font;
        }

        /// <summary>
        /// Draw the label
        /// </summary>
        /// <param name="dst">The rectangle to draw the label upon</param>
        public void Draw(Rect dst) {
            var renderer = Globals.renderer;

            Rect labelRect = new Rect(
                    (int)Math.Round(x * dst.W + dst.X),
                    (int)Math.Round(y * dst.H + dst.Y),
                    (int)Math.Round(w * dst.W),
                    (int)Math.Round(h * dst.H)
                    );

            // Draw the text
            if (textTexture == null) {
                using (
                        Surface textSurf =
                        font.TextSurface(text, Graphics.black)
                ) {
                    textTexture = new Texture(renderer, textSurf);
                }
            }
            renderer.RenderTexture(textTexture, labelRect, null);
        }
    }
}
