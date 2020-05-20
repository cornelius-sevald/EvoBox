using System.Linq;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;
using evobox.UI;

namespace evobox {
    class Program {

        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 800;

        private static bool quit = false;
        private static Simulation sim;
        private static Font font;
        private static Label[] labels;
        private static Slider[] sliders;
        private static Button[] buttons;
        private static InteractabelUIElement[] iUiElements;
        private static UIElement[] uiElements;

        static void Main(string[] args) {

            Initialize();

            // Main loop.
            while (!quit) {
                MainLoop();
            }

            if (sim != null) {
                sim.Stop();
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

            font = new Font(
                    "playfair-display/PlayfairDisplay-Regular.ttf",
                    128
                    );

            // Create the UI.
            labels  = new Label[] {
                // Titel label.
                new Label(3/8.0,   0,      1/4.0, 1/8.0,  "EvoBox", font),
                // "Jumpmen" header label.
                new Label(3/40.0,  1/5.0,  1/3.0, 1/10.0, "JUMPMEN", font),
                // "Enviroment" header label.
                new Label(1/2.0,   1/5.0,  4/9.0, 1/10.0, "ENVIRONMENT", font),
                // Jumpman slider labels.
                new Label(6/40.0,  6/21.0,  1/5.0, 1/20.0, "idle cost", font),
                new Label(5/40.0,  8/21.0,  1/4.0, 1/20.0, "speed cost", font),
                new Label(6/40.0,  10/21.0, 1/5.0, 1/20.0, "size cost", font),
                new Label(3/40.0,  12/21.0, 1/3.0, 1/20.0, "mutation chance",
                        font),
                // Enviroment slider labels.
                new Label(25/40.0, 6/21.0,  1/5.0, 1/20.0, "food rate", font),
            };
            sliders = new Slider[] {
                // Idle cost slider.
                new Slider(3/40.0,  1/3.0,   1/3.0, 1/30.0, 0,   10, 5),
                // Speed cost slider.
                new Slider(3/40.0,  3/7.0,   1/3.0, 1/30.0, 0.1, 3,  1),
                // Size cost slider.
                new Slider(3/40.0,  11/21.0, 1/3.0, 1/30.0, 1,   3,  2),
                // Mutation chance slider.
                new Slider(3/40.0,  13/21.0, 1/3.0, 1/30.0, 0,   5,  1),
                // Food spawn rate slider.
                new Slider(22/40.0, 1/3.0,   1/3.0, 1/30.0, 0,   2,  1)
            };
            buttons = new Button[] {
                new Button(3/24.0, 40/48.0, 1/6.0, 1/10.0, "quit", font,
                        () => quit = true),
                new Button(10/24.0, 40/48.0, 1/6.0, 1/10.0, "reset", font,
                        () => SettingsToSliders(SimulationSettings.
                                                DefaultSettings())),
                new Button(17/24.0, 40/48.0, 1/6.0, 1/10.0, "start", font,
                        () => StartSimulation(SlidersToSettings()))
            };

            iUiElements = sliders
                .OfType<InteractabelUIElement>()
                .Concat(buttons)
                .ToArray();
            uiElements = sliders
                .OfType<UIElement>()
                .Concat(buttons)
                .Concat(labels)
                .ToArray();
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
                            // 'q' stops the simulation.
                            case SDL.SDL_Keycode.SDLK_q:
                                if (sim != null) {
                                    sim.Stop();
                                    sim = null;
                                }
                                break;
                            default:
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

            // Get the mouse coordinates.
            SDL.SDL_GetMouseState(out Globals.mouseX, out Globals.mouseY);

            // Clear the screen.
            renderer.Color = Color.black;
            renderer.Clear();

            // Fill the draw area.
            Globals.renderer.Color = Color.white;
            renderer.FillRect(Globals.viewport);

            if (sim != null) {
                sim.Update(1.0/60.0);
            } else {
                MainMenu();
            }

            renderer.Present();
        }

        static void MainMenu() {
            // Update the UI.
            foreach (InteractabelUIElement ui in iUiElements) {
                ui.Update(Globals.viewport);
            }

            // Draw the UI.
            foreach (UIElement ui in uiElements) {
                ui.Draw(Globals.viewport);
            }
        }

        static void StartSimulation(SimulationSettings settings) {
            sim = new Simulation(settings);
        }

        static SimulationSettings SlidersToSettings() {
            double jumpmanIdleCost     = sliders[0].sliderValue;
            double jumpmanSpeedCost    = sliders[1].sliderValue;
            double jumpmanSizeCost     = sliders[2].sliderValue;
            double jumpmanMutationRate = sliders[3].sliderValue * 0.01;
            double foodSpawnRate       = sliders[4].sliderValue * 0.01;

            return new SimulationSettings(
                    jumpmanIdleCost,
                    jumpmanSpeedCost,
                    jumpmanSizeCost,
                    jumpmanMutationRate,
                    foodSpawnRate
                    );
        }

        static void SettingsToSliders(SimulationSettings settings) {
            sliders[0].sliderValue = settings.jumpmanIdleCost;
            sliders[1].sliderValue = settings.jumpmanSpeedCost;
            sliders[2].sliderValue = settings.jumpmanSizeCost;
            sliders[3].sliderValue = settings.jumpmanMutationRate / 0.01;
            sliders[4].sliderValue = settings.foodSpawnRate       / 0.01;
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
