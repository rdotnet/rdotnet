[<AutoOpen>]
module RDotNet.Vector

type CharacterVector with
   member GetSlice : start:int option * finish:int option -> CharacterVector
type ComplexVector with
   member GetSlice : start:int option * finish:int option -> ComplexVector
type IntegerVector with
   member GetSlice : start:int option * finish:int option -> IntegerVector
type LogicalVector with
   member GetSlice : start:int option * finish:int option -> LogicalVector
type NumericVector with
   member GetSlice : start:int option * finish:int option -> NumericVector
type RawVector with
   member GetSlice : start:int option * finish:int option -> RawVector