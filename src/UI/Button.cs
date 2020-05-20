using System;
using System.Linq;

using SDL2;

using evobox.Graphical;

namespace evobox.UI {

    /// <summary>
    /// The three different states a button can be in
    /// </summary>
    public enum ButtonStates {
        /// <summary>
        /// The buttons is able to be moused over (active)
        /// </summary>
        Mouseable,

        /// <summary>
        /// The user is mousing over the button
        /// </summary>
        Moused,

        /// <summary>
        /// The user has pressed the left mouse button while hovering over it
        /// </summary>
        Pressed
    }

    /// <summary>
    /// A UI button that can execute an action when pressed
    /// </summary>
    public class Button : InteractabelUIElement {
        public double x, y, w, h;
        public ButtonStates State { get; private set; }
        private string text;
        private Font font;
        private Action action;
        private Texture textTexture = null;

        /// <summary>
        /// Construct a new button
        /// </summary>
        /// <param name="x">The horizontal offset of the button [0 ; 1]</param>
        /// <param name="y">The vertical offset of the button [0 ; 1]</param>
        /// <param name="w">The width of the button [0 ; 1]</param>
        /// <param name="h">The height of the button [0 ; 1]</param>
        /// <param name="text">The label of the button</param>
        /// <param name="font">The font of the label</param>
        /// <param name="action">The action to execute when
        /// pressed and released</param>
        public Button(double x, double y, double w, double h,
        string text, Font font, Action action) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.text = text;
            this.font = font;
            this.action = action;
            State = ButtonStates.Mouseable;
        }

        /// <summary>
        /// Update the button
        /// </summary>
        /// <param name="panelRect">The rectangle the button resides in</param>
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
            if (_mouseX > x && _mouseX < x + w
                    && _mouseY > y && _mouseY < y + h) {
                hovering = true;
            }
            if (!hovering) {
                State = ButtonStates.Mouseable;
            }
            if (State == ButtonStates.Mouseable && hovering) {
                State = ButtonStates.Moused;
            }
            if (State == ButtonStates.Moused && mouseDown) {
                State = ButtonStates.Pressed;
            }
            if (State == ButtonStates.Pressed && mouseUp) {
                action();
                State = ButtonStates.Moused;
            }
        }

        /// <summary>
        /// Draw the button
        /// </summary>
        /// <param name="panelRect">The rectangle the button resides in</param>
        public override void Draw(Rect panelRect) {
            var renderer = Globals.renderer;

            Rect buttonRect = new Rect(
                    (int)Math.Round(x * panelRect.W + panelRect.X),
                    (int)Math.Round(y * panelRect.H + panelRect.Y),
                    (int)Math.Round(w * panelRect.W),
                    (int)Math.Round(h * panelRect.H)
                    );
            Rect textRect = new Rect(
                    (int)Math.Round(x * panelRect.W + panelRect.X +
                        panelRect.W * 0.0125),
                    (int)Math.Round(y * panelRect.H + panelRect.Y +
                        panelRect.H * 0.003125),
                    (int)Math.Round(w * panelRect.W * 0.9),
                    (int)Math.Round(h * panelRect.H * 0.9)
                    );

            Color buttonColor = new Color(0XAAAAAADD);
            if (State == ButtonStates.Moused) {
                buttonColor = new Color(0X888888DD);
            } else if (State == ButtonStates.Pressed) {
                buttonColor = new Color(0X666666DD);
            }
            renderer.Color = buttonColor;
            renderer.FillRect(buttonRect);

            // Draw the text
            if (textTexture == null) {
                using (
                        Surface textSurf =
                        font.TextSurface(text, Graphics.black)
                ) {
                    textTexture = new Texture(renderer, textSurf);
                }
            }
            renderer.RenderTexture(textTexture, textRect, null);
        }
    }
}
