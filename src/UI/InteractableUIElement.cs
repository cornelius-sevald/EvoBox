using evobox.Graphical;

namespace evobox.UI {

    /// <summary>
    /// Interactable UI elements.
    /// </summary>
    public abstract class InteractabelUIElement : UIElement {

        /// <summary>
        /// Update the UI element.
        /// </summary>
        public abstract void Update(Rect panelRect);

    }

}
