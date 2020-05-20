using System;
using System.Threading;

using SDL2;

using evobox.Graphical;

namespace evobox {

    public sealed class Simulation {

        public static Simulation Instance { get; private set; }

        private const int MIN_TIMESCALE = 1;
        private const int MAX_TIMESCALE = 10;
        // Time to 'warm up' the environment before spawning the jumpman.
        const double WARMUP_TIME = 1;

        public SimulationSettings settings { get; private set; }

        private bool helperMenu = true;
        private bool running = true;
        private Random rand;
        private Environment env;
        private Minimap minimap;
        private Texture helpMenuTex;
        private EnvironmentTracker envTracker;
        private EnvironmentGrapher envGrapher;
        // Used to wait between updating the graph.
        private object graphMonitor = new object();
        private Thread graphThread;

        public Simulation(SimulationSettings settings) {
            Instance = this;
            this.settings = settings;

            // Random number generation.
            this.rand = new Random();

            // Create the environment.
            this.env = new Environment(30, 30, new Random(rand.Next()));

            // 'Warm up' the environment.
            for (int i = 0; i < WARMUP_TIME * 60; i++) {
                env.Update(1.0/6.0);
            }

            // Add a jumpman to the environment with some energy.
            this.env.AddObject(
                    new Jumpman(99, new Random(rand.Next()))
                    );

            // Add the camera to the environment.
            Camera cam = new Camera(Vector2.zero, 15, 15, Globals.viewport);
            this.env.AddObject(cam);

            // Create the minimap.
            this.minimap = new Minimap(env, cam, Globals.mapRect);

            this.helpMenuTex = new Texture(Globals.renderer, "sprites/helper_menu.png");

            // Create the environment tracker.
            envTracker = new EnvironmentTracker(env);

            // Create the environment grapher.
            envGrapher = new EnvironmentGrapher(envTracker, GraphTypes.NutritionAndJumpmen);
            graphThread = new Thread(UpdateGraph);
            graphThread.Start();
        }

        public void Update(double deltaTime) {
            HandleInput();

            if (!helperMenu) {
                // Update the environment.
                env.Update(deltaTime);

                // Draw the minimap.
                minimap.DrawMinimap();
            } else {
                Globals.renderer.RenderTexture(helpMenuTex, Globals.viewport, null);
            }

        }

        public void Stop() {
            running = false;

            // Send a signal to the UpdateGraph method that is should
            // stop sleeping.
            lock (graphMonitor) {
                Monitor.Pulse(graphMonitor);
            }
            graphThread.Join();
            envGrapher.Close();

            Instance = null;
        }

        private void HandleInput() {
            foreach (SDL.SDL_Event e in Globals.eventQueue) {
                switch (e.type) {
                    // Check if the user has pressed af button.
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym) {
                            // 'n' switches to the next graph.
                            case SDL.SDL_Keycode.SDLK_n:
                                envGrapher.graphType = envGrapher.graphType.Next();
                                break;
                            // Escape toggles a help menu.
                            case SDL.SDL_Keycode.SDLK_ESCAPE:
                                helperMenu = !helperMenu;
                                break;
                            // Space toggles pausing the simulation.
                            case SDL.SDL_Keycode.SDLK_SPACE:
                                env.paused = !env.paused;
                                break;
                            // '+' makes the simulation faster.
                            case SDL.SDL_Keycode.SDLK_PLUS:
                                env.timeScale = Math.Clamp(env.timeScale + 1, MIN_TIMESCALE, MAX_TIMESCALE);
                                break;
                            // '-' makes the simulation slower.
                            case SDL.SDL_Keycode.SDLK_MINUS:
                                env.timeScale = Math.Clamp(env.timeScale - 1, MIN_TIMESCALE, MAX_TIMESCALE);
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateGraph() {
            while (envGrapher != null && running) {
                lock (graphMonitor) {
                    envGrapher.UpdateGraph();
                    Monitor.Wait(graphMonitor, TimeSpan.FromSeconds(1));
                }
            }
        }

    }

}
