---
title: ASP.NET with R.NET
permalink: /tut_asp_dot_net/
tags: getting-started, tutorial
audience: user
keywords: overview
last_updated: 
summary: ASP.NET with R.NET
---

{% include linkrefs.html %} 

## Background

There are quite often questions arising on discussions or issues relating to using R.NET from ASP.NET. 

* R.NET needs to change (if necessary) the PATH environment variable on Windows, to load the native library `R.dll`. It does this at process startup.
* Internet Information Server (IIS) resets the `PATH` environment variable to (presumably) the default. Possibly other environment variables are reset too.
* Consequently, loading some R packages fails if attempted. 

## Getting started

* By default do *not* set a R_HOME environment variable in the machine environment variable. R.NET does this with a specific format to make sure R works fine. You should really avoid interfering unless you are really stuck.
* However, given IIS behavior you need to add the location of the `R.dll` native library to the `PATH` environment variable. Note that if developping from Visual Studio, you may need to add the path to the 32 bits version of R, as this seems to be the 32 bits IIS engine that is launched, even if your project is compiled AnyCPU on a 64 bits Windows (the norm I presume). So add `c:\Program Files\r\R-3.2.0\bin\i386` to the Machine or User's PATH environment variable. See [this support page](https://support.microsoft.com/en-us/kb/310519) for detailed instructions; they are for Windows XP but really seem to be adequate for any Windows version since.
* Note that even if IIS is a separate process from Visual Studio, you need to restart Visual Studio for these new PATH settings to be picked up.

## Sample code

You can find ASP.NET sample code in the [R.NET onboarding samples](https://github.com/jmp75/rdotnet-onboarding), You'll find a visual studio solution file under /rdotnet-onboarding/solutions/WebApp. Credits for the bulk of this sample go to [Richie Melton](https://github.com/skyguy94).

## References

Below are a few links to prior material, for reference. Please follow the instructions in the present page first, as the links below may contain partial or deprecated instructions. 

* [R.NET failing on library(RODBC) call (and other external libraries) ](https://rdotnet.codeplex.com/discussions/572547)
* [Error in deployed asp.net application](https://rdotnet.codeplex.com/discussions/462947)

See also {{ts_asp_dot_net}}