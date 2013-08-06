module RDotNet.Matrix

let inline checkRows (m:Matrix<'a>) rowStart rowFinish =
   let rowStart = defaultArg rowStart 0
   let rowFinish = defaultArg rowFinish (m.RowCount - 1)
   if rowStart < 0 then
      outOfBounds "rowStart" "Negative value is passed"
   if rowFinish < rowStart then
      outOfBounds "rowFinish" "rowFinish is less than rowStart"
   if m.RowCount <= rowFinish then
      outOfBounds "rowFinish" "rowFinish is out of bounds"
   rowStart, rowFinish
let inline checkRow (m:Matrix<'a>) row =
   if row < 0 || m.RowCount <= row then
      outOfBounds "row" "row is out of bounds"
let inline checkColumns (m:Matrix<'a>) colStart colFinish =
   let colStart = defaultArg colStart 0
   let colFinish = defaultArg colFinish (m.ColumnCount - 1)
   if colStart < 0 then
      outOfBounds "colStart" "Negative value is passed"
   if colFinish < colStart then
      outOfBounds "colFinish" "colFinish is less than colStart"
   if m.ColumnCount <= colFinish then
      outOfBounds "colFinish" "colFinish is out of bounds"
   colStart, colFinish
let inline checkColumn (m:Matrix<'a>) col =
   if col < 0 || m.ColumnCount <= col then
      outOfBounds "col" "col is out of bounds"
let inline sliceMatrix (m:Matrix<'a>) rowStart rowFinish colStart colFinish =
   let rowCount = rowFinish - rowStart + 1
   let colCount = colFinish - colStart + 1
   let array = Array2D.zeroCreate rowCount colCount
   m.CopyTo (array, rowCount, colCount, rowStart, colStart)
   array
let inline sliceRow (m:Matrix<'a>) row colStart colFinish =
   let colCount = colFinish - colStart + 1
   let array = Array.zeroCreate colCount
   for col in colStart .. colFinish do
      array.[col - colStart] <- m.[row, col]
   array
let inline sliceColumn (m:Matrix<'a>) rowStart rowFinish col =
   let rowCount = rowFinish - rowStart + 1
   let array = Array.zeroCreate rowCount
   for row in rowStart .. rowFinish do
      array.[row - rowStart] <- m.[row, col]
   array

type CharacterMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateCharacterMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateCharacterVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateCharacterVector (array)

type ComplexMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateComplexMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateComplexVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateComplexVector (array)

type IntegerMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateIntegerMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateIntegerVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateIntegerVector (array)

type LogicalMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateLogicalMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateLogicalVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateLogicalVector (array)

type NumericMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateNumericMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateNumericVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateNumericVector (array)

type RawMatrix with
   member m.GetSlice (rowStart, rowFinish, colStart, colFinish) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceMatrix m rowStart rowFinish colStart colFinish
      m.Engine.CreateRawMatrix (array)
   member m.GetSlice (row, colStart, colFinish) =
      checkRow m row
      let colStart, colFinish = checkColumns m colStart colFinish
      let array = sliceRow m row colStart colFinish
      m.Engine.CreateRawVector (array)
   member m.GetSlice (rowStart, rowFinish, col) =
      let rowStart, rowFinish = checkRows m rowStart rowFinish
      let array = sliceColumn m rowStart rowFinish col
      m.Engine.CreateRawVector (array)