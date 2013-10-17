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
let mainProject = "RDotNet"
let fsharpProject = "RDotNet.FSharp"
let graphicsProject = "RDotNet.Graphics"
let mainSolution = % "RDotNet.Release.sln"

let nugetToolPath = % ".nuget" % "NuGet.exe"
let buildDir = % "Build"
let deployDir = % "Deploy"
let deployExtensions = ["dll"; "XML"; "fsx"]

type BuildParameter = {
   Help : bool
   Debug : bool
   CleanDeploy : bool
   NoZip : bool
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
      | "--no-zip" :: args -> loop { acc with NoZip = true } args
      | "--no-nuget" :: args -> loop { acc with NoNuGet = true } args
      | "--no-deploy" :: args -> loop { acc with NoZip = true; NoNuGet = true } args
      | "--unix" :: args -> loop { acc with Unix = true } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | _ :: args -> loop acc args  // ignores unknown argument
   let defaultBuildParam = {
      Help = false
      Debug = false
      CleanDeploy = false
      NoZip = false
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
--no-zip          Do not create zip archive
--no-nuget        Do not build NuGet packages
--no-deploy       Same as --no-zip --no-nuget
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
let buildZip = not buildParams.NoZip
let buildNuGet = not (buildParams.NoNuGet || buildParams.Unix)
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
   CleanDir buildDir
)
Target "CleanDeploy" (fun () ->
   CleanDir deployDir
)
Target "Clean" DoNothing

let getProducts projectName =
   if buildParams.Debug then ["*"] else deployExtensions
   |> Seq.collect (fun s -> % projectName % "bin" % snd configuration % (sprintf "*.%s" s) |> (!+) |> Scan)
Target "Build" (fun () ->
   build setBuildParams mainSolution
   projects
   |> Seq.collect getProducts
   |> Copy buildDir
   !+ (buildDir % "*.*")
   |> Scan
   |> Log "Build-Output: "
)
Target "EnsureDeploy" (fun () ->
   ensureDirectory deployDir
)

let getMainAssemblyVersion assemblyPath =
   let assembly = Assembly.LoadFrom (assemblyPath)  // cannot get attributes with ReflectionOnlyLoadFrom
   // Mono does not support GetCustomAttribute<T>.
   let infoVersion = assembly.GetCustomAttributes (typeof<AssemblyInformationalVersionAttribute>, false)
   (infoVersion.[0] :?> AssemblyInformationalVersionAttribute).InformationalVersion
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
   let assemblyPath = buildDir % assemblyName
   let version = getMainAssemblyVersion assemblyPath
   let nuspecPath = % (sprintf "%s.nuspec" projectName)
   NuGetPack (updateNuGetParams version) nuspecPath
Target "NuGetMain" (fun () ->
   pack mainProject
)
Target "NuGetFSharp" (fun () ->
   pack fsharpProject
)
Target "NuGetGraphics" (fun () ->
   pack graphicsProject
)
Target "NuGet" DoNothing

Target "Zip" (fun () ->
   !+ (buildDir % "*.*")
   |> Scan
   |> Zip buildDir zipName
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
==> "EnsureDeploy"
==> "NuGetMain"
==> "NuGetFSharp" <=> "NuGetGraphics"
==> "NuGet"

// Zip dependency
"Build"
==> "EnsureDeploy"
==> "Zip"

// Deploy
if buildZip || buildNuGet then
   "Build"
   =?> ("Zip", buildZip)
   =?> ("NuGet", buildNuGet)
   ==> "Deploy"
   |> Run
else
   Run "Build"
