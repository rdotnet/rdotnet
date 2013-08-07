// Path separators should be '/' for Unix environments.

#r @"FakeLib.dll"
#r @"ICSharpCode.SharpZipLib.dll"

open System
open System.IO
open Fake

let nugetToolPath = @"./.nuget/NuGet.exe"
let outputDir = @"./RDotNet.FSharp/bin/Release/"
let deployDir = @"./Deploy/"
let mainSolution = @"./RDotNet.FSharp.sln"
let rdotnetNuspec = @"./RDotNet.nuspec"
let rdotnetFSharpNuspec = @"./RDotNet.FSharp.nuspec"

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
   let args = Environment.GetCommandLineArgs () |> Array.toList  // args = ["fsi.exe"; "build.fsx"; ...]
   loop defaultBuildParam args.Tail.Tail

let zipName = if buildParams.Unix then @"./RDotNet.Unix.zip" else @"./RDotNet.Windows.zip"

let addBuildProperties =
   let definieUnix properties =
      match buildParams.Unix with
         | true -> ("DefineConstants", "UNIX") :: properties
         | false -> properties
   let setKey properties =
      match buildParams.Key with
         | Some (path) when File.Exists (path) -> ("SignAssembly", "true") :: ("AssemblyOriginatorKeyFile", path) :: properties
         | Some (path) -> failwithf "Key file not found at %s" path
         | None -> properties
   definieUnix >> setKey
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
   CleanDir deployDir
)

Target "Build" (fun () ->
   build setBuildParams mainSolution
)

let updateNuGetParams (p:NuGetParams) = { p with NoPackageAnalysis = false; OutputPath = "."; ToolPath = nugetToolPath; WorkingDir = "." }
let pack = NuGetPack updateNuGetParams
Target "NuGetMain" (fun () ->
   pack rdotnetNuspec
)
Target "NuGetFSharp" (fun () ->
   pack rdotnetFSharpNuspec
)

Target "Zip" (fun () ->
   !+ (Path.Combine (outputDir, "*.dll")) ++ (Path.Combine (outputDir, "*.xml"))
   |> Scan
   |> Zip outputDir zipName
)

Target "Deploy" (fun () ->
   ensureDirectory deployDir
   !+ zipName ++ @"./*.nupkg"
   |> Scan
   |> Seq.iter (MoveFile deployDir)
)

"Clean"
==> "Build"
==> "NuGetMain"
==> "NuGetFSharp"
==> "Zip"
==> "Deploy"

Run "Deploy"
