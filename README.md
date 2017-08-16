R.NET
=======

R.NET is an in-process bridge for the .NET Framework to access the R statistical language. R.NET works on Windows, Linux and MacOS. 

# Software requirements

On Windows, R.NET requires .NET Framework 4.5.2 and an access to the native R libraries installed with the R environment. R needs not necessarily be installed as a software on the executing machine, so long as DLL files are accessible.
On Linux and MacOS, Mono is required, as well as an access to the native R libraries.

# Getting started

As of 2017-08

* If you want the latest binary distribution of R.NET and you are already familiar with managing dependencies with NuGet, head to [R.NET on NuGet](https://www.nuget.org/packages?q=R.NET.Community)
* If you need a bit more documentation to get started, the prefered entry point is at [http://jmp75.github.io/rdotnet](http://jmp75.github.io/rdotnet)

# Building from source

R.NET uses [Paket](https://fsprojects.github.io/Paket/) for dependency management and build, and [FAKE v4](https://fake.build/legacy-gettingstarted.html) (_Note to self: investigate FAKE v5_)

## Windows

To query NuGet and get the latest versions of packages used by R.NET:

```bat
.paket\paket.exe update
.paket\paket.exe restore
```
