#r @"FakeLib.dll"
#r @"ICSharpCode.SharpZipLib.dll"

open System
open System.IO
open System.Reflection
open Fake

let baseDir = Path.GetDirectoryName (__SOURCE_DIRECTORY__)
let inline (~%) name = Path.Combine (baseDir, name)
let inline (%) dir name = Path.Combine (dir, name)

let projects = ["R.NET"; "RDotNet.NativeLibrary"; "RDotNet.FSharp"; "RDotNet.Graphics"]
let nugetToolPath = % ".nuget" % "NuGet.exe"
let outputDir = % "Build"
let deployDir = % "Deploy"
let mainSolution = % "RDotNet.Release.sln"
let rdotnetMain = "RDotNet"
let rdotnetFSharp = "RDotNet.FSharp"
let rdotnetGraphics = "RDotNet.Graphics"

type BuildParameter = {
   Help : bool
   Debug : bool
   CleanDeploy : bool
   NoNuGet : bool
   Unix : bool
   Key : string option
}
let buildParams =
   let rec loop acc = function
      | [] -> acc
      | "-h" :: _ | "--help" :: _ -> { acc with Help = true }  // don't care other arguments
      | "--debug" :: args -> loop { acc with Debug = true } args
      | "--clean-deploy" :: args -> loop { acc with CleanDeploy = true } args
      | "--no-nuget" :: args -> loop { acc with NoNuGet = true } args
      | "--unix" :: args -> loop { acc with Unix = true } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | _ :: args -> loop acc args  // ignores unknown argument
   let defaultBuildParam = {
      Help = false
      Debug = false
      CleanDeploy = false
      NoNuGet = false
      Unix = false
      Key = None
   }
   let args = fsi.CommandLineArgs |> Array.toList  // args = ["build.fsx"; ...]
   loop defaultBuildParam args.Tail

if buildParams.Help then
   printfn """R.NET Build Script

#Usage

fsi.exe build.fsx [<options>]

# Options
-h | --help       Show this help
--debug           Debug build
--clean-deploy    Clean up deploy directory before build
--no-nuget        Do not build NuGet packages
--unix            Build with UNIX symbol (force --no-nuget)
--key <key_path>  Sign assembly with the specified key"""
   exit 0

let zipName = deployDir % if buildParams.Unix then "RDotNet.Unix.zip" else "RDotNet.Windows.zip"

let addBuildProperties =
   let debugSymbol properties =
      match buildParams.Debug with
         | true -> ("DebugSymbols", "true") :: ("DebugType", "full") :: properties
         | false -> ("DebugSymbols", "false") :: ("DebugType", "pdbonly") :: properties
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

Target "CleanBuild" (fun () ->
   build setCleanParams mainSolution
   CleanDir outputDir
)
Target "CleanDeploy" (fun () ->
   CleanDir deployDir
)
Target "Clean" DoNothing

let getProducts projectName =
   if buildParams.Debug then ["*"] else ["dll"; "XML"]
   |> Seq.collect (fun s -> % projectName % "bin" % snd configuration % (sprintf "*.%s" s) |> (!+) |> Scan)
Target "Build" (fun () ->
   build setBuildParams mainSolution
   projects
   |> Seq.collect getProducts
   |> Copy outputDir
)

let getMainAssemblyVersion assemblyPath =
   let assembly = Assembly.LoadFrom (assemblyPath)  // cannot get attributes with ReflectionOnlyLoadFrom
   let infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute> ()
   infoVersion.InformationalVersion
let updateNuGetParams version (p:NuGetParams) = {
   p with
      NoPackageAnalysis = false
      OutputPath = deployDir
      ToolPath = nugetToolPath
      WorkingDir = baseDir
      Version = version
}
let pack projectName =
   let assemblyName = sprintf "%s.dll" projectName
   let assemblyPath = outputDir % assemblyName
   let version = getMainAssemblyVersion assemblyPath
   let nuspecPath = % (sprintf "%s.nuspec" projectName)
   NuGetPack (updateNuGetParams version) nuspecPath
Target "NuGetMain" (fun () ->
   pack rdotnetMain
)
Target "NuGetFSharp" (fun () ->
   pack rdotnetFSharp
)
Target "NuGetGraphics" (fun () ->
   pack rdotnetGraphics
)
Target "NuGet" DoNothing

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

// Clean dependency
"CleanBuild"
=?> ("CleanDeploy", buildParams.CleanDeploy)
==> "Clean"

// Build dependency
"Clean"
==> "Build"

// NuGet dependency
"Build"
==> "NuGetMain"
==> "NuGetFSharp" <=> "NuGetGraphics"
==> "NuGet"

// Zip dependency
"Build"
==> "Zip"

// Deploy dependency
"Zip"
=?> ("NuGet", not (buildParams.NoNuGet || buildParams.Unix))
==> "Deploy"

Run "Deploy"
