// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------

#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"
open System
open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper

// --------------------------------------------------------------------------------------
// Information about the project to be used at NuGet and in AssemblyInfo files
// --------------------------------------------------------------------------------------

let projectName = "R.NET"
let projectSummary = "Interoperability library to access the R statistical language runtime from .NET"
let projectDescription = """
  A .NET interoperability library to access the R statistical language runtime from .NET languages.
  The library is designed for fast data exchange, in process."""
let authors = ["Kosei Abe"; "Jean-Michel Perraud"]
let companyName = ""
let tags = ".NET R R.NET visualization statistics C# F#"

let gitHome = "https://github.com/jmp75/rdotnet"
let gitName = "rdotnet"

// --------------------------------------------------------------------------------------
// The rest of the code is standard F# build script 
// --------------------------------------------------------------------------------------

// Read release notes & version info from RELEASE_NOTES.md
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let binDir = __SOURCE_DIRECTORY__ @@ "bin"
let release = IO.File.ReadLines "RELEASE_NOTES.md" |> parseReleaseNotes

// Generate assembly info files with the right version & up-to-date information
// 2018-01 moving to netstandard2.0 and VersionInfo may be outdated/superseded
// Target "VersionInfo" (fun _ ->
//   let fileName = "R.NET/Properties/VersionInfo.cs"
//   CreateCSharpAssemblyInfoWithConfig fileName
//       [ Attribute.Version release.AssemblyVersion
//         Attribute.FileVersion release.AssemblyVersion ] 
//       (AssemblyInfoFileConfig(false))
//   CreateFSharpAssemblyInfoWithConfig "RDotNet.FSharp/VersionInfo.fs"
//       [ Attribute.Version release.AssemblyVersion
//         Attribute.FileVersion release.AssemblyVersion ] 
//       (AssemblyInfoFileConfig(false))
// )
Target "VersionInfo" DoNothing


// --------------------------------------------------------------------------------------
// Update the assembly version numbers in the script file.

open System.IO

//Target "UpdateFsxVersions" (fun _ ->
//    let pattern = "packages/RProvider.(.*)/lib"
//    let replacement = sprintf "packages/RProvider.%s/lib" release.NugetVersion
//    let path = "./src/RProvider/RProvider.fsx"
//    let text = File.ReadAllText(path)
//    let text = Text.RegularExpressions.Regex.Replace(text, pattern, replacement)
//    File.WriteAllText(path, text)
//)

// --------------------------------------------------------------------------------------
// Clean build results & restore NuGet packages

//Target "Clean" (fun _ ->
//    CleanDirs ["bin"; "temp" ]
//)

//Target "CleanDocs" (fun _ ->
//    CleanDirs ["docs/output"]
//)

// --------------------------------------------------------------------------------------
// Build library & test project

Target "Build" (fun _ ->
//    !! ("R.NET.sln")
//    |> MSBuildRelease "" "Build"
//    |> Log "AppBuild-Output: "
//)
//
//Target "BuildTests" (fun _ ->
    !! ("RDotNet.Tests.sln")
    |> MSBuildRelease "" "Build"
    |> Log "AppBuild-Output: "
)

Target "Rebuild" (fun _ ->
//    !! ("R.NET.sln")
//    |> MSBuildRelease "" "Build"
//    |> Log "AppBuild-Output: "
//)
//
//Target "BuildTests" (fun _ ->
    !! ("RDotNet.Tests.sln")
    |> MSBuildRelease "" "Rebuild"
    |> Log "AppBuild-Output: "
)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner & kill test runner when complete

Target "RunTests" (fun _ ->
    let nunitConsolePath = "packages/NUnit.Runners/tools"

    ActivateFinalTarget "CloseTestRunner"

    !! "RDotNet.Tests/bin/Release/RDotNet.Tests.dll"
    |> NUnit (fun p ->
            {p with 
                ToolPath = nunitConsolePath
                Domain = NUnitDomainModel.SingleDomainModel
            })
)
 
