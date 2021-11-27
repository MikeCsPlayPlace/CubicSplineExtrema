using System;
using System.IO; // TextWriter
using System.Drawing; // PointF
using System.Collections.Generic; // List

namespace CubicSplineExtrema {
    class CubicSplineExtrema {
        static void Main(string[] args) {


            string dataFile;

            Console.Write("Enter name of the input file: ");
            dataFile = Console.ReadLine();
            
            List<PointF> inputPoints = CsvFileReader(dataFile);
            List<PointF> extremaPoints;

            Console.WriteLine("\nInput points: " + inputPoints.Count);
            //using foreach loop
            foreach(var point in inputPoints)
            {
                Console.WriteLine(point);
            }

            // Call the method that does the heavy lifting
            CubicSplineExtrema extrema = new CubicSplineExtrema();
            extrema.ComputeExtrema(inputPoints.ToArray(), out extremaPoints);

            // print the resulting list of extrema 
            Console.WriteLine("\nExtrema computed: " + extremaPoints.Count);
            foreach(var point in extremaPoints) {
                Console.WriteLine(point);
            }

            // TODO: put this is a separate method as part of unit testing
            // And get the expected results
            String expectedResultsFile = dataFile.Replace("csv", "expected_output.csv");
            List<PointF> expectedPoints = CsvFileReader(expectedResultsFile);

            bool doesExtremaCountMatch = true;
            if (expectedPoints.Count!=extremaPoints.Count) {
                doesExtremaCountMatch =false;
                Console.WriteLine("Error: The number of extrema computed does not match expected.");
                Console.WriteLine("Therefore unable to compute percent errors for each pair.");
            }
            Console.WriteLine("\nExtrema expected: " + expectedPoints.Count);

            int i = 0;
            float avgCompositeError = 0.0f;
            foreach(var point in expectedPoints) {
                Console.Write(point);
                // Make sure we have a 1-to-1 set of xy pairs
                if (doesExtremaCountMatch) {
                    PointF error = ComputeAbsRelativePercentDiff(extremaPoints[i], point);
                    Console.Write($"   % aPRD = {error.X}, {error.Y} ");
                    float compositeError = ComputeCompositeAbsRelativePercentDiff(error);
                    Console.Write($"  Composite={compositeError}%");
                    avgCompositeError += compositeError;
                    i++;
                }
                Console.Write("\n");
            }
            if (doesExtremaCountMatch) {
                avgCompositeError = avgCompositeError/expectedPoints.Count;
                Console.WriteLine($"Average Composite Error = {avgCompositeError}%");
            }
        }

        // Determine the hypotenuse of the X and Y errors
        static float ComputeCompositeAbsRelativePercentDiff(PointF error) {
            return (float)Math.Sqrt(error.X * error.X + error.Y * error.Y);
        }

        static PointF ComputeAbsRelativePercentDiff(PointF computed, PointF expected) {
            PointF error = new PointF();
            error.X = ComputeAbsRelativePercentDiff(computed.X, expected.X);
            error.Y = ComputeAbsRelativePercentDiff(computed.Y, expected.Y);
            return error;
        }

        static float ComputeAbsRelativePercentDiff(float computed, float expected) {
            // absolute Relative Percent Difference (aRPD), where
            // aRPD = 2|𝑎−𝑏| / |𝑎|+|𝑏|
            float denominator = Math.Abs(computed) + Math.Abs(expected);
            if (denominator==0.0f) {
                return 0.0f;
            }
            // modification to handle small denominators that cause misleading results
            if (denominator<1.0f) {
                denominator += 1.0f;
            }
            return 2.0f * Math.Abs(computed - expected) / denominator * 100.0f;
        }

