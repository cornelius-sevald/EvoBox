using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {

    public static class Globals {
        /// <summary>
        /// The path to the resource folder.
        ///
        /// <para>This is where all textures, fonts, etc.
        /// be located</para>
        /// </summary>
        public const string RESOURCE_PATH = "resources/";

        public static Window window     = null;
        public static Renderer renderer = null;
        public static int mouseX, mouseY;
        public static Rect screenRect   = new Rect(0, 0, 0, 0);
        public static Rect viewport     = new Rect(0, 0, 0, 0);
        public static Rect mapRect      = new Rect(0, 0, 0, 0);
        public static Queue<SDL.SDL_Event> eventQueue = null;
        public static readonly Keyboard keyboard = Keyboard.Instance;
    }

}
