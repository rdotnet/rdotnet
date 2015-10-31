---
title: .NET function callbacks from R
permalink: /tut_R_function_callbacks_from_R/
tags: tutorial
audience: user
keywords: overview
last_updated: 
summary: .NET function callbacks from R
---

{% include linkrefs.html %} 

## Use case

This tutorial was written following [this question at the R.NET site on codeplex](http://rdotnet.codeplex.com/discussions/646729). 

* a .NET program is loading R.NET and calling R
* executes a long-lived R calculation in an R function or R script,
* at regular intervals, the R function calls notification functions such as a percentage completion.
* somehow, this R function calls makes its way back into .NET so that the percentage can be used to update a progress bar

## Approach

### Sample code

You'll find a working sample code addressing this use case in [rdotnet-onboarding](https://github.com/jmp75/rdotnet-onboarding). This consists of two parts:
* a C# program CallbackFunctions, available in visual studio by opening the solution under solutions/Onboard
* a folder rcpp containing the source code for an R package `rdotnetsamples`

### Technical details

Let's analyse this with the end in mind. In R we want something like:

```R
my_r_calculation <- function()
{
  for (i in seq(0, 100, by=20)) {
	long_lived_calculation()
    rdotnetsamples::broadcast_progress_update(paste0('Some Update Message for ', i), i)
  }
}
```

where the function `broadcast_progress_update` triggers some useful notification, in the end via .NET code for our use case.

```R
broadcast_progress_update <- function(message, percentage) {
  # something happens here
}
```

So, how so we set things up such that .NET code is triggered in the notification? There are several possible technical approaches (I think). Let's look at one that uses the package `Rcpp`. We create an R package `rdotnetsamples` that includes R and C++ code, and depends on `Rcpp`. Using `Rcpp`, a C++ function in this new package for `broadcast_progress_update` has the following signature.

```C++
void broadcast_progress_update(CharacterVector message, NumericVector percentage);
```

We want the C++ broadcast_progress_update to trigger a customizable function call.

```C++
#define RDN_SAMPLE_EXPORT extern "C" __declspec(dllexport) 
typedef void(*progress_callback)(const char * message, double percentage);
progress_callback progress_handler;
RDN_SAMPLE_EXPORT void register_progress_handler(void* handler);
```

So, now that we have the C++ layer offering an entry point for registering a custom progress_handler, how to we somehow pass a C# function? Say we want to register the following C# `ProcessProgress`:

```C#
class CallBackHandlers
{
	public void ProcessProgress(string buffer, double percentage) {/*stuff happens*/}
}
```

Using .NET P/Invoke, we can define a delegate (roughly, a function pointer signature):

```C#
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void ProgressNotificationHandler([In] [MarshalAs(UnmanagedType.LPStr)] string buffer, double percentage);
```

and use it for instance on a class offering function pointers for callback handling.

```C#
[StructLayout(LayoutKind.Sequential)]
class TestCallback
{
	[MarshalAs(UnmanagedType.FunctionPtr)]
	public ProgressNotificationHandler MyHandler;
}
```

We have all the elements in place, now we need to call the exported C function `register_progress_handler` from C#. R.NET already uses the library `DynamicInterop` to manage calls to the native `R.dll`, so we may as well use it also to call `register_progress_handler` in the native rdotnetsamples.dll

```C#
REngine engine;
engine.Evaluate("library(rdotnetsamples)");
var unixDllPath = engine.Evaluate("getLoadedDLLs()$rdotnetsamples[['path']]").AsCharacter()[0];
//  unixDllPath would be e.g. "c:/RLibDev/rdotnetsamples/libs/i386/rdotnetsamples.dll"
var dllPath = unixDllPath.Replace("/", "\\"); 
var dll = new DynamicInterop.UnmanagedDll(dllPath);
TestCallback cback = new TestCallback();
CallBackHandlers cbh = new CallBackHandlers();
cback.MyHandler = cbh.ProcessProgress;

string cFunctionRegisterCallback = "register_progress_handler";
register_default_progress_handler registerHandlerFun = dll.GetFunction<register_default_progress_handler>(cFunctionRegisterCallback);
registerHandlerFun(cback.MyHandler);
```

In the end we check that the callback works:

```
> CallbackFunctions.exe
0 : Default R progress msg: Some Update Message for 0
20 : Default R progress msg: Some Update Message for 20
40 : Default R progress msg: Some Update Message for 40
60 : Default R progress msg: Some Update Message for 60
80 : Default R progress msg: Some Update Message for 80
100 : Default R progress msg: Some Update Message for 100
[1] "c:/RLibDev/rdotnetsamples/libs/i386/rdotnetsamples.dll"

After registering the callback with a function pointer to a C# function:

C# progress handler: at 0% - Some Update Message for 0
C# progress handler: at 20% - Some Update Message for 20
C# progress handler: at 40% - Some Update Message for 40
C# progress handler: at 60% - Some Update Message for 60
C# progress handler: at 80% - Some Update Message for 80
C# progress handler: at 100% - Some Update Message for 100
```

with the main program being:

```C#
	static void rdotnet_discussions_646729(REngine engine)
	{
		var setup = @"library(rdotnetsamples) ; rdotnetsamples::register_default_progress_handler()";
		engine.Evaluate(setup);
		var myRFunction = @"
my_r_calculation <- function() {
  for (i in seq(0, 100, by=20)) {
    rdotnetsamples::broadcast_progress_update(paste0('Some Update Message for ', i), i);
  }
}
";
		engine.Evaluate(myRFunction);
		engine.Evaluate("my_r_calculation()");

		var unixDllPath = engine.Evaluate("getLoadedDLLs()$rdotnetsamples[['path']]").AsCharacter()[0];
		var dllPath = unixDllPath.Replace("/", "\\");
		var dll = new DynamicInterop.UnmanagedDll(dllPath);
		TestCallback cback = new TestCallback();
		CallBackHandlers cbh = new CallBackHandlers();
		cback.MyHandler = cbh.ProcessProgress;

		string cFunctionRegisterCallback = "register_progress_handler";
		register_default_progress_handler registerHandlerFun = dll.GetFunction<register_default_progress_handler>(cFunctionRegisterCallback);
		registerHandlerFun(cback.MyHandler);

		Console.WriteLine();
		Console.WriteLine("After registering the callback with a function pointer to a C# function:");
		Console.WriteLine();
		engine.Evaluate("my_r_calculation()");

	}
```