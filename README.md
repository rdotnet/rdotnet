[![Build status](https://ci.appveyor.com/api/projects/status/bok963px1o9k7jay?svg=true)](https://ci.appveyor.com/project/jmp75/rdotnet)

# DISCLAIMER
This is a fork of the original repo. I am using R.NET for different calculations of psychometric data. The production website is at https://app.getteaminsight.com. The personal report is free - everyone can fill the survey. We measure agile behavior for teams and individuals.

The webapp runs on .NETCore, so I was using the .NETStandard fork of R.NET. However, after R version 3.5.0 some of the basic structures has changed in R and the available NuGet packages were not up to date. For different technical reasons we could not manage to incorporate the older R version in our CI/CD pipeline - and we did not have the time to investigate deeper. The only solution was to port the existing R.NET fork that has already implemented the changes for the newer R versions.

So this repo is a port of StatTag/altrep branch to .NETCore and I also added a few changes to automatically support CentOS that I am using.

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
