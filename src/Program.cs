using System;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        private static bool quit = false;
        private static Random rand;
        private static Environment env;

        static void Main(string[] args) {

            Initialize();

            // Main loop.
            while (!quit) {
                MainLoop();
            }
        }

        private static void Initialize() {

            // Initialize graphics, create window & renderer etc.
            Graphics.InitGraphics();
            Window window = new Window("EvoBox", 100, 100,
                    SCREEN_WIDTH, SCREEN_HEIGHT);
            Renderer renderer = new Renderer(window);

            Globals.window = window;
            Globals.renderer = renderer;
            (Globals.viewport = renderer.OutputRect()).Square();
            Globals.eventQueue = new Queue<SDL.SDL_Event>();

            Surface icon = new Surface("EvoBoxIcon.png");
            window.SetWindowIcon(icon);

            // Random number generation.
            rand = new Random();

            // Create the environment.
            env = new Environment(30, 30, new Random(rand.Next()));

            // Add a jumpman to the environment with some energy.
            env.AddObject(
                    new Jumpman(99, new Random(rand.Next()))
                    );

            // Add the camera to the environment.
            env.AddObject(
                    new Camera(Vector2.zero, 30, 30, Globals.viewport)
                    );
        }

        private static void MainLoop() {
            PollEvents();

            // Check if user wants to quit.
            foreach (SDL.SDL_Event e in Globals.eventQueue) {
                if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                    quit = true;
                }
            }

            var renderer  = Globals.renderer;
            renderer.OutputRect(ref Globals.viewport);
            Globals.viewport.Square();

            // Clear the screen.
            renderer.Color = Color.black;
            renderer.Clear();

            // Fill the draw area
            Globals.renderer.Color = Color.white;
            renderer.FillRect(Globals.viewport);


            // Update the environment.
            env.Update(1.0 / 60.0);

            renderer.Present();
        }

        private static void PollEvents() {
            Globals.eventQueue.Clear();

            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0) {
                Globals.eventQueue.Enqueue(e);
            }
        }
    }
}
