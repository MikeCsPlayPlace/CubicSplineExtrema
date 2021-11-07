# CubicSplineExtrema

This is the main level for access to the software algorithm that I invented/derived back in 1995 and was published in Dr. Dobb's Journal back in April 1996. 

The algorithm essentially computes the minima and maxima *directly* for a set of discrete data points. Typical searches require the use of bracketed iterative searches and are hence much less efficient. My approach instead goes directly for a solution that computes a second derivative spline at the data "knots", and thus provides a fast and  accurate set of results. And handles both "happy path" and "unhappy path" test cases, as exhibited by the test data sets used in the development of the algorithm. Those data sets were presened in the original article, and are provided here for running the included unit tests.  

The algorithm was originally written in the C language, but here I am porting it to other languages as well. Those will be Java, Python, C#, and C++ and hence will be OOO versions of the code.