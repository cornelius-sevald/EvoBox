using System;
using System.Linq;
using System.Collections.Generic;

using SDL2;

using evobox.Graphical;

namespace evobox {

    /// <summary>
    /// A camera draws entities onto the screen.
    /// </summary>
    public class Camera : SceneObject {

        private readonly Vector2 MIN_SCALE = new Vector2(1, 1);
        private const double MOVE_SPEED = 10;
        private const double ZOOM_SPEED = -100;

        public Rect drawRect;

        /// <summary>
        /// Construct a new camera given a position, viewport width and height.
        /// </summary>
        /// <param name="position">The position of the camera.</param>
        /// <param name="width">The width of the camera viewport in world units.</param>
        /// <param name="height">The height of the camera viewport in world units.</param>
        /// <param name="drawRect">The screen rectangle the camera draws onto.</param>
        public Camera(Vector2 position, double width, double height, Rect drawRect)
            : base(position, new Vector2(width, height)) {
            this.drawRect = drawRect;
        }

        public override void Update(double deltaTime) {
            // Draw the entities in the scene.
            Draw(Globals.renderer, drawRect, environment);
            Draw(Globals.renderer, drawRect, environment.entities);

            var kb = Globals.keyboard;

            Vector2 moveDir = Vector2.zero;
            int zoomDir = 0;

            moveDir.y =
               (kb[SDL.SDL_Scancode.SDL_SCANCODE_UP]   |
                kb[SDL.SDL_Scancode.SDL_SCANCODE_W])   -
               (kb[SDL.SDL_Scancode.SDL_SCANCODE_DOWN] |
                kb[SDL.SDL_Scancode.SDL_SCANCODE_S]);
            moveDir.x =
               (kb[SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] |
                kb[SDL.SDL_Scancode.SDL_SCANCODE_D])    -
               (kb[SDL.SDL_Scancode.SDL_SCANCODE_LEFT]  |
                kb[SDL.SDL_Scancode.SDL_SCANCODE_A]);

            foreach (SDL.SDL_Event e in Globals.eventQueue) {
                if (e.type == SDL.SDL_EventType.SDL_MOUSEWHEEL) {
                    if (e.wheel.y > 0) {
                        zoomDir =  1;
                    }
                    else if (e.wheel.y < 0) {
                        zoomDir = -1;
                    }
                }
            }

            Move(moveDir * MOVE_SPEED * deltaTime);
            Zoom(zoomDir * ZOOM_SPEED * deltaTime);
        }

        private void Move(Vector2 amount) {
            // Move the camera by the specified amount.
            transform.position += amount;
            Vector2 max = (environment.transform.scale - transform.scale) / 2;
            Vector2 min = -max;
            // Restrict the cameras position to be inside the environment.
            transform.position = transform.position.Clamp(min, max);
        }

        private void Zoom(double amount) {
            // Scale the camera by the specified amount.
            transform.scale += Vector2.one * amount;
            Vector2 max = environment.transform.scale;
            Vector2 min = MIN_SCALE;
            // Restrict the cameras scale.
            transform.scale = transform.scale.Clamp(min, max);

            // Restrict the cameras position efter zooming.
            Move(Vector2.zero);
        }

        /// <summary>
        /// Draw a list of entities onto a rectangle on the screen.
        /// </summary>
        /// <param name="renderer">The renderer drawing onto the window.</param>
        /// <param name="drawRect">The part of the screen to draw onto.</param>
        /// <param name="entities">The entities to draw.</param>
        private void Draw(Renderer renderer, Rect drawRect, List<Entity> entities) {
            List<Entity> zEntities = entities.OrderBy(e => e.zIndex).ToList();
            foreach (Entity entity in zEntities) {
                Draw(renderer, drawRect, entity);
            }
        }

        private void Draw(Renderer renderer, Rect drawRect, Entity entity) {
            double xc = transform.position.x;           // Camera X position
            double yc = transform.position.y;           // Camera Y position
            double wc = transform.scale.x;              // Camera width.
            double hc = transform.scale.y;              // Camera height.
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
