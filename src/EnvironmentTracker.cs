using System.Linq;
using System.Collections.Generic;

namespace evobox {

    /// <summary>
    /// Class that tracks the statistics of an environment.
    /// </summary>
    public class EnvironmentTracker {

        // A timeline of the nutrition available in the environment.
        public List<TimedEntry<double>> nutritionTimeline;
        // A timeline of the amount of living jumpmen.
        public List<TimedEntry<int>> jumpmanCountTimeline;
        // A timeline of the average jumpman attributes.
        public List<TimedEntry<JumpmanAttributes>> jumpmanAttrTimeline;
        // A list of the current attributes of jumpmen.
        public List<JumpmanAttributes> jumpmanAttributes;

        public EnvironmentTracker(Environment environment) {
            nutritionTimeline    = new List<TimedEntry<double>>();
            jumpmanCountTimeline = new List<TimedEntry<int>>();
            jumpmanAttrTimeline  = new List<TimedEntry<JumpmanAttributes>>();
            jumpmanAttributes    = new List<JumpmanAttributes>();

            // Add initial values.
            nutritionTimeline.Add(new TimedEntry<double>(0, 0));
            jumpmanCountTimeline.Add(new TimedEntry<int>(0, 0));

            // Add the event handlers to the environment.
            environment.SceneObjectAdded   += c_SceneObjectAdded;
            environment.SceneObjectRemoved += c_SceneObjectRemoved;
        }

        /// <summary>
        /// Collect statistics when a <c>SceneObject</c> is added.
        /// </summary>
        void c_SceneObjectAdded(object sender, SceneObjectAddedOrRemovedEventArgs e) {
            if (e.Object is Jumpman j) {
                // Update the jumpman count timeline...
                var prevEntry = jumpmanCountTimeline.Last();
                var entry = new TimedEntry<int>(e.Time, prevEntry.entry + 1);
                jumpmanCountTimeline.Add(entry);
            }

            else if (e.Object is Food f) {
                // Update the nutrition timeline...
                var prevEntry = nutritionTimeline.Last();
                var entry = new TimedEntry<double>(e.Time, prevEntry.entry + f.nutrition);
                nutritionTimeline.Add(entry);
            }
        }

        /// <summary>
        /// Collect statistics when a <c>SceneObject</c> is removed.
        /// </summary>
        void c_SceneObjectRemoved(object sender, SceneObjectAddedOrRemovedEventArgs e) {
            if (e.Object is Jumpman j) {
                // Update the jumpman count timeline...
                var prevEntry = jumpmanCountTimeline.Last();
                var entry = new TimedEntry<int>(e.Time, prevEntry.entry - 1);
                jumpmanCountTimeline.Add(entry);
            }

            else if (e.Object is Food f) {
                // Update the nutrition timeline...
                var prevEntry = nutritionTimeline.Last();
                var entry = new TimedEntry<double>(e.Time, prevEntry.entry - f.nutrition);
                nutritionTimeline.Add(entry);
            }
        }

    }

    /// <summary>
    /// An entry with a timestamp attatched.
    /// </summary>
    public struct TimedEntry<T> {
        public double time;
        public T entry;

        public TimedEntry(double time, T entry) {
            this.time = time;
            this.entry = entry;
        }
    }

}
