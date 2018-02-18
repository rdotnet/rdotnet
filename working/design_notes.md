R.NET design notes
===================

Starting a document to capture questions, mullings and decisions about R.NET after 2018-01. Four years after doing less with .NET, the landscape has changed.

# Topics

* .NET Core and .NET Standard
    * Got over most logistical things to have new proj file format
        * Disappointed by the lack of migration tools to help move to the new format. MS could easily have done better, at least leaving aside edge/complicated cases.
    * While I think I get the idea of .net standards, still unclear in practice what will happen when people use R.NET from their projects.
* Should I keep using paket for dependencies management?
    * I got put off by the sometimes massive list of things that were put in project files, however this seems alleviated if moving to .NET standard and .net core proj file format.
    * paket.lock: bit big for my liking but may be OK since it is generated and if it's not changing too often.
    * The lack of integration with VS SFAIK (warning icons in the solution explorer, no possibility to use the nuget dep facilities) is an annoyance.
* Should I keep using FAKE the build process?
    * Fake v5 is a significant change from fake v4. Had a look at FAKE v5 Documentation - did not clarify much. I'd be like to contribute but keeping it feels "in the way" rather than helpful now to reach r.net for .NETstandard2


* Use alpha or beta versioning to allow for tests of netstandard2.0
* porting to xunit:
    * Clearly laid out: https://xunit.github.io/docs/getting-started-dotnet-core
    * UTs seemed to be run in parrallel; https://xunit.github.io/docs/configuring-with-json  


I cannot seem to get the right nuget repo to be tapped into (local). VS config has it as a repo, sees the local repo, reports a newer package, but still VS sticks to the version on nuget.org.  

using dotnet CLI fails to locate the dynamicinterop alpha package as well

```
cd path\to\rdotnet.nativelibrary
dotnet remove package DynamicInterop
dotnet add package -s "C:\local\nuget" -f netstandard2.0 DynamicInterop
```

It appeared I needed to do:
```
nuget install -Prerelease DynamicInterop
```
Note that I did not locate any CLI options for `dotnet add package` that would let me add the alpha package. [Has been reported by another](https://github.com/dotnet/cli/issues/8485)

I still have no idea why the VS UI stuck to the released package. Foudn out dotnet CLI cannot handle prerelease pkgs... 


Moving on to Linux after managing to get things about OK on Windows:

```
/usr/share/dotnet/sdk/2.1.3/Sdks/Microsoft.NET.Sdk/build/Microsoft.NET.Sdk.targets(114,5): error : Cannot find project info for '/home/per202/src/github_jm/rdotnet/RDotNet.NativeLibrary/RDotNet.NativeLibrary.csproj'. This can indicate a missing project reference. [/home/per202/src/github_jm/rdotnet/R.NET/RDotNet.csproj]
```
I nuked RDotNet.NativeLibrary.csproj; why is it still there.

```
dotnet clean RDotNet.Tests.sln
Microsoft (R) Build Engine version 15.5.179.9764 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

/home/per202/src/github_jm/rdotnet/RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj.metaproj : error MSB4025: The project file could not beloaded. Could not find file '/home/per202/src/github_jm/rdotnet/RDotNet.FSharp.Tests/RDotNet.FSharp.Tests.fsproj.metaproj'.
/home/per202/src/github_jm/rdotnet/RDotNet.FSharp/RDotNet.FSharp.fsproj.metaproj : error MSB4025: The project file could not be loaded. Could not find file '/home/per202/src/github_jm/rdotnet/RDotNet.FSharp/RDotNet.FSharp.fsproj.metaproj'.
```
