---
title: Basic types with R.NET
permalink: /tut_basic_types/
tags: getting-started, tutorial
audience: user
keywords: overview
last_updated: 
summary: Basic data types with R.NET
---

{% include linkrefs.html %} 

R.NET 1.5.10 and subsequent versions include significant changes notably to alleviate two stumbling blocks often dealt with by users: paths to the R shared library, and preventing multiple engine initializations.

## Hello world

The following "Hello World" sample illustrates how the new API is simpler in 90% of use cases on Windows (on Linux you may need to set up an environment variable, see thereafter):

```C#
static void Main(string[] args)
{
	REngine.SetEnvironmentVariables(); // <-- May be omitted; the next line would call it.
	REngine engine = REngine.GetInstance();
	// A somewhat contrived but customary Hello World:
	CharacterVector charVec = engine.CreateCharacterVector(new[] { "Hello, R world!, .NET speaking" });
	engine.SetSymbol("greetings", charVec);
	engine.Evaluate("str(greetings)"); // print out in the console
	string[] a = engine.Evaluate("'Hi there .NET, from the R engine'").AsCharacter().ToArray();
	Console.WriteLine("R answered: '{0}'", a[0]);
	Console.WriteLine("Press any key to exit the program");
	Console.ReadKey();
	engine.Dispose();
}
``` 
You retrieve a single REngine object instance, after setting the necessary environmental variables. Even the call to `REngine.SetEnvironmentVariables()` can be omitted, though we'd advise you keep it explicit. `SetEnvironmentVariables`, on Windows, looks at the Registry settings set up by the R installer. 

If R.NET complains about not being able to find one of the paths to R, see {{ts_paths_on_windows}}. This can happen due to the many variations of states of Windows Registries and environment variables. If need be, you can override the behaviours setting the environment variables and engine initialization with your own steps.

On Linux/MacOS, the path to libR.so (for Linux) *must* be in the environment variable {"LD_LIBRARY_PATH"} *before* the process start, otherwise the R.NET engine will not properly initialize. If this is not set up, R.NET will throw an exception with a detailed message giving users hints as to what to do. Read the Appendix at the end of this page if R.NET complains about your {"LD_LIBRARY_PATH"}.

## Numeric vectors

You usually interact with the REngine object with the methods `Evaluate`, `GetSymbol`, and `SetSymbol`. To create R vector and matrices, the REngine object has extension methods such as `CreateNumericVector`, `CreateCharacterMatrix`, etc. Finally, you can invoke R functions in a variety of ways, using the method `Evaluate` of the REngine object, and also more directly. 

The following sample code illustrate the most used capabilities. It is extracted from the sample code 2 at <a href="https://github.com/jmp75/rdotnet-onboarding"></a>, as of 2014-04.

This illustrate basic operations with numeric vectors

```C#
var e = engine.Evaluate("x <- 3");
// You can now access x defined in the R environment
NumericVector x = engine.GetSymbol("x").AsNumeric();
engine.Evaluate("y <- 1:10");
NumericVector y = engine.GetSymbol("y").AsNumeric();
```

### Basic numeric example with t-test statistic

It is available from the sample code 1 at <a href="https://github.com/jmp75/rdotnet-onboarding">rdotnet-onboarding</a>, as of 2014-04.

```C#
static void Main(string[] args)
{
	 REngine.SetEnvironmentVariables();
	 REngine engine = REngine.GetInstance();
	 // REngine requires explicit initialization.
	 // You can set some parameters.
	 engine.Initialize();

	 // .NET Framework array to R vector.
	 NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
	 engine.SetSymbol("group1", group1);
	 // Direct parsing from R script.
	 NumericVector group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

	 // Test difference of mean and get the P-value.
	 GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
	 double p = testResult["p.value"].AsNumeric().First();

	 Console.WriteLine("Group1: [{0}]", string.Join(", ", group1));
	 Console.WriteLine("Group2: [{0}]", string.Join(", ", group2));
	 Console.WriteLine("P-value = {0:0.000}", p);

	 // you should always dispose of the REngine properly.
	 // After disposing of the engine, you cannot reinitialize nor reuse it
	 engine.Dispose();
}
```
## Character vectors


### Missing values

Placeholder, showing what happens bidirectionally with NA values for the various vector types. See the Data Types section later in this page.

### Further examples

Looking at the unit tests under the project RDotNet.Tests will provide further information on R.NET uses and programming idioms.
Illustrate the speed of data transfer

### Runtime performance

Placeholder, showing best practices to maximise runtime speed


