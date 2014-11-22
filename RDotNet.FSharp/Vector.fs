module RDotNet.Vector

let inline copy (v:Vector<'a>) start finish =
   let start = defaultArg start 0
   let finish = defaultArg finish (v.Length - 1)
   if start < 0 then
      outOfBounds "rowStart" "Negative value is passed"
   if finish < start then
      outOfBounds "rowFinish" "rowFinish is less than rowStart"
   if v.Length <= finish then
      outOfBounds "rowFinish" "rowFinish is out of bounds"
   let length = finish - start + 1
   let array = Array.zeroCreate<'a> length
   v.CopyTo (array, length, start)
   array

type CharacterVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateCharacterVector
type ComplexVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateComplexVector
type IntegerVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateIntegerVector
type LogicalVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateLogicalVector
type NumericVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateNumericVector
type RawVector with
   member v.GetSlice (start, finish) = copy v start finish |> v.Engine.CreateRawVector