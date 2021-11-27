using System;
using System.Drawing; // PointF
using System.Collections.Generic; // List

namespace CubicSplineExtrema {

    // General purpose algorithms. Hence the public accessibility.
    class MathUtils {

        public static void ComputeSecondDerivatives (
            PointF[] inputPoints, out float[] secondDerivs)
        {
            int   i = 0;
            int numPoints = inputPoints.Length;
            float[] mainDiag = new float[numPoints - 2];
            float[] diag = new float[numPoints - 1];
            float[] right = new float[numPoints - 2];

            // Compute the matrix main and off-diagonal values
            // Even though the calling program is suppose to have guaranteed that the
            // input x values are increasing, assert that neither of the diagonal
            // differences are zero to avoid a divide by zero condition.
            for (i = 1; i < numPoints - 1; i++) {
                mainDiag[i-1] = 2.0f * (inputPoints[i+1].X - inputPoints[i-1].X);
                //TODO
                //assert(mainDiag[i-1] > 0);
            }
            for (i = 0; i < numPoints - 1; i++) {
                diag[i] = inputPoints[i+1].X - inputPoints[i].X;
                //TODO
                //assert(diag[i] > 0);
            }

            // Compute right hand side of equation
            for (i = 1; i < numPoints - 1; i++) {
                right[i-1] = 6.0f * ((inputPoints[i+1].Y - inputPoints[i].Y)/
                    diag[i] - (inputPoints[i].Y - inputPoints[i-1].Y ) / diag[i-1]);
            }

            // Forward eliminate tridiagonal
            secondDerivs = new float[numPoints];
            secondDerivs[0] = 0.0f;
            secondDerivs[numPoints - 1] = 0.0f;

            float ftemp;
            for (i = 1; i < numPoints - 2; i++) {
                ftemp = diag[i] / mainDiag[i];
                right[i] -= (right[i-1] * ftemp);
                mainDiag[i] -= (diag[i-1] * ftemp);
            }

            // Backward substitution to solve for second derivative at each knot
            for (i = numPoints - 2; i > 0; i--) {
                secondDerivs[i] = (right[i-1] - diag[i-1] * secondDerivs[i+1]) / mainDiag[i-1];
            }
        }

        /*  
        Given an abscissa (x) location, computes the corresponding cubic spline
        ordinate (y) value.
        */

        public static void ComputeQuadraticRoots(
            float a, float b, float c, out float? x1, out float? x2) 
        {
            float d;   // root algorithm variable 
            x1 = null; // init to null so the caller knows if we did not set this
            x2 = null; // ditto

            d = b * b - 4 * a *c;
            if (d < 0) {
                return;
            }

            d = (float)Math.Sqrt((double)d);
            // Make the result of sqrt the sign of b
            if (b < 0 ) {
                d = -d;
            }
            d = -0.5f * (b + d);

            // Solve for the roots of the quadratic.
            // If both root computations will yield divide by zero ... fahget about it! 
            if (a == 0 && d == 0) {
                return;
            }
            
            // Compute first root if denominator a is not zero 
            if (a != 0) {
                x1 = d / a;
            }

            // Compute second root if denominator d is not zero 
            if (d != 0) {
                x2 = c / d;
            }
        }

        public static void ComputeQuadraticCoefficients(
            PointF[] inputPoints, float[] secondDerivs, int i,
            out float a, out float b, out float c) 
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