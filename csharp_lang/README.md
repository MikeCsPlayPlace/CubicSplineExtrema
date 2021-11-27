# C# language version of CubicSplineExtrema

This is the port and redesign of the original Cubic Spline Extrema algorithm implementation from the C language into C#.

If running VSCode on a mac, you may first have to install 'scriptcs' via:

`brew install scriptcs`

Also make sure that you have the 'C# for Visual Studio Code (powered by OmniSharp)' extension installed. That will also install the dotnet application.
Then you can create a C# solution inside the existing csharp_lang directory:

`dotnet new sln`

Then create a C# project (i.e. csharp_lang.csproj) inside the existing csharp_lang directory:

`dotnet new console -o .`

Then add the project to the solution:

`dotnet sln add ./csharp_lang.csproj`

And verify that it was added:

`dotnet sln list`

These actions will also have created a Program.cs file, which we don't care about since we are not starting from scratch here ... so you can delete it.
If compiling from the command line, then use this to build the main program while pulling in the utility classes as well:

`csc CubicSplineExtrema.cs ErrorAnalysisUtils.cs MathUtils.cs`

You should now see a new file called CubicSplineExtrema.exe
On Windows you can invoke this directly via command line.

`$ CubicSplineExtrema.exe`

But on a Mac you will have to use Mono, which is the cross-platform .NET framework.

`$ mono CubicSplineExtrema.exe`

and you should see a prompt to enter the test data input file:

`Enter name of the input file:`

You can enter in the absolute path to the 'test_data' directory file, or if you are still in the csharp_lang directory, the following relative path example will do the trick:

`../test_data/4_point_symmetric_simple_maxima_data.csv`
