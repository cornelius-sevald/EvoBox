using System;

namespace evobox {

    public sealed class Simulation {

        // Time to 'warm up' the environment before spawning the jumpman.
        const double WARMUP_TIME = 1;

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

            // 'Warm up' the environment.
            for (int i = 0; i < WARMUP_TIME * 60; i++) {
                env.Update(1.0/6.0);
            }

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
