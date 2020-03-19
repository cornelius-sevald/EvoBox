using System;

using SDL2;

using evobox.Graphical;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        private const string RESOURCE_PATH = "resources/";

        static void Main(string[] args) {

            Graphics.InitGraphics();
            Window window = new Window("EvoBox", 100, 100,
                    SCREEN_WIDTH, SCREEN_HEIGHT);
            Renderer renderer = new Renderer(window);

            Texture jumpmanSprite = new Texture(renderer, "Jumpman.png");

            bool quit = false;
            while (!quit) {

                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        quit = true;
                    }
                }

                renderer.RenderTexture(jumpmanSprite);

                renderer.Present();
            }
        }
    }
}
