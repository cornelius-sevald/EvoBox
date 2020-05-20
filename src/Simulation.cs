using System;

namespace evobox {

    public sealed class Simulation {

        private static Random rand;
        private static Environment env;
        private static Minimap minimap;

        public Simulation() {
            Initialize();
        }

        public void Update(double deltaTime) {
            // Update the environment.
            env.Update(deltaTime);

            // Draw the minimap.
            minimap.DrawMinimap();

        }

        private void Initialize() {
            // Random number generation.
            rand = new Random();

            // Create the environment.
            env = new Environment(30, 30, new Random(rand.Next()));

            // Add a jumpman to the environment with some energy.
            env.AddObject(
                    new Jumpman(99, new Random(rand.Next()))
                    );

            // Add the camera to the environment.
            Camera cam = new Camera(Vector2.zero, 15, 15, Globals.viewport);
            env.AddObject(cam);

            // Create the minimap.
            minimap = new Minimap(env, cam, Globals.mapRect);
        }
    }

}
