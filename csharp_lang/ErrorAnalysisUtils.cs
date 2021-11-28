using System;
using System.Drawing; // PointF

namespace CubicSplineExtrema {

    // General purpose error computation formula. Hence the public accessibility.
    class ErrorAnalysisUtils {

        // Determine the hypotenuse of the X and Y errors
        public static float ComputeCompositeAbsRelativePercentDiff(PointF error) {
            return (float)Math.Sqrt(error.X * error.X + error.Y * error.Y);
        }

        internal static PointF ComputeAbsRelativePercentDiff(PointF computed, PointF expected) {
            PointF error = new PointF();
            error.X = ComputeAbsRelativePercentDiff(computed.X, expected.X);
            error.Y = ComputeAbsRelativePercentDiff(computed.Y, expected.Y);
            return error;
        }

        // Compute modified absolute Relative Percent Difference (aRPD)
        internal static float ComputeAbsRelativePercentDiff(double computed, double expected) {
            // aRPD = 2|𝑎−𝑏| / |𝑎|+|𝑏|
            double denominator = Math.Abs(computed) + Math.Abs(expected);
            if (denominator==0.0) {
                return 0.0f;
            }
            // modification to handle small denominators that cause misleading results
            if (denominator<1.0) {
                denominator += 1.0;
            }
            return (float)(2.0 * Math.Abs(computed - expected) / denominator * 100.0);
        }
    }
}