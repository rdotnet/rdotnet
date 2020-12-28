#r "RDotNet.dll"
#r "RDotNet.FSharp.dll"

open RDotNet

type private S = SymbolicExpressionExtension
type private F = delegate of nativeint * nativeint -> nativeint
type private FunctionType =
   | Cons
   | LCons
   | Eval
let private invoke (engine:REngine) =
   function
      | Cons -> this.GetFunction<F> ("Rf_cons")
      | LCons -> this.GetFunction<F> ("Rf_lcons")
      | Eval -> this.GetFunction<F> ("Rf_eval")
   >> fun del -> del.Invoke
let private getHandle (sexp:SymbolicExpression) = sexp.DangerousGetHandle ()

fsi.AddPrinter (fun (sexp:SymbolicExpression) ->
   let engine = sexp.Engine
   use print = engine.BaseNamespace.GetSymbol ("print") |> S.AsFunction :?> Closure
   use env = engine.CreateEnvironment (print.Environment)
   let invoke = invoke engine
   let p = invoke Cons (getHandle sexp, getHandle engine.NilValue)
   let call = invoke LCons (getHandle print, p)
   invoke Eval (call, getHandle env) |> ignore
   sprintf "{ Type = %A }" sexp.Type
)