[<AutoOpen>]
module internal RDotNet.RuntimeHelper

open System

let inline outOfBounds (paramName:string) (message:string) =
   raise <| ArgumentOutOfRangeException (paramName, message)