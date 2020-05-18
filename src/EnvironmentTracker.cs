using System;
using System.IO;

namespace evobox {

    /// <summary>
    /// Class that tracks the statistics of an environment.
    /// </summary>
    public sealed class EnvironmentTracker : IDisposable {

        public readonly string nutritionTmpPath;
        public readonly string jumpmanCountTmpPath;
        public readonly string averageJumpmanAttrTmpPath;

        private double maxSpeed = 0; // Debug
        private double maxAvgSpeed = 0; // Debug

        private double nutrition;
        private int jumpmanCount;
        private JumpmanAttributes averageJumpmanAttr;
        private StreamWriter nutritionSW;
        private StreamWriter jumpmanCountSW;
        private StreamWriter averageJumpmanAttrSW;
        private object sceneObjectAddedLock = new object();
        private object sceneObjectRemovedLock = new object();

        public EnvironmentTracker(Environment environment) {
            nutrition = 0;
            jumpmanCount = 0;
            averageJumpmanAttr = new JumpmanAttributes();

            // Create writers for the temporary data files.
            nutritionTmpPath          = Path.GetTempFileName();
            jumpmanCountTmpPath       = Path.GetTempFileName();
            averageJumpmanAttrTmpPath = Path.GetTempFileName();
            nutritionSW          = new StreamWriter(nutritionTmpPath, true);
            jumpmanCountSW       = new StreamWriter(jumpmanCountTmpPath, true);
            averageJumpmanAttrSW = new StreamWriter(averageJumpmanAttrTmpPath, true);

            // Debug
            Console.WriteLine(nutritionTmpPath);
            Console.WriteLine(jumpmanCountTmpPath);
            Console.WriteLine(averageJumpmanAttrTmpPath);

            // Add the event handlers to the environment.
            environment.SceneObjectAdded   += c_SceneObjectAdded;
            environment.SceneObjectRemoved += c_SceneObjectRemoved;
        }

        void LogData<T>(StreamWriter sw, double time, T data) {
            sw.WriteLine("{0}, {1}", time, data);
            sw.Flush();
        }

        /// <summary>
        /// Collect statistics when a <c>SceneObject</c> is added.
        /// </summary>
        void c_SceneObjectAdded(object sender, SceneObjectAddedOrRemovedEventArgs e) {
            lock (sceneObjectAddedLock) {
                if (e.Object is Jumpman j) {
                    // Update and log the jumpman count.
                    jumpmanCount += 1;
                    LogData<int>(jumpmanCountSW, e.Time, jumpmanCount);
                    // Update and log the average attributes.
                    double avgSpeed = averageJumpmanAttr.speed;
                    double avgSize  = averageJumpmanAttr.size;
                    int n = jumpmanCount;
                    avgSpeed += (j.attr.speed - avgSpeed) / n;
                    avgSize  += (j.attr.size  - avgSize)  / n;
                    averageJumpmanAttr.speed = avgSpeed;
                    averageJumpmanAttr.size  = avgSize;
                    LogData<JumpmanAttributes>(averageJumpmanAttrSW, e.Time, averageJumpmanAttr);
                    if (j.attr.speed > maxSpeed) {
                        maxSpeed = j.attr.speed;
                        Console.WriteLine("New max speed:     {0}", maxSpeed);
                    }
                    if (averageJumpmanAttr.speed > maxAvgSpeed) {
                        maxAvgSpeed = averageJumpmanAttr.speed;
                        Console.WriteLine("New max avg speed: {0}", maxAvgSpeed);
                    }
                }
                else if (e.Object is Food f) {
                    nutrition += f.nutrition;
                    LogData<double>(nutritionSW, e.Time, nutrition);
                }
            }
        }

        /// <summary>
        /// Collect statistics when a <c>SceneObject</c> is removed.
        /// </summary>
        void c_SceneObjectRemoved(object sender, SceneObjectAddedOrRemovedEventArgs e) {
            lock (sceneObjectRemovedLock) {
                if (e.Object is Jumpman j) {
                    // Update and log the average attributes.
                    double avgSpeed = 0;
                    double avgSize  = 0;
                    int n = jumpmanCount;
                    if (n > 0) {
                        avgSpeed = averageJumpmanAttr.speed;
                        avgSize  = averageJumpmanAttr.size;
                        avgSpeed = ((avgSpeed * n) - j.attr.speed) / (n - 1);
                        avgSize  = ((avgSize  * n) - j.attr.size)  / (n - 1);
                    }
                    averageJumpmanAttr.speed = avgSpeed;
                    averageJumpmanAttr.size  = avgSize;
                    LogData<JumpmanAttributes>(averageJumpmanAttrSW, e.Time, averageJumpmanAttr);
                    // Update and log the jumpman count.
                    jumpmanCount -= 1;
                    LogData<int>(jumpmanCountSW, e.Time, jumpmanCount);
                }

                else if (e.Object is Food f) {
                    nutrition -= f.nutrition;
                    LogData<double>(nutritionSW, e.Time, nutrition);
                }
            }
        }

        public void Dispose() {
            if (nutritionSW != null) {
                nutritionSW.Dispose();
            }
            if (jumpmanCountSW != null) {
                jumpmanCountSW.Dispose();
            }
        }

    }
}
