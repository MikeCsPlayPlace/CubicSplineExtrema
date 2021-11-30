using System;
using System.Collections.Generic; // List
using System.Drawing; // PointF

namespace CubicSplineExtrema {

    // General purpose algorithms. Hence the public accessibility.
    class MathUtils {

        public static void ComputeSecondDerivatives (
            PointF[] inputPoints, out double[] secondDerivs)
        {
            int   i = 0;
            int numPoints = inputPoints.Length;
            double[] mainDiag = new double[numPoints - 2];
            double[] diag = new double[numPoints - 1];
            double[] right = new double[numPoints - 2];

            // Compute the matrix main and off-diagonal values
            // Even though the calling program is suppose to have guaranteed that the
            // input x values are increasing, assert that neither of the diagonal
            // differences are zero to avoid a divide by zero condition.

            for (i = 1; i < numPoints - 1; i++) {
                mainDiag[i-1] = 2.0 * (inputPoints[i+1].X - inputPoints[i-1].X);
            }
            for (i = 0; i < numPoints - 1; i++) {
                diag[i] = inputPoints[i+1].X - inputPoints[i].X;
            }

            // Compute right hand side of equation
            for (i = 1; i < numPoints - 1; i++) {
                right[i-1] = 6.0 * ((inputPoints[i+1].Y - inputPoints[i].Y)/
                    diag[i] - (inputPoints[i].Y - inputPoints[i-1].Y ) / diag[i-1]);
            }

            // Forward eliminate tridiagonal
            secondDerivs = new double[numPoints];
            secondDerivs[0] = 0.0;
            secondDerivs[numPoints - 1] = 0.0;

            double temp;
            for (i = 1; i < numPoints - 2; i++) {
                temp = diag[i] / mainDiag[i];
                right[i] -= (right[i-1] * temp);
                mainDiag[i] -= (diag[i-1] * temp);
            }

            // Backward substitution to solve for second derivative at each knot
            for (i = numPoints - 2; i > 0; i--) {
                secondDerivs[i] = (right[i-1] - diag[i-1] * secondDerivs[i+1]) / mainDiag[i-1];
            }

        }

        // Given an abscissa (x) location, compute the corresponding cubic spline ordinate (y) value.
        public static void ComputeQuadraticRoots(
            double a, double b, double c, out float? x1, out float? x2) 
        {
            double d;   // root algorithm variable 
            x1 = null; // init to null so the caller knows if we did not set this
            x2 = null; // ditto

            d = b * b - 4 * a *c;
            if (d < 0) {
                return;
            }

            d = Math.Sqrt(d);
            // Make the result of sqrt the sign of b
            if (b < 0 ) {
                d = -d;
            }
            d = -0.5 * (b + d);

            // Solve for the roots of the quadratic.
            // If both root computations will yield divide by zero ... fahget about it! 
            if (a == 0 && d == 0) {
                return;
            }
            
            // Compute first root if denominator a is not zero. 
            // Note that these are not throwable situations, but instead how to test for a valid root.
            if (a != 0) {
                x1 = (float)(d / a);
            }

            // Compute second root if denominator d is not zero 
            if (d != 0) {
                x2 = (float)(c / d);
            }
        }

        public static void ComputeQuadraticCoefficients(
            PointF[] inputPoints, double[] secondDerivs, int i,
            out double a, out double b, out double c) 
        {

            a = 3 * (secondDerivs[i+1] - secondDerivs[i]);
            b = 6 * (inputPoints[i+1].X * secondDerivs[i] - inputPoints[i].X * secondDerivs[i+1]);
            c = 6 * (inputPoints[i+1].Y - inputPoints[i].Y) - 
                (2 * inputPoints[i+1].X * inputPoints[i+1].X - inputPoints[i].X * 
                inputPoints[i].X + 2 * inputPoints[i].X * inputPoints[i+1].X) * secondDerivs[i];
            c -= (inputPoints[i+1].X * inputPoints[i+1].X - 2 * inputPoints[i].X * 
                inputPoints[i+1].X - 2 * inputPoints[i].X * inputPoints[i].X) * secondDerivs[i+1];

        }
    }
}