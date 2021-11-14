# CubicSplineExtrema

This is the directory that contains the original software that was published in Dr. Dobb's Journal issue #246 way back in April 1996. It was written in the C language and that code is presented here as-is.

The easiest way to compile this is probably via command line via gcc. Then you can just run the following in this directory once you have installed gcc:
 `gcc cubic_extrema.c main.c -o main`

That will output a main executable that you can run via:
 `./main <absolute or relative path to a test_data file>`

The `main` executable that I created on my Intel-based Mac is also committed to github and it _may_ work for you without having to compile anything.

The test files provided are in the `test_data` directory, so `../test_data` from here.
You can then view the expected results for each of the files in an IDE or command line (e.g. `cat <filename>`) to visually compare the exactly calculated extrema vs what the cubic extrema algorithm produced. Those expected files are named the same as the input files, but with `.expected_output.csv` as the extension.

Each of the test data sets also an explanation for how the data was produced, and how the expected output values were determined for that input data. See the `test_data/README.md` for those details.
