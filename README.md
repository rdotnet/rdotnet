# R.NET

# DISCLAIMER
This is a fork of the original repo. I am using R.NET for different calculations of psychometric data. The production website is at https://app.getteaminsight.com. The personal report is free - everyone can fill the survey. We measure agile behavior for teams and individuals.

The webapp runs on .NETCore, so I was using the .NETStandard fork of R.NET. However, after R version 3.5.0 some of the basic structures has changed in R and the available NuGet packages were not up to date. For different technical reasons we could not manage to incorporate the older R version in our CI/CD pipeline - and we did not have the time to investigate deeper. The only solution was to port the existing R.NET fork that has already implemented the changes for the newer R versions.

So this repo is a port of StatTag/altrep branch to .NETCore and I also added a few changes to automatically support CentOS that I am using.

R.NET
=======
windows master: [![Build status master](https://ci.appveyor.com/api/projects/status/bok963px1o9k7jay/branch/master?svg=true)](https://ci.appveyor.com/project/jmp75/rdotnet/branch/master) windows testing: [![Build status testing](https://ci.appveyor.com/api/projects/status/bok963px1o9k7jay/branch/testing?svg=true)](https://ci.appveyor.com/project/jmp75/rdotnet/branch/testing) Linux master: [![Linux master Build Status](https://travis-ci.org/jmp75/rdotnet.svg?branch=master "Linux master Build Status")](https://travis-ci.org/jmp75/rdotnet/builds) [![Nuget Version](https://buildstats.info/nuget/R.NET)](https://www.nuget.org/packages/R.NET/)

R.NET is an in-process bridge for the .NET Framework to access the R statistical language. R.NET works on Windows, Linux and MacOS.

## License

[MIT](./License.txt)

## Software requirements

On Windows, R.NET requires .NET Framework > 4.6.1 or .NET Core 2.0, and an access to the native R libraries installed with the R environment. R needs not necessarily be installed as a software on the executing machine, so long as DLL files are accessible (you may need to tweak environment variables for the latter to work, though)
On Linux and MacOS, Mono is required, as well as an access to the native R libraries.

## Getting started

As of 2017-08

* If you want the latest binary distribution of R.NET and you are already familiar with managing dependencies with NuGet, head to [R.NET on NuGet](https://www.nuget.org/packages?q=R.NET)
* If you need a bit more documentation to get started, the prefered entry point is at [http://rdotnet.github.io/rdotnet](http://rdotnet.github.io/rdotnet)

## Building from source

### Compiler toolchain foreword

Due to the move to targetting `netstandard2.0`, you might encounter compiling issue if using an older toolchain. This is machine dependent (mostly, which visual studio versions and .NET targetting packs you have). You may want to [adapt the instructions from the rClr packge](https://github.com/rdotnet/rClr/blob/master/README.md#windows) to avoid some pitfalls.

As an example:

* `where msbuild` returns `C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe` should be the first line. `C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe` is probably not a good option.
* `msbuild -version` returns `16.0.461.62831`
* `where dotnet`: `C:\Program Files\dotnet\dotnet.exe`
* `dotnet --version`:  `2.1.602`. Note that this version of the .net core SDK would prevent compilation from VS2017: VS2019 required.
* `nuget help`: `4.9.4.5839`

### Compiling

If using dotnet version prior to 2.1.3, [installing/restoring prerelease dependency packages is problematic](https://github.com/dotnet/cli/issues/8485). You may need to use

```sh
nuget install -Prerelease DynamicInterop -OutputDirectory packages
```

otherwise:

```sh
nuget restore RDotNet.Tests.sln
dotnet build --configuration Debug --no-restore RDotNet.Tests.sln
# or if any issue possibly try:
# msbuild RDotNet.Tests.sln /p:Platform="Any CPU" /p:Configuration=Debug /consoleloggerparameters:ErrorsOnly
```

#### Unit tests

Unit tests can be run using:

```sh
dotnet test RDotNet.Tests/RDotNet.Tests.csproj
```

Normally you should get something like:

```text
Total tests: 92. Passed: 84. Failed: 0. Skipped: 8.
Test Run Successful.
Test execution time: 5.2537 Seconds
```

However note that from time to time (or at the first `dotnet test` execution) tests may fail to start, for reasons as yet unknown:

```text
Starting test execution, please wait...
The active test run was aborted. Reason:
Test Run Aborted.
```

It may be that all subsequent calls then work as expected.

```sh
dotnet test RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj
```

### Building the nuget package

*This section is primarily a reminder to the package author.*

```bat
set B_CONFIG=Release
:: Or for initial testing/debugging substitute Debug for Release
:: set B_CONFIG=Debug

dotnet build --configuration %B_CONFIG% --no-restore RDotNet.Tests.sln
dotnet pack R.NET/RDotNet.csproj --configuration %B_CONFIG% --no-build --no-restore --output nupkgs
dotnet pack RDotNet.FSharp/RDotNet.FSharp.fsproj --configuration %B_CONFIG% --no-build --no-restore --output nupkgs
```
