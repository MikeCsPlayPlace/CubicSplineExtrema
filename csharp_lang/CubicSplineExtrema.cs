using System;
using System.IO; // TextWriter
using System.Drawing; // PointF
using System.Collections.Generic; // List

namespace CubicSplineExtrema {
    class CubicSplineExtrema {
        static void Main(string[] args) {


            string dataFile;
            TextWriter errorWriter = Console.Error;
            
            CubicSplineExtrema extrema = new CubicSplineExtrema();
            try {
                Console.Write("Enter name of the input file: ");
                dataFile = Console.ReadLine();
                List<PointF> inputPoints = extrema.CsvFileReader(dataFile);
                List<PointF> extremaPoints;

                Console.WriteLine("\nInput points: " + inputPoints.Count);
                //using foreach loop
                foreach(var point in inputPoints) {
                    Console.WriteLine(point);
                }

                // Call the method that does the heavy lifting
                extrema.ComputeExtrema(inputPoints.ToArray(), out extremaPoints);

                // print the resulting list of extrema 
                Console.WriteLine("\nExtrema computed: " + extremaPoints.Count);
                foreach(var point in extremaPoints) {
                    Console.WriteLine(point);
                }

                String expectedResultsFile = dataFile.Replace("csv", "expected_output.csv");
                List<PointF> expectedPoints = extrema.CsvFileReader(expectedResultsFile);

                bool doesExtremaCountMatch = true;
                if (expectedPoints.Count!=extremaPoints.Count) {
                    doesExtremaCountMatch =false;
                    Console.WriteLine("The number of extrema computed does not match expected.");
                    Console.WriteLine("Therefore unable to compute percent errors for each pair.");
                }
                Console.WriteLine("\nExtrema expected: " + expectedPoints.Count);

                int i = 0;
                float avgCompositeError = 0.0f;
                foreach(var point in expectedPoints) {
                    Console.Write(point);
                    // Make sure we have a 1-to-1 set of xy pairs
                    if (doesExtremaCountMatch) {
                        PointF error = ErrorAnalysisUtils.ComputeAbsRelativePercentDiff(extremaPoints[i], point);
                        Console.Write($"   % aPRD = {error.X}, {error.Y} ");
                        float compositeError = ErrorAnalysisUtils.ComputeCompositeAbsRelativePercentDiff(error);
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
            } catch (Exception e) {
                errorWriter.WriteLine(e.Message);
                System.Environment.Exit(1);
            }
        }

        private List<PointF> CsvFileReader(String dataFile) {

            // Note: using List because it preserves order
            List<PointF> points = new List<PointF>();

            string[] lines = null;
            try {
                lines = System.IO.File.ReadAllLines(@dataFile);
            } catch (Exception e) {
                throw e;
            }

            if (lines.Length==0) {
                throw new Exception($"No lines were read from the input file {dataFile}. Terminating.");
            }

            foreach (string line in lines) {

                string[] xyPoint = line.Split(',');
                if (xyPoint.Length!=2) {
                    throw new Exception($"Invalid format in {dataFile} at line={line}");
                }
                
                try {
                    float x = float.Parse(xyPoint[0]);
                    float y = float.Parse(xyPoint[1]);
                    points.Add(new PointF {X=x, Y=y});
                }  catch (FormatException fe) {
                    throw new Exception($"Error {fe} {dataFile} parsing point {xyPoint}.");
                }
            }
            return points;
        }

        // Given abscissa (x) location, compute corresponding cubic spline ordinate (y) value.
        private static void ComputeY (
            int i, // input - array index
            float ? xValue, // input - x value at which to solve for y
            PointF[] inputPoints, // input - array of y values
            double[] secondDerivs, // input - array of second derivatives of each data interval
            out float yValue // output - address of y extreme value at x
        ) {
            double A, B, C, D; // cubic spline coefficients

            if (xValue == null) {
                throw new Exception("ComputeY() xValue cannot be null");
            }
            // We are supposed to have input X values that are are adjacient and non-duplicated,
            // but check for an issue anyway.
            if (inputPoints[i + 1].X == inputPoints[i].X) {
                throw new Exception("ComputeY() input X axis data values must be unique");
            }

            // Compute the standard cubic spline coefficients
            A = (double)(inputPoints[i + 1].X - xValue) / (inputPoints[i + 1].X - inputPoints[i].X);
            B = 1 - A;
            double temp = Math.Pow(inputPoints[i + 1].X - inputPoints[i].X, 2.0) / 6.0;
            C = (A * A * A - A) * temp;
            D = (B * B * B - B) * temp;

            // compute the ordinate value at the abscissa location
            yValue = (float)(A * inputPoints[i].Y + B * inputPoints[i + 1].Y + C * secondDerivs[i] + D * secondDerivs[i + 1]);
        }

        // Perform bounds checking to ensure that the root is within the spline interval
        static bool DetermineIfValidRoot(ref float ? x, int i, PointF[] inputPoints) {
            
            bool IsRootValid = (x!=null && (x > inputPoints[i].X) && (x < inputPoints[i+1].X));
            if (!IsRootValid) {
                x = null;
                return false;
            }
            return true;
        }

        /*  
        Primary routine that implements the cubic spline extrema algorithm. 
        Computes the second derivatives, computes quadratic
        coefficients, solves for the quadratic roots, determines if
        roots are valid abscissa (X's), and computes the corresponding ordinates (Y's).
        */
        
        public void ComputeExtrema(PointF[] inputPoints, out List<PointF> extremaPoints)
        {
            double a, b, c;  // coefficients of quadratic equation
            float ? x1 = 0.0f, x2 = 0.0f;   // roots of quadratic equation to be computed 
            extremaPoints = new List<PointF>(); // the extrema that we are computing

            int numPoints = inputPoints.Length;

            // Make sure we have at least 3 points !!!
            if (numPoints < 3) {
                throw new Exception("Less than 3 input points were read, which cannot contain an extremum !");
            }
            // Ensure that the X values we were fed are increasing
            for (int i = 0; i < numPoints - 1; i++) {
                if (inputPoints[i].X >= inputPoints[i+1].X) {
                    throw new Exception("Input data must have values that are increasing in the X direction !");
                }
            }

            // Compute the second derivatives 
            double[] secondDerivs;
            MathUtils.ComputeSecondDerivatives(inputPoints, out secondDerivs);
            
            // Loop through all the input points and find the extrema
            for (int i = 0; i < numPoints - 1; i++) {

                MathUtils.ComputeQuadraticCoefficients(inputPoints, secondDerivs, i, out a, out b, out c);
                MathUtils.ComputeQuadraticRoots(a, b, c, out x1, out x2);

                float y1=0.0f, y2=0.0f;

                if (DetermineIfValidRoot(ref x1, i, inputPoints)) {
                    // compute the corresponding value of y1 at the extreme x1 value
                    ComputeY(i, x1, inputPoints, secondDerivs, out y1);
                }

                if (DetermineIfValidRoot(ref x2, i, inputPoints)) {
                    // compute the corresponding value of y2 at the extreme x2 value 
                    ComputeY(i, x2, inputPoints, secondDerivs, out y2);
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