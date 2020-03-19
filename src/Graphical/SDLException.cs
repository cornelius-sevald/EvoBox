using System;

using SDL2;

namespace evobox.Graphical {

    /// <summary>
    /// Exception type for SDL errors
    /// </summary>
    public class SDLException : Exception {
        public SDLException(string message)
           : base(message + " error: " + SDL.SDL_GetError()) {
        }
    }

}
