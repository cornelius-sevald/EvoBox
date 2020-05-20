using evobox.Graphical;

namespace evobox.UI {

    /// <summary>
    /// Visual UI element.
    /// </summary>
    public abstract class UIElement {

        /// <summary>
        /// Draw the UI element in the panel rectangle.
        /// </summary>
        public abstract void Draw(Rect panelRect);

    }

}
