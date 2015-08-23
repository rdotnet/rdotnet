---
title: ASP.NET running R.NET
permalink: /ts_asp_dot_net/
tags: troubleshooting
audience: user
keywords: troubleshooting
last_updated: 
summary: Troubleshooting issues preventing R.NET from finding R paths on Windows
---

{% include linkrefs.html %} 

## Issue 

Sample program using R.NET works from command line application, but fails running from ASP.NET (often refered to in online questions as a "running from a Web App")

### Description

```
// snip
REngine r = REngine.GetInstance();
// further code
r.Evaluate("library(RODBC)");
```

```
An unhandled exception of type 'RDotNet.EvaluationException' occurred in RDotNet.dll
Additional information: Error: package or namespace load failed for 'RODBC'
```

### Solution

See instructions and links in the tutorial page {{tut_asp_dot_net}}

### Diagnosis

Below are extracts from [this discussion thread](https://rdotnet.codeplex.com/discussions/572547) with instructions that may help you diagnose issues with running R.NET from ASP.NET.

You can find ASP.NET sample code in the [R.NET onboarding samples](https://github.com/jmp75/rdotnet-onboarding), You'll find a visual studio solution file under /rdotnet-onboarding/solutions/WebApp. Credits for the bulk of this sample go to [Richie Melton](https://github.com/skyguy94).

Environment from which these instructions were written:
```
Win7 64 bits
R 3.2.0
VS2013
IIS Express
```

This section uses an arbitrary package, twitteR, for the sake of example.

You can put  breakpoints into `CodeController.Execute` and its constructor, in the sample web application of the onboarding guide. Then if executing from the Web page:

```
library(twitteR)
```

and using the "immediate" debug window on breakpoints

```
Environment.Is64BitProcess
false
```

So note that even if I am running a 64 bits operating system and the build is AnyCPU, the process is 32 bits, so the 32 bits native `R.dll` should be loaded. If I query for the environment variables using the "Immediate" debug window of Visual Studio.

```
Environment.GetEnvironmentVariable("PATH")
"c:\\Yadi\\Yada\\Lots\\of\paths;F:\\bin\\doxygen\\bin"
Environment.GetEnvironmentVariable("R_HOME")
"C:/PROGRA~1/R/R-32~1.0"
```

So, `R_HOME` is present as set by R.NET, but the path to the native `R.dll` is not present in the `PATH` (should be at the end, as appended by R.NET)

The call to `r.Evaluate('library(twitteR)');` thus fails indeed.

PATH should have included "c:\Program Files\r\R-3.2.0\bin\i386", but it is missing. R.NET would have appended it. Don't know/understand why IIS does that, whatever. R_HOME has been set correctly by R.NET (and the path format for R_HOME is deliberate)

To fix it follow instructions in the tutorial page {{tut_asp_dot_net}}

Restart in debug mode, same breakpoint, immediate window:

```
Environment.GetEnvironmentVariable("PATH")
"c:\\Yadi\\Yada\\Lots\\of\paths;F:\\bin\\doxygen\\bin;c:\\Program Files\\r\\R-3.2.0\\bin\\i386"
Environment.GetEnvironmentVariable("R_HOME")
"C:/PROGRA~1/R/R-32~1.0"
```

Then things work:

```
library(twitteR)
tuser <- getUser('geoffjentry')
```


