---
title: Data Frames with R.NET
permalink: /tut_data_frame/
tags: tutorial
audience: user
keywords: overview
last_updated: 
summary: Manipulating R Data Frames from R.NET
---

{% include linkrefs.html %} 

## Simple data frame manipulations

The following code illustrate that while R.NET tries to mimic the behavior of R with respect to data frames. Data frames are a central part of R data structures, so it is worth expanding with a few examples 

```C#
engine.Evaluate("cases <- expand.grid(x=c('a','b','c'), y=1:3)")
// As of R.NET 1.5.10, factor to character expressions work consistently with R
var letterCases = engine.Evaluate("cases[,'y']").AsCharacter().ToArray();
// "a","a","a","b","b","b", etc. Same as as.character(cases[,'y']) in R
// Note that this used to return  "1", "1", "1", "2", "2", etc. with R.NET 1.5.5
```

There are other ways to extract columns from the data frame, without passing strings of R expressions:

```C#
// Equivalent:
letterCases = df[1].AsCharacter().ToArray();
letterCases = df["y"].AsCharacter().ToArray();
```

The behavior for what is returned by 2-dimensional indexing usually mirrors what is observed directly in R. One exception is when row names are missing; the R behavior is debatable, so R.NET prefers to be strict.

```C#
// Accessing items by two dimensional indexing
string s = (string)df[1, 1]; // "a"
s = (string)df[3, 1]; // "a"
s = (string)df[3, "y"]; // "b"
// s = (string)df["4", "y"]; // fails because there are no row names
df[3, "y"] = "a";
s = (string)df[3, "y"]; // "a"
df[3, "y"] = "d";
s = (string)df[3, "y"]; // null, because we have an <NA> string in R
```

## Advanced data frame manipulations

A section where we show how to create data frames passing complicated stuff from .NET
