using System;

using SDL2;

namespace evobox.Graphical {

    /// <summary>
    /// Simple RGBA color representation
    /// </summary>
    public struct Color {

        public static Color white = new Color(0xFFFFFFFF);
        public static Color cyan = new Color(0x00FFFFFF);
        public static Color magenta = new Color(0xFF00FFFF);
        public static Color yellow = new Color(0xFFFF00FF);
        public static Color red = new Color(0xFF0000FF);
        public static Color green = new Color(0x00FF000F);
        public static Color blue = new Color(0x0000FFFF);
        public static Color black = new Color(0x000000FF);

        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        /// <summary>
        /// Construct a color from RGBA values
        /// </summary>
        /// <param name="r">The red component from [0 ; 255]</param>
        /// <param name="g">The green component from [0 ; 255]</param>
        /// <param name="b">The blue component from [0 ; 255]</param>
        /// <param name="a">The alpha component from [0 ; 255]</param>
        public Color(byte r, byte g, byte b, byte a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Contruct a color from RGB values with full opacity
        /// </summary>
        /// <param name="r">The red component from [0 ; 255]</param>
        /// <param name="g">The green component from [0 ; 255]</param>
        /// <param name="b">The blue component from [0 ; 255]</param>
        public Color(byte r, byte g, byte b) {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        /// <summary>
        /// Construct a color from a hexidecimal number
        /// </summary>
        /// <param name="color">The hex representation fo the color</param>
        public Color(long color) {
            R = (byte)((color & 0xFF000000) >> 24);
            G = (byte)((color & 0x00FF0000) >> 16);
            B = (byte)((color & 0x0000FF00) >> 8);
            A = (byte)((color & 0x000000FF) >> 0);
        }

        /// <summary>
        /// Get a SDL_Color struct from this color
        /// </summary>
        /// <returns>An SDL_Color struct</returns>
        public SDL.SDL_Color SDLColor() {
            SDL.SDL_Color color;
            color.r = R;
            color.g = G;
            color.b = B;
            color.a = A;

            return color;
        }
    }

}
