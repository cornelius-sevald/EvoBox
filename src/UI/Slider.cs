using System;
using System.Linq;

using SDL2;

using evobox.Graphical;

namespace evobox.UI {

    /// <summary>
    /// The three different states a slider can be in
    /// </summary>
    public enum SliderStates {
        /// <summary>
        /// The slider is able to be moused over (active)
        /// </summary>
        Mouseable,

        /// <summary>
        /// The user is mousing over the slider
        /// </summary>
        Moused,

        /// <summary>
        /// The user has pressed the left mouse button while hovering over it
        /// </summary>
        Pressed
    }

    /// <summary>
    /// A UI slider that can be slid between two values.
    /// </summary>
    public class Slider : InteractabelUIElement {
        public double x, y, w, h;
        public double sliderValue;
        public double minValue;
        public double maxValue;
        public SliderStates State { get; private set; }

        /// <summary>
        /// Construct a new slider
        /// </summary>
        /// <param name="x">The horizontal offset of the slider [0 ; 1]</param>
        /// <param name="y">The vertical offset of the slider [0 ; 1]</param>
        /// <param name="w">The width of the slider [0 ; 1]</param>
        /// <param name="h">The height of the slider [0 ; 1]</param>
        /// <param name="text">The label of the slider</param>
        /// <param name="minValue">The minimal slider value</param>
        /// <param name="maxValue">The maximum slider value</param>
        /// <param name="startValue">The initial slider value</param>
        public Slider(double x, double y, double w, double h,
                double minValue, double maxValue, double startValue) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.sliderValue = startValue;
            State = SliderStates.Mouseable;
        }

        /// <summary>
        /// Update the slider
        /// </summary>
        /// <param name="mouseX">The x-position of the mouse</param>
        /// <param name="mouseY">The y-position of the mouse</param>
        /// <param name="panelRect">The rectangle the slider resides in</param>
        public override void Update(Rect panelRect) {

            int mouseX = Globals.mouseX;
            int mouseY = Globals.mouseY;
            var events = Globals.eventQueue;

            double _mouseX = (mouseX - panelRect.X) / (double)panelRect.W;
            double _mouseY = (mouseY - panelRect.Y) / (double)panelRect.H;

            bool hovering = false;
            bool mouseUp = (events.Any(e =>
                        e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP));
            bool mouseDown = (events.Any(e =>
                        e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN));
            if (_mouseX > x - h/2 && _mouseX < x + h/2 + w &&
                    _mouseY > y && _mouseY < y + h) {
                hovering = true;
            }
            if (!hovering && State != SliderStates.Pressed) {
                State = SliderStates.Mouseable;
            }
            if (State == SliderStates.Mouseable && hovering) {
                State = SliderStates.Moused;
            }
            if (State == SliderStates.Moused && mouseDown) {
                State = SliderStates.Pressed;
            }
            if (State == SliderStates.Pressed && mouseUp) {
                State = SliderStates.Moused;
            }
            if (State == SliderStates.Pressed) {
                sliderValue = Math.Clamp((_mouseX - x) / w, 0, 1) *
                    (maxValue - minValue) + minValue;
            }
        }

        /// <summary>
        /// Draw the slider
        /// </summary>
        /// <param name="panelRect">The rectangle the slider resides in</param>
        public override void Draw(Rect panelRect) {
            var renderer = Globals.renderer;

            double n = (sliderValue - minValue) / (maxValue - minValue);
            Rect bobRect = new Rect(
                    (int)Math.Round((x + w * n - h/2) * panelRect.W +
                        panelRect.X),
                    (int)Math.Round(y * panelRect.H +
                        panelRect.Y),
                    (int)Math.Round(h * panelRect.H),
                    (int)Math.Round(h * panelRect.H)
                    );

            Color bobColor = new Color(0XAAAAAADD);
            if (State == SliderStates.Moused) {
                bobColor = new Color(0X888888DD);
            } else if (State == SliderStates.Pressed) {
                bobColor = new Color(0X666666DD);
            }
            renderer.Color = Color.black;
            renderer.DrawLine(
                    (int)(x * panelRect.W + panelRect.X),
                    (int)((y + h / 2) * panelRect.H + panelRect.Y),
                    (int)((x + w) * panelRect.W + panelRect.X),
                    (int)((y + h / 2) * panelRect.H + panelRect.Y)
                    );
            renderer.Color = bobColor;
            renderer.FillRect(bobRect);
        }
    }
}

