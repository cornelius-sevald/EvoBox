namespace evobox {

    public struct SimulationSettings {

        const double DEFAULT_JUMPMAN_IDLE_COST     = 5;
        const double DEFAULT_JUMPMAN_SPEED_COST    = 1;
        const double DEFAULT_JUMPMAN_SIZE_COST     = 2;
        const double DEFAULT_JUMPMAN_MUTATION_RATE = 0.01;
        const double DEFAULT_FOOD_SPAWNRATE        = 0.01;

        public double jumpmanIdleCost;
        public double jumpmanSpeedCost;
        public double jumpmanSizeCost;
        public double jumpmanMutationRate;
        public double foodSpawnRate;

        public SimulationSettings(
                double jumpmanIdleCost, double jumpmanSpeedCost,
                double jumpmanSizeCost, double jumpmanMutationRate,
                double foodSpawnRate
        ) {
            this.jumpmanIdleCost     = jumpmanIdleCost;
            this.jumpmanSpeedCost    = jumpmanSpeedCost;
            this.jumpmanSizeCost     = jumpmanSizeCost;
            this.jumpmanMutationRate = jumpmanMutationRate;
            this.foodSpawnRate       = foodSpawnRate;
        }

        public static SimulationSettings DefaultSettings() {
            return new SimulationSettings(
                    DEFAULT_JUMPMAN_IDLE_COST,
                    DEFAULT_JUMPMAN_SPEED_COST,
                    DEFAULT_JUMPMAN_SIZE_COST,
                    DEFAULT_JUMPMAN_MUTATION_RATE,
                    DEFAULT_FOOD_SPAWNRATE
                    );
        }
    }

}

