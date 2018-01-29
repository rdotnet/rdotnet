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