        static List<PointF> CsvFileReader(String dataFile) {

            // Note: using List because it preserves order
            List<PointF> points = new List<PointF>();

            TextWriter errorWriter = Console.Error;
            string[] lines = null;
            try {
                lines = System.IO.File.ReadAllLines(@dataFile);
            } catch (Exception e) {
                errorWriter.WriteLine(e.Message);
                System.Environment.Exit(1);
            }

            if (lines.Length==0) {
                errorWriter.WriteLine($"No lines were read from the input file {dataFile}. Terminating.");
                System.Environment.Exit(1);
            }

            foreach (string line in lines) {

                string[] xyPoint = line.Split(',');
                if (xyPoint.Length!=2) {
                    Console.WriteLine($"Invalid format in {dataFile} at line={line}. Expected something like:");
                    Console.WriteLine("\t14, 2\tor\t14.1,2.0");
                    if (xyPoint.Length>2) {
                        Console.WriteLine($"You may have unexpected commas within your values (e.g. 1,400). Please remove them.");
                    }
                    System.Environment.Exit(1);
                }
                
                try {
                    float x = float.Parse(xyPoint[0]);
                    float y = float.Parse(xyPoint[1]);
                    points.Add(new PointF {X=x, Y=y});
                }  catch (FormatException) {
                    Console.WriteLine($"Error in {dataFile} converting point {xyPoint} into floats.");
                    System.Environment.Exit(1);
                }
            }
            return points;
        }

        void ComputeSecondDerivatives (
            PointF[] inputPoints, 
            out float[] secondDerivs
            )
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
                //assert(mainDiag[i-1] > 0);
            }
            for (i = 0; i < numPoints - 1; i++) {
                diag[i] = inputPoints[i+1].X - inputPoints[i].X;
                //assert(diag[i] > 0);
            }

            // compute right hand side of equation
            for (i = 1; i < numPoints - 1; i++) {
                right[i-1] = 6.0f * ((inputPoints[i+1].Y - inputPoints[i].Y)/
                    diag[i] - (inputPoints[i].Y - inputPoints[i-1].Y ) / diag[i-1]);
            }

            // forward eliminate tridiagonal
            secondDerivs = new float[numPoints];
            secondDerivs[0] = 0.0f;
            secondDerivs[numPoints - 1] = 0.0f;

            float ftemp;
            for (i = 1; i < numPoints - 2; i++) {
                ftemp = diag[i] / mainDiag[i];
                right[i] -= (right[i-1] * ftemp);
                mainDiag[i] -= (diag[i-1] * ftemp);
            }

