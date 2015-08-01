---
title: R Functions with R.NET
permalink: /tut_R_functions/
tags: tutorial
audience: user
keywords: overview
last_updated: 
summary: manipulating R functions with R.NET
---

{% include linkrefs.html %} 

## Basic calls to R functions

R functions and function calls are expressions, and can be evaluated by way of generating strings that are valid R expressions:

```C#
engine.Evaluate("cases <- expand.grid(x=c('a','b','c'), y=1:3)")
var df = engine.Evaluate("expand.grid(x=c('A','B','C'), y=1:3)").AsDataFrame()
```

## More advanced ways to interact with R functions

While you may evaluate function calls by generating a string and call the Evaluate method, this can be unwieldy for cases where you pass large amounts of data. The following demonstrates how you may call a function, a bit like how you would invoke a function reflectively in .NET. 

```C#
// Invoking functions; Previously you may have needed custom function definitions
var myFunc = engine.Evaluate("function(x, y) { expand.grid(x=x, y=y) }").AsFunction();
var v1 = engine.CreateIntegerVector(new[] { 1, 2, 3 });
var v2 = engine.CreateCharacterVector(new[] { "a", "b", "c" });
var df = myFunc.Invoke(new SymbolicExpression[] { v1, v2 }).AsDataFrame();
```

R.NET 1.5.10 includes many improvements to support function calls directly from C#, with less string manipulations and less calls to `REngine.Evaluate`.

```C#
// As of R.NET 1.5.10, more function call syntaxes are supported.
var expandGrid = engine.Evaluate("expand.grid").AsFunction();
var d = new Dictionary<string, SymbolicExpression>();
d["x"] = v1;
d["y"] = v2;
df = expandGrid.Invoke(d).AsDataFrame();
```

