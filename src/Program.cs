using System;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        static void Main(string[] args) {

            // Initialize graphics, create window & renderer etc.
            Graphics.InitGraphics();
            Window window = new Window("EvoBox", 100, 100,
                    SCREEN_WIDTH, SCREEN_HEIGHT);
            Renderer renderer = new Renderer(window);

            Globals.WINDOW = window;
            Globals.RENDERER = renderer;

            Surface icon = new Surface("EvoBoxIcon.png");
            window.SetWindowIcon(icon);

            // Random number generation.
            Random rand = new Random();

            // Create a jumpman.
            Entity jumpman = new Jumpman(1, new Random(rand.Next()));

            // Create a camera centered on the jumpmen.
            Camera camera = new Camera(Vector2.zero, 10, 10);

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

                // Update the jumpman.
                jumpman.Update(1.0 / 60.0);

                // Clear the screen.
                renderer.Color = Color.black;
                renderer.Clear();

                // Fill the draw area
                renderer.Color = Color.white;
                Rect drawRect = renderer.OutputRect().Square();
                renderer.FillRect(drawRect);

                // Draw the jumpman.
                camera.Draw(renderer, drawRect, jumpman);

                renderer.Present();
            }
        }
    }
}
