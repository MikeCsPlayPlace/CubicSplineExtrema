# CubicSplineExtrema

This is the directory that contains the original software that was published in Dr. Dobb's Journal issue #246 way back in April 1996. It was written in the C language and that code is presented here as-is.

The easiest way to compile this is probably via command line via gcc. Then you can just run the following in this directory once you have installed gcc:
 gcc cubic_extrema.c main.c -o main

That will output a main executable that you can run via:
 ./main <absolute path to a test_data file>
