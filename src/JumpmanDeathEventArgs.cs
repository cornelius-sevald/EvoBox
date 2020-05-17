using System;

namespace evobox {

    public class JumpmanDeathEventArgs : EventArgs {

        public Jumpman Deceased { get; set; }
        public string DeathReason { get; set; }
        public double TimeOfDeath { get; set; }

    }

}
