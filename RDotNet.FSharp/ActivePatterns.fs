module RDotNet.ActivePatterns

open RDotNet.Internals

// Port from RProvider's RInterop.fs
// https://github.com/BlueMountainCapital/FSharpRProvider
let (|CharacterVector|_|) (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.CharacterVector then Some(sexp.AsCharacter()) else None
let (|ComplexVector|_|)   (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.ComplexVector   then Some(sexp.AsComplex()) else None
let (|IntegerVector|_|)   (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.IntegerVector   then Some(sexp.AsInteger()) else None
let (|LogicalVector|_|)   (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.LogicalVector   then Some(sexp.AsLogical()) else None
let (|NumericVector|_|)   (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.NumericVector   then Some(sexp.AsNumeric()) else None

let (|Function|_|)        (sexp: SymbolicExpression)  = 
   if sexp <> null && (sexp.Type = SymbolicExpressionType.BuiltinFunction || sexp.Type = SymbolicExpressionType.Closure || sexp.Type = SymbolicExpressionType.SpecialFunction) then 
      Some(sexp.AsFunction()) else None

let (|BuiltinFunction|_|) (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.BuiltinFunction then Some(sexp.AsFunction() :?> BuiltinFunction) else None
let (|Closure|_|)         (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.Closure then Some(sexp.AsFunction() :?> Closure) else None
let (|SpecialFunction|_|) (sexp: SymbolicExpression)  = if sexp <> null && sexp.Type = SymbolicExpressionType.SpecialFunction then Some(sexp.AsFunction() :?> SpecialFunction) else None

let (|Environment|_|)   (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.Environment  then Some(sexp.AsEnvironment()) else None
let (|Expression|_|)    (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.ExpressionVector then Some(sexp.AsExpression()) else None
let (|Language|_|)      (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.LanguageObject then Some(sexp.AsLanguage()) else None
let (|List|_|)          (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.List then Some(sexp.AsList()) else None     
let (|Pairlist|_|)      (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.Pairlist then Some(sexp :?> Pairlist) else None     
let (|Null|_|)          (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.Null then Some() else None
let (|Symbol|_|)        (sexp: SymbolicExpression)    = if sexp <> null && sexp.Type = SymbolicExpressionType.Symbol then Some(sexp.AsSymbol()) else None
