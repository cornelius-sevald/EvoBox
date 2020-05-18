using System;

namespace evobox {

    public sealed class Simulation {

        public static Simulation Instance { get; private set; }

        // Time to 'warm up' the environment before spawning the jumpman.
        const double WARMUP_TIME = 1;

        public SimulationSettings settings { get; private set; }

        private Random rand;
        private Environment env;
        private Minimap minimap;
        private EnvironmentTracker envTracker;

        public Simulation(SimulationSettings settings) {
            Instance = this;
            this.settings = settings;

            // Random number generation.
            this.rand = new Random();

            // Create the environment.
            this.env = new Environment(30, 30, new Random(rand.Next()));

            // 'Warm up' the environment.
            for (int i = 0; i < WARMUP_TIME * 60; i++) {
                env.Update(1.0/6.0);
            }

            // Add a jumpman to the environment with some energy.
            this.env.AddObject(
                    new Jumpman(99, new Random(rand.Next()))
                    );

            // Add the camera to the environment.
            Camera cam = new Camera(Vector2.zero, 15, 15, Globals.viewport);
            this.env.AddObject(cam);

            // Create the minimap.
            this.minimap = new Minimap(env, cam, Globals.mapRect);

            // Create the environment tracker.
            envTracker = new EnvironmentTracker(env);
        }

        public void Update(double deltaTime) {
            // Update the environment.
            env.Update(deltaTime);

            // Draw the minimap.
            minimap.DrawMinimap();

        }

    }

}
