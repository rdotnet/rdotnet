// Modification of https://gist.github.com/3564010

module XmlEditor

open System
open System.IO
open Microsoft.FSharp.Core.LanguagePrimitives

type Trial<'a, 'b> =
   | Success of 'a
   | Failure of 'b

type Error =
   | None = 0
   | IO = 1
   | InvalidArgument = 2
   | InvalidInput = 3

let exit = EnumToValue >> Environment.Exit
   
let doEdit edit =
   function
      | Success input ->
         try
            Success (edit input)
         with
            | ex -> Failure (ex, Error.InvalidInput)
      | Failure (ex, error) ->
         Failure (ex, error)

let getUserInput () =
   match Environment.GetCommandLineArgs () with
      | [| _; _ |] ->
         try
            Success <| Console.In.ReadToEnd ()
         with
            | ex -> Failure (ex, Error.IO)
      | [| _; _; path |] ->
         try
            Success <| File.ReadAllText (path)
         with
            | ex -> Failure (ex, Error.IO)
      | args ->
         let filename = Path.GetFileName (args.[0])
         let message = sprintf "Usage: Fsi.exe %s [project.fsproj]" filename
         let ex = ApplicationException (message) :> exn
         Failure (ex, Error.InvalidArgument)

let main edit =
   getUserInput ()
   |> doEdit edit
   |> function
      | Success result ->
         printfn "%A" result
         Error.None
      | Failure (ex, error) ->
         eprintfn "%s" ex.Message
         error
   |> exit
