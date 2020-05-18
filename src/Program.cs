using System;
using System.Threading;
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
        private static Minimap minimap;
        private static EnvironmentTracker envTracker;
        private static EnvironmentGrapher envGrapher;
        // Used to wait between updating the graph.
        private static object graphMonitor = new object();
        private static Thread graphThread;

        static void Main(string[] args) {

            Initialize();

            // Main loop.
            while (!quit) {
                MainLoop();
            }

            DeInitialize();
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

            // Create the environment tracker.
            envTracker = new EnvironmentTracker(env);

            env.SceneObjectAdded += c_SceneObjectAdded;

            // Create the environment grapher.
            envGrapher = new EnvironmentGrapher(envTracker, GraphTypes.NutritionAndJumpmen);
            graphThread = new Thread(UpdateGraph);
            graphThread.Start();
        }

        private static void MainLoop() {
            PollEvents();

            foreach (SDL.SDL_Event e in Globals.eventQueue) {
                switch (e.type) {
                    // Check if user wants to quit.
                    case SDL.SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                    // Check if the user has pressed af button.
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym) {
                            // 'n' switches to the next graph.
                            case SDL.SDL_Keycode.SDLK_n:
                                envGrapher.graphType = envGrapher.graphType.Next();
                                break;
                        }
                        break;
                    default:
                        break;
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

        private static void DeInitialize() {
            // Send a signal to the UpdateGraph method that is should
            // stop sleeping.
            lock (graphMonitor) {
                Monitor.Pulse(graphMonitor);
            }
            graphThread.Join();
        }

        private static void UpdateGraph() {
            while (envGrapher != null && !quit) {
                lock (graphMonitor) {
                    envGrapher.UpdateGraph();
                    Monitor.Wait(graphMonitor, TimeSpan.FromSeconds(1));
                }
            }
        }

        /// <summary>
        /// Add the <c>c_JumpmanDeath</c> handler to every new jumpman.
        /// </summary>
        static void c_SceneObjectAdded(object sender, SceneObjectAddedOrRemovedEventArgs e) {
            if (e.Object is Jumpman j) {
                j.Death += c_JumpmanDeath;
            }
        }

        /// <summary>
        /// Send a debug message when a jumpman dies.
        /// </summary>
        static void c_JumpmanDeath(object sender, JumpmanDeathEventArgs e) {
            Console.WriteLine("Jumpman died due to {0} at {1:0.00} seconds.",
                    e.DeathReason, e.TimeOfDeath);
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
