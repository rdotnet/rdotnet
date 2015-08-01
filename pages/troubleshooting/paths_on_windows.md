---
title: Paths on Windows
permalink: /ts_paths_on_windows/
tags: troubleshooting
audience: user
keywords: troubleshooting
last_updated: 
summary: Troubleshooting issues preventing R.NET from finding R paths on Windows
---

{% include linkrefs.html %} 

## Issue 

```
An unhandled exception of type 'System.NotSupportedException' occurred in RDotNet.NativeLibrary.dll
Additional information: R_HOME was not provided and a suitable path could not be found by R.NET
```

### Description

```C#
```

### Diagnosis

For R.NET 1.6.5 or later, it is possible to get a log of the discovery process of the R paths.

The following program, also present in <a href="https://github.com/jmp75/rdotnet-onboarding/tree/master/ReportInfo">R.NET onboarding - ReportInfo</a>, is useful to diagnose and report issues.

```C#
using System;
using RDotNet.NativeLibrary;

namespace ReportInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            string rHome = null;
            string rPath = null;
            if (args.Length > 0)
                rPath = args[0];
            if (args.Length > 1)
                rHome = args[1];

            var logInfo = NativeUtility.FindRPaths(ref rPath, ref rHome);

            Console.WriteLine("Is this process 64 bits? {0}", System.Environment.Is64BitProcess);
            Console.WriteLine(logInfo);
        }
    }
}
```

### Solution



