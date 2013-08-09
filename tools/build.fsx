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
let rdotnet = "RDotNet"
let rdotnetFSharp = "RDotNet.FSharp"
let rdotnetGraphics = "RDotNet.Graphics"

type BuildParameter = {
   Debug : bool
   Unix : bool
   VersionSuffix : Map<string, string>
   Key : string option
}
let buildParams =
   let rec loop acc = function
      | [] -> acc
      | "--debug" :: args -> loop { acc with Debug = true } args
      | "--unix" :: args -> loop { acc with Unix = true } args
      | "--pre" :: pre :: args ->
         loop {
            acc with
               VersionSuffix =
                  pre.Split (';')
                  |> Array.map (fun keyvalue -> let keyvalue = keyvalue.Split ([|'='|], 2) in keyvalue.[0], keyvalue.[1])
                  |> Array.fold (fun map (key, value) -> Map.add key value map) acc.VersionSuffix
         } args
      | "--key" :: path :: args -> loop { acc with Key = Some (path) } args
      | _ :: args -> loop acc args  // ignores unknown argument
   let defaultBuildParam = { Debug = false; Unix = false; VersionSuffix = Map.empty; Key = None }
   let args = fsi.CommandLineArgs |> Array.toList  // args = ["build.fsx"; ...]
   loop defaultBuildParam args.Tail

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

Target "Clean" (fun () ->
   build setCleanParams mainSolution
   CleanDir outputDir
   CleanDir deployDir
)

let getProducts projectName =
   if buildParams.Debug then ["*"] else ["dll"; "XML"]
   |> Seq.collect (fun s -> % projectName % "bin" % snd configuration % (sprintf "*.%s" s) |> (!+) |> Scan)
Target "Build" (fun () ->
   build setBuildParams mainSolution
   projects
   |> Seq.collect getProducts
   |> Copy outputDir
)

let getMainAssemblyVersion assemblyPath versionSuffix =
   let assembly = Assembly.ReflectionOnlyLoadFrom (assemblyPath)
   let name = assembly.GetName ()
   let version = sprintf "%A" name.Version
   match versionSuffix with
      | Some (suffix) -> sprintf "%s-%s" version suffix
      | None -> version
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
   let version = getMainAssemblyVersion assemblyPath <| Map.tryFind projectName buildParams.VersionSuffix
   let nuspecPath = % (sprintf "%s.nuspec" projectName)
   NuGetPack (updateNuGetParams version) nuspecPath
Target "NuGetMain" (fun () ->
   pack rdotnet
)
Target "NuGetFSharp" (fun () ->
   pack rdotnetFSharp
)
Target "NuGetGraphics" (fun () ->
   pack rdotnetGraphics
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
   ==> "NuGetFSharp" <=> "NuGetGraphics"
   ==> "Zip"
   ==> "Deploy"

Run "Deploy"
