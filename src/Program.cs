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

            labels  = new Label[] {
                new Label(3/8.0, 0, 1/4.0, 1/8.0, "evobox", font),
            };
            sliders = new Slider[] {
                new Slider(3/8.0, 8/24.0, 1/4.0, 1/32.0, 0, 10, 5)
            };
            buttons = new Button[] {
                new Button(3/8.0, 16/24.0, 1/4.0, 1/8.0, "quit", font,
                        () => quit = true)
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

            // Get the mouse coordinates.
            SDL.SDL_GetMouseState(out Globals.mouseX, out Globals.mouseY);

            // Clear the screen.
            renderer.Color = Color.black;
            renderer.Clear();

            // Fill the draw area.
            Globals.renderer.Color = Color.white;
            renderer.FillRect(Globals.viewport);

            // Update the UI.
            foreach (InteractabelUIElement ui in iUiElements) {
                ui.Update(Globals.viewport);
            }

            // Draw the UI.
            foreach (UIElement ui in uiElements) {
                ui.Draw(Globals.viewport);
            }

            // The edges outside the viewport.
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
                    leftSide.W + 1,
                    screenRect.H
            );
            Rect topSide = new Rect(
                    0,
                    0,
                    screenRect.W,
                    (screenRect.H - Globals.viewport.H) / 2
            );
            Rect bottomSide = new Rect(
                    0,
                    topSide.H + Globals.viewport.H,
                    screenRect.W,
                    topSide.H + 1
            );

            renderer.Color = Color.black;
            renderer.FillRect(leftSide);
            renderer.FillRect(rightSide);
            renderer.FillRect(topSide);
            renderer.FillRect(bottomSide);

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
