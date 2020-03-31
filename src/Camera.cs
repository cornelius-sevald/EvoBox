using System;
using System.Linq;
using System.Collections.Generic;
using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// A camera draws entities onto the screen.
    /// </summary>
    public class Camera : SceneObject {

        public double width, height;

        /// <summary>
        /// Construct a new camera given a position, viewport width and height.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="width">The width of the camera viewport in world units.</param>
        /// <param name="height">The height of the camera viewport in world units.</param>
        public Camera(Vector2 position, double width, double height)
            : base(position, Vector2.one) {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Draw a list of entities onto a rectangle on the screen.
        /// </summary>
        /// <param name="renderer">The renderer drawing onto the window.</param>
        /// <param name="drawRect">The part of the screen to draw onto.</param>
        /// <param name="entities">The entities to draw.</param>
        public void Draw(Renderer renderer, Rect drawRect, List<Entity> entities) {
            List<Entity> zEntities = entities.OrderBy(e => e.zIndex).ToList();
            foreach (Entity entity in zEntities) {
                Draw(renderer, drawRect, entity);
            }
        }

        public void Draw(Renderer renderer, Rect drawRect, Entity entity) {
            double xc = transform.position.x;           // Camera X position
            double yc = transform.position.y;           // Camera Y position
            double wc = width;                          // Camera width.
            double hc = height;                         // Camera height.
            double xe = entity.transform.position.x;    // Entity X position
            double ye = entity.transform.position.y;    // Entity Y position
            double we = entity.transform.scale.x;       // Entity width.
            double he = entity.transform.scale.y;       // Entity height.
            int xd = drawRect.X;                        // Draw rect X position.
            int yd = drawRect.Y;                        // Draw rect Y position.
            int wd = drawRect.W;                        // Draw rect width.
            int hd = drawRect.H;                        // Draw rect height.

            int w = (int)Math.Round(wd * we / wc);
            int h = (int)Math.Round(hd * he / hc);
            int x = (int)Math.Round((xe - xc - (we - wc) / 2) * (wd / wc) + xd);
            int y = (int)Math.Round(-(ye - yc - (hc - he) / 2) * (hd / hc) + yd);

            Rect dst = new Rect(x, y, w, h);
            renderer.RenderTexture(entity.texture, dst, null);
        }
    }

}