FinalTarget "CloseTestRunner" (fun _ ->  
    ProcessHelper.killProcess "nunit-console.exe"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

let DoNuGet fileName =
    // Format the description to fit on a single line (remove \r\n and double-spaces)
    let projectDescription = projectDescription.Replace("\r", "").Replace("\n", "").Replace("  ", " ")
    NuGet (fun p -> 
        { p with   
            Authors = authors
            Project = projectName
            Summary = projectSummary
            Description = projectDescription
            Version = release.NugetVersion
            ReleaseNotes = String.concat " " release.Notes
            Tags = tags
            OutputPath = "bin"
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" })
        fileName

Target "NuGetRDotNet" (fun _ ->
    DoNuGet "nuget/RDotNet.nuspec"
)

Target "NuGetRDotNetFs" (fun _ ->
    DoNuGet "nuget/RDotNet.FSharp.nuspec"
)

Target "NuGetBackward" (fun _ ->
    DoNuGet "nuget/RDotNet.Community.nuspec"
    DoNuGet "nuget/RDotNet.Community.FSharp.nuspec"
    DoNuGet "nuget/RDotNet.Previous.FSharp.nuspec"
)

// --------------------------------------------------------------------------------------
// Generate the documentation

//Target "GenerateDocs" (fun _ ->
//    executeFSIWithArgs "docs/tools" "generate.fsx" ["--define:RELEASE"] [] |> ignore
//)

// --------------------------------------------------------------------------------------
// Release Scripts

//Target "ReleaseDocs" (fun _ ->
//    Repository.clone "" (gitHome + "/" + gitName + ".git") "temp/gh-pages"
//    Branches.checkoutBranch "temp/gh-pages" "gh-pages"
//    CopyRecursive "docs/output" "temp/gh-pages" true |> printfn "%A"
//    CommandHelper.runSimpleGitCommand "temp/gh-pages" "add ." |> printfn "%s"
//    let cmd = sprintf """commit -a -m "Update generated documentation for version %s""" release.NugetVersion
//    CommandHelper.runSimpleGitCommand "temp/gh-pages" cmd |> printfn "%s"
//    Branches.push "temp/gh-pages"
//)

//Target "ReleaseBinaries" (fun _ ->
//    Repository.clone "" (gitHome + "/" + gitName + ".git") "temp/release" 
//    Branches.checkoutBranch "temp/release" "release"
//    CopyRecursive "bin" "temp/release" true |> printfn "%A"
//    let cmd = sprintf """commit -a -m "Update binaries for version %s""" release.NugetVersion
//    CommandHelper.runSimpleGitCommand "temp/release" cmd |> printfn "%s"
//    Branches.push "temp/release"
//)

//Target "TagRelease" (fun _ ->
//    // Concatenate notes & create a tag in the local repository
//    let notes = (String.concat " " release.Notes).Replace("\n", ";").Replace("\r", "")
//    let tagName = "v" + release.NugetVersion
//    let cmd = sprintf """tag -a %s -m "%s" """ tagName notes
//    CommandHelper.runSimpleGitCommand "." cmd |> printfn "%s"
//
//    // Find the main remote (BlueMountain GitHub)
//    let _, remotes, _ = CommandHelper.runGitCommand "." "remote -v"
//    let main = remotes |> Seq.find (fun s -> s.Contains("(push)") && s.Contains("BlueMountainCapital/FSharpRProvider"))
//    let remoteName = main.Split('\t').[0]
//    Fake.Git.Branches.pushTag "." remoteName tagName
//)

Target "All" DoNothing
Target "NuGet" DoNothing
Target "Release" DoNothing
Target "Test" DoNothing
Target "Clean" DoNothing

//"Clean"
//  ==> "Build"
//  ==> "BuildTests"
//  ==> "RunTests"
//  ==> "All"

// Note to self: ==> should be read as "comes before"
"VersionInfo" ==> "All" 
"Clean" ==> "Rebuild" 
"Build" ==> "Rebuild" 
"Build" ==> "All" 

"NuGetRDotNet" ==> "NuGet"
"NuGetRDotNetFs" ==> "NuGet"
  
"All" ==> "RunTests" ==> "Test"
"All" ==> "NuGet" ==> "Release"

RunTargetOrDefault "All"
