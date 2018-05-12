[![Windows netstandard2 Build Status](https://ci.appveyor.com/api/projects/status/6h0aajsja1qhv2so?svg=true "Windows netstandard2 Build Status")](https://ci.appveyor.com/project/jmp75/rdotnet) 

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

## Foreword/rant

As of February 2018 I am underwhelmed by the state of nuget package dependency. The process of migrating R.NET to `netstandard2.0` has been replete with frustration with the lack of clarity, multiple issues, inconsistencies or bugs in the behaviors of `dotnet`, `nuget` and visual studio.

## Building from source

If using dotnet version prior to 2.1.3, [installing/restoring prerelease dependency packages is problematic](https://github.com/dotnet/cli/issues/8485). You may need to use

```sh
nuget install -Prerelease DynamicInterop -OutputDirectory packages
```

```sh
dotnet restore RDotNet.Tests.sln
dotnet build --configuration Debug --no-restore  RDotNet.Tests.sln
```

```sh
dotnet test RDotNet.Tests/RDotNet.Tests.csproj
```

## Building the nuget package

*This section is primarily a reminder to the package author.*

```bash
dotnet build --configuration Release --no-restore  RDotNet.Tests.sln
dotnet pack R.NET/RDotNet.csproj --configuration Release --no-build --no-restore --output nupkgs
# Or for initial testing/debugging
dotnet pack DynamicInterop/DynamicInterop.csproj --configuration Debug --no-build --no-restore --output nupkgs
```
