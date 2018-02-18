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

I still have no idea why the VS UI stuck to the released package. 

