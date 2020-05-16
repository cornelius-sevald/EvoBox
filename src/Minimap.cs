using evobox.Graphical;

namespace evobox {

    public class Minimap {

        private readonly Color BG_COLOR = new Color(200, 200, 200, 75);
        private readonly Color OUTLINE_COLOR = new Color(0, 0, 0, 255);

        private Environment environment;
        private Camera camera;
        private Rect mapRect;

        public Minimap(Environment environment, Camera camera, Rect mapRect) {
            this.environment = environment;
            this.camera = camera;
            this.mapRect = mapRect;
        }

        public void DrawMinimap() {
            var renderer = Globals.renderer;
            renderer.Color = BG_COLOR;
            renderer.FillRect(mapRect);

            Rect camRect = new Rect(0, 0, 0, 0);
            camRect.X = (int)((camera.transform.position.x -
                        environment.transform.position.x) /
                    environment.transform.scale.x * mapRect.W);
            camRect.Y = (int)((-camera.transform.position.y -
                        environment.transform.position.y) /
                    environment.transform.scale.y * mapRect.H);
            camRect.W = (int)(camera.transform.scale.x /
                    environment.transform.scale.x * mapRect.W);
            camRect.H = (int)(camera.transform.scale.y /
                    environment.transform.scale.y * mapRect.H);

            camRect.X += (mapRect.W - camRect.W) / 2;
            camRect.Y += (mapRect.H - camRect.H) / 2;

            renderer.Color = OUTLINE_COLOR;
            renderer.DrawRect(camRect);
        }

    }

}
