using System;
using System.IO;

using SDL2;

namespace evobox.Graphical {

    /// <summary>
    /// Wrapper of an SDL texture
    ///
    /// <para>A texture is much like a surface, but more efficient</para>
    /// <seealso cref="Surface"/>
    /// </summary>
    public class Texture : IDisposable {

        /// <summary>
        /// Pointer to the internal SDL_Texture
        /// </summary>
        public IntPtr TexPtr { get; private set; }

        /// <summary>
        /// Create a blank texture.
        /// </summary>
        public Texture(Renderer renderer, int width, int height,
                SDL.SDL_TextureAccess access) {
            IntPtr texture = SDL.SDL_CreateTexture(
                    renderer.RenPtr,
                    SDL.SDL_PIXELFORMAT_RGBA8888,
                    (int)access,
                    width,
                    height
                    );

            if (texture == IntPtr.Zero) {
                throw new SDLException("SDL_CreateTexture");
            }

            TexPtr = texture;
        }

        /// <summary>
        /// Create a texture from an image file
        /// </summary>
        /// <param name="renderer">The renderer that will render this texture</param>
        /// <param name="name">The path to the image from the resource folder</param>
        public Texture(Renderer renderer, string name) {
            // Construct the full path
            string path = Path.Combine(Globals.RESOURCE_PATH, name);

            IntPtr texture = SDL_image.IMG_LoadTexture(renderer.RenPtr, path);
            if (texture == IntPtr.Zero) {
                throw new SDLException("IMG_LoadTexture");
            }

            TexPtr = texture;
        }

        /// <summary>
        /// Create a texture from a surface
        /// </summary>
        /// <param name="renderer">The renderer that will render this texture</param>
        /// <param name="surface">The surface containing the image data</param>
        public Texture(Renderer renderer, Surface surface) {
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer.RenPtr, surface.SurfPtr);
            if (texture == IntPtr.Zero) {
                throw new SDLException("SDL_CreateTextureFromSurface");
            }

            TexPtr = texture;
        }

        /// <summary>
        /// Set an additional color value multiplied into render copy operations.
        /// </summary>
        public void SetColorMod(Color c) {
            SDL.SDL_SetTextureColorMod(
                TexPtr,
                c.R,
                c.G,
                c.B
            );
        }

        /// <summary>
        /// Get the additional color value multiplied into render copy operations.
        /// </summary>
        public Color GetColorMod() {
            byte r, g, b;
            SDL.SDL_GetTextureColorMod(TexPtr, out r, out g, out b);
            return new Color(r, g, b);
        }

        /// <summary>
        /// Get the width and height of the texture
        /// </summary>
        /// <param name="w">The width of the texture</param>
        /// <param name="h">The height of the texture</param>
        public void Query(out int w, out int h) {
            SDL.SDL_QueryTexture(TexPtr, out _, out _, out w, out h);
        }

        protected virtual void Dispose(bool disposing) {
            if (TexPtr != IntPtr.Zero) // only dispose once!
            {
                SDL.SDL_DestroyTexture(TexPtr);
            }
        }

        /// <summary>
        /// Dispose of this object, freeing used resources
        /// </summary>
        public void Dispose() {
            Dispose(true);
            // tell the GC not to finalize
            GC.SuppressFinalize(this);
        }

        ~Texture() {
            Dispose(false);
        }
    }

}
