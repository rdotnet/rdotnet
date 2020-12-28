[<AutoOpen>]
module RDotNet.Matrix

type CharacterMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> CharacterMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> CharacterVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> CharacterVector
type ComplexMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> ComplexMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> ComplexVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> ComplexVector
type IntegerMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> IntegerMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> IntegerVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> IntegerVector
type LogicalMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> LogicalMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> LogicalVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> LogicalVector
type NumericMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> NumericMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> NumericVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> NumericVector
type RawMatrix with
   member GetSlice : rowStart:int option * rowFinish:int option * colStart:int option * colFinish:int option -> RawMatrix
   member GetSlice : row:int * colStart:int option * colFinish:int option -> RawVector
   member GetSlice : rowStart:int option * rowFinish:int option * col:int -> RawVector