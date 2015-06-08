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
let projectSummary = "Interoperability library to access R from .NET languages"
let projectDescription = """
  A C# interoperability library to access the R statistical package from .NET languages.
  The library is designed for fast data exchange of arrayw, in process."""
let authors = ["Kosei Abe"; "Jean-Michel Perraud"]
let companyName = ""
let tags = "C# R visualization statistics"

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
//Target "AssemblyInfo" (fun _ ->
//  let fileName = "src/Common/AssemblyInfo.fs"
//  CreateFSharpAssemblyInfoWithConfig fileName
//      [ Attribute.Title projectName
//        Attribute.Company companyName
//        Attribute.Product projectName
//        Attribute.Description projectSummary
//        Attribute.Version release.AssemblyVersion
//        Attribute.FileVersion release.AssemblyVersion ] 
//      (AssemblyInfoFileConfig(false))
//)

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

Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp" ]
    CleanDirs ["tests/R.NET/bin"; "tests/R.NET/obj" ]
)

Target "CleanDocs" (fun _ ->
    CleanDirs ["docs/output"]
)

// --------------------------------------------------------------------------------------
// Build library & test project

Target "Build" (fun _ ->
    !! ("RDotNet.sln")
    |> MSBuildRelease "" "Rebuild"
    |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ ->
    !! ("RDotNet.Tests.sln")
    |> MSBuildRelease "" "Rebuild"
    |> Log "AppBuild-Output: "
)

// --------------------------------------------------------------------------------------
// Run the unit tests using test runner & kill test runner when complete

Target "RunTests" (fun _ ->
    let nunitConsolePath = "packages/NUnit.Runners/tools/nunit-console.exe"

    ActivateFinalTarget "CloseTestRunner"

    !! "tests/Test.RProvider/bin/**/Test*.dll"
    |> xUnit (fun p -> 
            {p with 
                ToolPath = nunitConsolePath
                ShadowCopy = false
                HtmlOutput = true
                XmlOutput = true
                OutputDir = "." })
)
 
FinalTarget "CloseTestRunner" (fun _ ->  
    ProcessHelper.killProcess "nunit-console.exe"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target "NuGet" (fun _ ->
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
        "nuget/blah.nuspec"
)

// --------------------------------------------------------------------------------------
// Generate the documentation

Target "GenerateDocs" (fun _ ->
    executeFSIWithArgs "docs/tools" "generate.fsx" ["--define:RELEASE"] [] |> ignore
)

// --------------------------------------------------------------------------------------
// Release Scripts

Target "ReleaseDocs" (fun _ ->
    Repository.clone "" (gitHome + "/" + gitName + ".git") "temp/gh-pages"
    Branches.checkoutBranch "temp/gh-pages" "gh-pages"
    CopyRecursive "docs/output" "temp/gh-pages" true |> printfn "%A"
    CommandHelper.runSimpleGitCommand "temp/gh-pages" "add ." |> printfn "%s"
    let cmd = sprintf """commit -a -m "Update generated documentation for version %s""" release.NugetVersion
    CommandHelper.runSimpleGitCommand "temp/gh-pages" cmd |> printfn "%s"
    Branches.push "temp/gh-pages"
)

Target "ReleaseBinaries" (fun _ ->
    Repository.clone "" (gitHome + "/" + gitName + ".git") "temp/release" 
    Branches.checkoutBranch "temp/release" "release"
    CopyRecursive "bin" "temp/release" true |> printfn "%A"
    let cmd = sprintf """commit -a -m "Update binaries for version %s""" release.NugetVersion
    CommandHelper.runSimpleGitCommand "temp/release" cmd |> printfn "%s"
    Branches.push "temp/release"
)

Target "TagRelease" (fun _ ->
    // Concatenate notes & create a tag in the local repository
    let notes = (String.concat " " release.Notes).Replace("\n", ";").Replace("\r", "")
    let tagName = "v" + release.NugetVersion
    let cmd = sprintf """tag -a %s -m "%s" """ tagName notes
    CommandHelper.runSimpleGitCommand "." cmd |> printfn "%s"

    // Find the main remote (BlueMountain GitHub)
    let _, remotes, _ = CommandHelper.runGitCommand "." "remote -v"
    let main = remotes |> Seq.find (fun s -> s.Contains("(push)") && s.Contains("BlueMountainCapital/FSharpRProvider"))
    let remoteName = main.Split('\t').[0]
    Fake.Git.Branches.pushTag "." remoteName tagName
)

Target "Release" DoNothing

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing
Target "AllCore" DoNothing

"Clean"
  ==> "UpdateFsxVersions"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "MergeRProviderServer"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "All"

"MergeRProviderServer"
  ==> "AllCore"

"All" 
  ==> "CleanDocs" 
  ==> "GenerateDocs" 
  ==> "ReleaseDocs" 
  ==> "ReleaseBinaries" 
  ==> "Release"
  
"All" ==> "NuGet" ==> "Release"
"All" ==> "TagRelease" ==> "Release"

RunTargetOrDefault "All"