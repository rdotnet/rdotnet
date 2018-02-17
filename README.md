R.NET
=======

R.NET is an in-process bridge for the .NET Framework to access the R statistical language. R.NET works on Windows, Linux and MacOS. 

# Software requirements

On Windows, R.NET requires .NET Framework > 4.6.1 or .NET Core 2.0, and an access to the native R libraries installed with the R environment. R needs not necessarily be installed as a software on the executing machine, so long as DLL files are accessible (you may need to tweak environment variables for the latter to work, though)
On Linux and MacOS, Mono is required, as well as an access to the native R libraries.

# Getting started

As of 2017-08

* If you want the latest binary distribution of R.NET and you are already familiar with managing dependencies with NuGet, head to [R.NET on NuGet](https://www.nuget.org/packages?q=R.NET)
* If you need a bit more documentation to get started, the prefered entry point is at [http://jmp75.github.io/rdotnet](http://jmp75.github.io/rdotnet)

# Building from source

```sh
dotnet restore RDotNet.Tests.sln
dotnet build --configuration Debug --no-restore  RDotNet.Tests.sln
```

```sh
dotnet test RDotNet.Tests/RDotNet.Tests.csproj
```

# DEPRECATED Building from source

R.NET uses [Paket](https://fsprojects.github.io/Paket/) for dependency management and build, and [FAKE v4](https://fake.build/legacy-gettingstarted.html) (_Note to self: investigate FAKE v5_)

## Windows

To query NuGet and get the latest versions of packages used by R.NET:

```bat
.paket\paket.exe update
.paket\paket.exe restore
```

Note that you may want to specify which msbuild engine to use (had woes with default detection)

```bat
set VisualStudioVersion=14.0
.\build.cmd NuGet
```

