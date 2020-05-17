using System;
using System.Collections.Generic;

using SDL2;
using AwokeKnowing.GnuplotCSharp;

using evobox.Graphical;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        private static bool quit = false;
        private static Random rand;
        private static Environment env;
        private static Minimap minimap;

        static void Main(string[] args) {

            double x = 0;
            while (true) {
                x += 0.0001;
                GnuPlot.Plot(String.Format("sin(x*{0})", x));
            }

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

            Globals.window     = window;
            Globals.renderer   = renderer;
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
            Camera cam = new Camera(Vector2.zero, 15, 15, Globals.viewport);
            env.AddObject(cam);

            // Create the minimap.
            minimap = new Minimap(env, cam, Globals.mapRect);
        }

        private static void MainLoop() {
            PollEvents();

            // Check if user wants to quit.
            foreach (SDL.SDL_Event e in Globals.eventQueue) {
                if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                    quit = true;
                }
            }

            var renderer   = Globals.renderer;
            var screenRect = renderer.OutputRect();
            Globals.screenRect.Set(screenRect);
            Globals.viewport.Set(screenRect.Square());
            Globals.mapRect.Set(
                    new Rect(
                        0,
                        0,
                        Globals.viewport.W / 5,
                        Globals.viewport.H / 5)
                    );

            // Clear the screen.
            renderer.Color = Color.black;
            renderer.Clear();

            // Fill the draw area
            Globals.renderer.Color = Color.white;
            renderer.FillRect(Globals.viewport);


            // Update the environment.
            env.Update(1.0 / 60.0);

            // The left and right side outside the viewport.
            // These are needed to cover up entities drawn at the edge of
            // the viewport.
            Rect leftSide = new Rect(
                    0,
                    0,
                    (screenRect.W - Globals.viewport.W) / 2,
                    screenRect.H
            );
            Rect rightSide = new Rect(
                    leftSide.W + Globals.viewport.W,
                    0,
                    leftSide.W,
                    screenRect.H
            );

            renderer.Color = Color.black;
            renderer.FillRect(leftSide);
            renderer.FillRect(rightSide);

            // Draw the minimap.
            minimap.DrawMinimap();

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