            // backward substitution to solve for second derivative at each knot
            for (i = numPoints - 2; i > 0; i--) {
                secondDerivs[i] = (right[i-1] - diag[i-1] * secondDerivs[i+1]) / mainDiag[i-1];
            }
        }

        /*  
        Given an abscissa (x) location, computes the corresponding cubic spline
        ordinate (y) value.
        */

        void ComputeY (
            int i,             // input - array index
            float xValue,      // input - x value at which to solve for y
            PointF[] inputPoints,     // input - array of y values
            float[] secondDerivs, // input - array of second derivatives of each data interval
            out float yValue)   // output - address of y extreme value at x
        {
            float A, B, C, D; // cubic spline coefficients
            //TODO: should we check for xValue of null coming in, 
            // and also be able to return a null yValue?
            // compute the standard cubic spline coefficients
            A = (inputPoints[i + 1].X - xValue) / (inputPoints[i + 1].X - inputPoints[i].X);
            B = 1 - A;
            float ftemp = (float) Math.Pow((double)(inputPoints[i + 1].X - inputPoints[i].X), 2.0f) / 6.0f;
            C = (A * A * A - A) * ftemp;
            D = (B * B * B - B) * ftemp;

            // compute the ordinate value at the abscissa location
            yValue = A * inputPoints[i].Y + B * inputPoints[i + 1].Y + C * secondDerivs[i] + D * secondDerivs[i + 1];
        }

        void FindQuadraticRoots(float a, float b, float c, out float? x1, out float? x2) 
        {
            float d;   // root algorithm variable 
            x1 = null; // init to null so the caller knows if we did not set this
            x2 = null; // ditto

            d = b * b - 4 * a *c;
            if (d < 0) {
                return;
            }

            d = (float)Math.Sqrt((double)d);
            // make the result of sqrt the sign of b
            if (b < 0 ) {
                d = -d;
            }
            d = -0.5f * (b + d);

            // Solve for the roots of the quadratic.
            // If both root computations will yield divide by zero ... fahget about it! 
            if (a == 0 && d == 0) {
                return;
            }
            
            // compute first root if denominator a is not zero 
            if (a != 0) {
                x1 = d / a;
            }

            // compute second root if denominator d is not zero 
            if (d != 0) {
                x2 = c / d;
            }

        }

        static void ComputeQuadraticCoefficients(
            PointF[] inputPoints, float[] secondDerivs, int i,
            out float a, out float b, out float c) {

            a = 3 * (secondDerivs[i+1] - secondDerivs[i]);
            b = 6 * (inputPoints[i+1].X * secondDerivs[i] - inputPoints[i].X * secondDerivs[i+1]);
            c = 6 * (inputPoints[i+1].Y - inputPoints[i].Y) - 
                (2 * inputPoints[i+1].X * inputPoints[i+1].X - inputPoints[i].X * 
                inputPoints[i].X + 2 * inputPoints[i].X * inputPoints[i+1].X) * secondDerivs[i];
            c -= (inputPoints[i+1].X * inputPoints[i+1].X - 2 * inputPoints[i].X * 
                inputPoints[i+1].X - 2 * inputPoints[i].X * inputPoints[i].X) * secondDerivs[i+1];

        }

        // Perform bounds checking to ensure that the root is within the spline interval
        static bool isRootValid(ref float ? x, int i, PointF[] inputPoints) {
            
            bool isRootValid = (x!=null && (x > inputPoints[i].X) && (x < inputPoints[i+1].X));
            if (!isRootValid) {
                x = null;
                return false;
            }
            return true;
        }

        /*  
        Primary routine that implements the cubic spline extrema algorithm. Calls
        ComputeSecDerivs() to compute the second derivatives, computes quadratic
        coefficients, calls FindQuadRoots() to solve quadratic roots, determines if
        roots are valid abscissa, and calls ComputeY() to compute ordinates.
        */
        
        void ComputeExtrema(PointF[] inputPoints, out List<PointF> extremaPoints)
        {
            float a, b, c;  // coefficients of quadratic equation
            float ? x1 = 0.0f, x2 = 0.0f;   // roots of quadratic equation to be computed 
            extremaPoints = new List<PointF>(); // the extrema that we are computing

            TextWriter errorWriter = Console.Error;
            int numPoints = inputPoints.Length;

            // Make sure we have at least 3 points !!!
            if (numPoints < 3) {
                errorWriter.WriteLine("Less than 3 input points were read, which cannot contain an extremum !");
                System.Environment.Exit(1);
            }
            // Ensure that the X values we were fed are increasing
            for (int i = 0; i < numPoints - 1; i++) {
                if (inputPoints[i].X >= inputPoints[i+1].X) {
                    errorWriter.WriteLine("Input data must have values that are increasing in the X direction !");
                    System.Environment.Exit(1);
                }
            }

            // Compute the second derivatives 
            float[] secondDerivs;
            ComputeSecondDerivatives(inputPoints, out secondDerivs);
            
            // Loop through all the input points and find the extrema
            for (int i = 0; i < numPoints - 1; i++) {

                ComputeQuadraticCoefficients(inputPoints, secondDerivs, i, out a, out b, out c);
                FindQuadraticRoots(a, b, c, out x1, out x2);

                float y1=0f, y2=0f;

                if (isRootValid(ref x1, i, inputPoints)) {
                    // compute the corresponding value of y1 at the extreme x1 value
                    ComputeY(i, (float)x1, inputPoints, secondDerivs, out y1);
                }

                if (isRootValid(ref x2, i, inputPoints)) {
                    // compute the corresponding value of y2 at the extreme x2 value 
                    ComputeY(i, (float)x2, inputPoints, secondDerivs, out y2);
                }

                // Add them to the list in ascending order of X. 
                // This is mostly to make comparisons to expected values easier.
                if (x1==null && x2==null){
                    continue;
                } else if (x1!=null && x2==null) {
                    extremaPoints.Add(new PointF { X = (float)x1, Y = y1});
                } else if (x1==null && x2!=null) {
                    extremaPoints.Add(new PointF { X = (float)x2, Y = y2});
                } else {
                    if (x1 < x2) {
                        extremaPoints.Add(new PointF { X = (float)x1, Y = y1});
                        extremaPoints.Add(new PointF { X = (float)x2, Y = y2});
                    } else {
                        extremaPoints.Add(new PointF { X = (float)x2, Y = y2});
                        extremaPoints.Add(new PointF { X = (float)x1, Y = y1});
                    }
                }
            }
        }
    }
}