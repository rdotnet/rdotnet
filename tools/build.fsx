#r @"FakeLib.dll"
#r @"ICSharpCode.SharpZipLib.dll"

open System
open System.IO
open System.Reflection
open Fake

let baseDir = Path.GetDirectoryName (__SOURCE_DIRECTORY__)
let inline (~%) name = Path.Combine (baseDir, name)
let inline (%) dir name = Path.Combine (dir, name)

let projects = ["R.NET"; "RDotNet.NativeLibrary"; "RDotNet.FSharp"]
let nugetToolPath = % ".nuget" % "NuGet.exe"
let outputDir = % "Build"
let rdotnetAssemblyName = "RDotNet.dll"
let rdotnetFSharpAssemblyName = "RDotNet.FSharp.dll"
let deployDir = % "Deploy"
let mainSolution = % "RDotNet.FSharp.sln"
let rdotnetNuspec = % "RDotNet.nuspec"
let rdotnetFSharpNuspec = % "RDotNet.FSharp.nuspec"

type BuildParameter = {
   Debug : bool
   Unix : bool
   Key : string option
}
let buildParams =
   let rec loop acc = function
      | [] -> acc
      | "--debug" :: args -> loop { acc with Debug = true } args
      | "--unix" :: args -> loop { acc with Unix = true } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | _ :: args -> loop acc args  // ignores unknown argument
   let defaultBuildParam = { Debug = false; Unix = false; Key = None }
   let args = fsi.CommandLineArgs |> Array.toList  // args = ["build.fsx"; ...]
   loop defaultBuildParam args.Tail

let zipName = deployDir % if buildParams.Unix then "RDotNet.Unix.zip" else "RDotNet.Windows.zip"

let addBuildProperties =
   let debugSymbol properties =
      match buildParams.Debug with
         | true -> ("DebugSymbols", "true") :: ("DebugType", "full") :: properties
         | false -> ("DebugSymbols", "false") :: ("DebugType", "none") :: properties
   let definieUnix properties =
      match buildParams.Unix with
         | true -> ("DefineConstants", "UNIX") :: properties
         | false -> properties
   let setKey properties =
      match buildParams.Key with
         | Some (path) when File.Exists (path) -> ("SignAssembly", "true") :: ("AssemblyOriginatorKeyFile", path) :: properties
         | Some (path) -> failwithf "Key file not found at %s" path
         | None -> properties
   debugSymbol >> definieUnix >> setKey
let configuration = "Configuration", if buildParams.Debug then "Debug" else "Release"
let setBuildParams (p:MSBuildParams) = {
   p with
      Targets = ["Build"]
      Properties = configuration :: addBuildProperties p.Properties
}
let setCleanParams (p:MSBuildParams) = {
   p with
      Targets = ["Clean"]
      Properties = [configuration]
}

Target "Clean" (fun () ->
   build setCleanParams mainSolution
   CleanDir outputDir
   CleanDir deployDir
)

let getProducts projectName = % projectName % "bin" % snd configuration % "*.*" |> (!+) |> Scan
Target "Build" (fun () ->
   build setBuildParams mainSolution
   projects
   |> Seq.collect getProducts
   |> Copy outputDir
)

let getMainAssemblyVersion assemblyPath =
   let assembly = Assembly.ReflectionOnlyLoadFrom (assemblyPath)
   let name = assembly.GetName ()
   sprintf "%A" name.Version
let updateNuGetParams assemblyPath (p:NuGetParams) = {
   p with
      NoPackageAnalysis = false
      OutputPath = deployDir
      ToolPath = nugetToolPath
      WorkingDir = baseDir
      Version = getMainAssemblyVersion assemblyPath
}
let pack mainAssemblyPath = NuGetPack (updateNuGetParams mainAssemblyPath)
Target "NuGetMain" (fun () ->
   let path = outputDir % rdotnetAssemblyName
   pack path rdotnetNuspec
)
Target "NuGetFSharp" (fun () ->
   let path = outputDir % rdotnetFSharpAssemblyName
   pack path rdotnetFSharpNuspec
)

Target "Zip" (fun () ->
   !+ (outputDir % "*.*")
   |> Scan
   |> Zip outputDir zipName
)

Target "Deploy" (fun () ->
   !+ (deployDir % "*.*")
   |> Scan
   |> Log "Build-Output: "
)

if buildParams.Unix then
   "Clean"
   ==> "Build"
   ==> "Zip"
   ==> "Deploy"
else
   "Clean"
   ==> "Build"
   ==> "NuGetMain"
   ==> "NuGetFSharp"
   ==> "Zip"
   ==> "Deploy"

Run "Deploy"
