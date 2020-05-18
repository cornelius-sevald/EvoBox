using System;
using AwokeKnowing.GnuplotCSharp;

namespace evobox {

    /// <summary>
    /// The <c>EnvironmentGrapher</c> graphs data from an <c>EnvironmentTracker</c>.
    /// </summary>
    public class EnvironmentGrapher {

        public GraphTypes graphType;
        private EnvironmentTracker tracker;

        /// <summary>
        /// Create a new environment grapher.
        /// </summary>
        public EnvironmentGrapher(EnvironmentTracker tracker, GraphTypes graphType) {
            this.tracker = tracker;
            this.graphType = graphType;
        }

        public void UpdateGraph () {
            switch (graphType) {
                case GraphTypes.NutritionAndJumpmen:
                    PlotNutritionAndJumpmen();
                    break;
                case GraphTypes.AverageSpeedAndSize:
                    PlotAverageSpeedAndSize();
                    break;
                default:
                    break;
            }
        }

        void PlotNutritionAndJumpmen() {
            GnuPlot.Set("xlabel 'Time [Seconds]'");
            GnuPlot.Set("ylabel 'Total nutrition'");
            GnuPlot.Set("y2label 'Amount of jumpmen'");
            GnuPlot.Set("ytics nomirror");
            GnuPlot.Set("y2tics");
            GnuPlot.Set("tics out");
            GnuPlot.Unset("yrange");
            GnuPlot.Unset("y2range");
            //GnuPlot.Set("autoscale y");
            //GnuPlot.Set("autoscale y2");
            GnuPlot.Set("key left top Left box 3");

            GnuPlot.HoldOn();
            GnuPlot.Plot(tracker.nutritionTmpPath, "using 1:2 title 'Nutrition' w l axes x1y1");
            GnuPlot.Plot(tracker.jumpmanCountTmpPath, "using 1:2 title 'Jumpmen' w lp axes x1y2");
            GnuPlot.HoldOff();
        }

        void PlotAverageSpeedAndSize() {
            GnuPlot.Set("xlabel 'Time [Seconds]'");
            GnuPlot.Set("ylabel 'Avg. speed'");
            GnuPlot.Set("y2label 'Avg. size'");
            GnuPlot.Set("ytics nomirror");
            GnuPlot.Set("y2tics");
            GnuPlot.Set("tics out");
            GnuPlot.Set(String.Format("yrange [{0}:{1}]", JumpmanAttributes.MIN_SPEED, JumpmanAttributes.MAX_SPEED));
            GnuPlot.Set(String.Format("y2range [{0}:{1}]", JumpmanAttributes.MIN_SIZE, JumpmanAttributes.MAX_SIZE));
            //GnuPlot.Set("autoscale y");
            //GnuPlot.Set("autoscale y2");
            GnuPlot.Set("key left top Left box 3");

            GnuPlot.HoldOn();
            GnuPlot.Plot(tracker.averageJumpmanAttrTmpPath, "using 1:5 title 'Speed' w lp axes x1y1");
            GnuPlot.Plot(tracker.averageJumpmanAttrTmpPath, "using 1:6 title 'Size'  w lp axes x1y2");
            GnuPlot.HoldOff();
        }
    }

    public enum GraphTypes {
        NutritionAndJumpmen,
        AverageSpeedAndSize
    }

    public static class GraphTypesExtentions {

        public static GraphTypes Next(this GraphTypes graphType) {
            if (graphType == GraphTypes.AverageSpeedAndSize) {
                return GraphTypes.NutritionAndJumpmen;
            }
            return (GraphTypes)((int)graphType + 1);
        }

        public static GraphTypes Prev(this GraphTypes graphType) {
            if (graphType == GraphTypes.NutritionAndJumpmen) {
                return GraphTypes.AverageSpeedAndSize;
            }
            return (GraphTypes)((int)graphType - 1);
        }
    }

}
