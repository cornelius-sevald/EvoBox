using System;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        private const string RESOURCE_PATH = "resources/";

        static void Main(string[] args) {

            // Initialize graphics, create window & renderer etc.
            Graphics.InitGraphics();
            Window window = new Window("EvoBox", 100, 100,
                    SCREEN_WIDTH, SCREEN_HEIGHT);
            Renderer renderer = new Renderer(window);

            Surface icon = new Surface("EvoBoxIcon.png");
            window.SetWindowIcon(icon);

            // Random number generation.
            Random rand = new Random();

            // Create some random jumpmen.
            Texture jumpmanSprite = new Texture(renderer, "Jumpman.png");
            List<Entity> jumpmen = new List<Entity>();
            for (int i = 0; i < 5; i++) {
                Vector2 pos = new Vector2(rand.NextDouble(), rand.NextDouble());
                Vector2 scale = new Vector2(rand.NextDouble(), rand.NextDouble());
                Jumpman randJumpman = new Jumpman(pos, scale, jumpmanSprite, rand.Next());
                jumpmen.Add(randJumpman);
            }

            // Create a camera centered on the jumpmen.
            Camera camera = new Camera(Vector2.one * 0.5, 2, 2);

            // Main loop.
            bool quit = false;
            while (!quit) {
                // Check if user wants to quit.
                SDL.SDL_Event e;
                while (SDL.SDL_PollEvent(out e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        quit = true;
                    }
                }

                renderer.Clear();

                // Update the jumpmen.
                foreach (Jumpman jumpman in jumpmen) {
                    // Constant frame rate. Does not really matter.
                    jumpman.Update(1.0 / 60.0);
                }

                // Draw the jumpmen.
                Rect drawRect = renderer.OutputRect();
                camera.Draw(renderer, drawRect, jumpmen);

                renderer.Present();
            }
        }
    }
}
