namespace RDotNet

open NUnit.Framework

type S = SymbolicExpressionExtension

type MatrixTest () =
   inherit RDotNetTestFixture ()

   // 1  4  7  10
   // 2  5  8  11
   // 3  6  9  12
   [<Test>]
   member this.``can slice matrix all`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[*, *]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (1))
      Assert.That (actual.[1, 0], Is.EqualTo (2))
      Assert.That (actual.[2, 0], Is.EqualTo (3))
      Assert.That (actual.[0, 1], Is.EqualTo (4))
      Assert.That (actual.[1, 1], Is.EqualTo (5))
      Assert.That (actual.[2, 1], Is.EqualTo (6))
      Assert.That (actual.[0, 2], Is.EqualTo (7))
      Assert.That (actual.[1, 2], Is.EqualTo (8))
      Assert.That (actual.[2, 2], Is.EqualTo (9))
      Assert.That (actual.[0, 3], Is.EqualTo (10))
      Assert.That (actual.[1, 3], Is.EqualTo (11))
      Assert.That (actual.[2, 3], Is.EqualTo (12))

   // | 1  4  7  10 |
   // | 2  5  8  11 |
   // +-------------+
   //   3  6  9  12
   [<Test>]
   member this.``can slice matrix rows from start to specific index`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[..1, *]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (1))
      Assert.That (actual.[1, 0], Is.EqualTo (2))
      Assert.That (actual.[0, 1], Is.EqualTo (4))
      Assert.That (actual.[1, 1], Is.EqualTo (5))
      Assert.That (actual.[0, 2], Is.EqualTo (7))
      Assert.That (actual.[1, 2], Is.EqualTo (8))
      Assert.That (actual.[0, 3], Is.EqualTo (10))
      Assert.That (actual.[1, 3], Is.EqualTo (11))

   //   1  4  7  10
   // +-------------+
   // | 2  5  8  11 |
   // | 3  6  9  12 |
   [<Test>]
   member this.``can slice matrix rows from specific index to end`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[1.., *]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (2))
      Assert.That (actual.[1, 0], Is.EqualTo (3))
      Assert.That (actual.[0, 1], Is.EqualTo (5))
      Assert.That (actual.[1, 1], Is.EqualTo (6))
      Assert.That (actual.[0, 2], Is.EqualTo (8))
      Assert.That (actual.[1, 2], Is.EqualTo (9))
      Assert.That (actual.[0, 3], Is.EqualTo (11))
      Assert.That (actual.[1, 3], Is.EqualTo (12))

   //   1  4  7  10
   // +-------------+
   // | 2  5  8  11 |
   // +-------------+
   //   3  6  9  12
   [<Test>]
   member this.``can slice matrix rows from specific index to specific index`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[1..1, *]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (2))
      Assert.That (actual.[0, 1], Is.EqualTo (5))
      Assert.That (actual.[0, 2], Is.EqualTo (8))
      Assert.That (actual.[0, 3], Is.EqualTo (11))

   // Requires F# 3.1 to compile.
   //   1  4  7  10
   // +-------------+
   // | 2  5  8  11 |
   // +-------------+
   //   3  6  9  12
   //[<Test>]
   //member this.``can slice matrix specific row`` () =
   //   let matrix =
   //      let engine = this.Engine
   //      engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
   //   let actual = matrix.[1, *]
   //   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   //   Assert.That (actual, Is.EquivalentTo ([2..3..11]))

   // --------+
   // 1  4  7 | 10
   // 2  5  8 | 11
   // 3  6  9 | 12
   // --------+
   [<Test>]
   member this.``can slice matrix columns from start to specific index`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[*, ..2]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (1))
      Assert.That (actual.[1, 0], Is.EqualTo (2))
      Assert.That (actual.[2, 0], Is.EqualTo (3))
      Assert.That (actual.[0, 1], Is.EqualTo (4))
      Assert.That (actual.[1, 1], Is.EqualTo (5))
      Assert.That (actual.[2, 1], Is.EqualTo (6))
      Assert.That (actual.[0, 2], Is.EqualTo (7))
      Assert.That (actual.[1, 2], Is.EqualTo (8))
      Assert.That (actual.[2, 2], Is.EqualTo (9))

   //      +------
   // 1  4 | 7  10
   // 2  5 | 8  11
   // 3  6 | 9  12
   //      +------
   [<Test>]
   member this.``can slice matrix columns from specific index to end`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[*, 2..]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (7))
      Assert.That (actual.[1, 0], Is.EqualTo (8))
      Assert.That (actual.[2, 0], Is.EqualTo (9))
      Assert.That (actual.[0, 1], Is.EqualTo (10))
      Assert.That (actual.[1, 1], Is.EqualTo (11))
      Assert.That (actual.[2, 1], Is.EqualTo (12))

   //   +------+
   // 1 | 4  7 | 10
   // 2 | 5  8 | 11
   // 3 | 6  9 | 12
   //   +------+
   [<Test>]
   member this.``can slice matrix columns from specific index to specific index`` () =
      let matrix =
         let engine = this.Engine
         engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
      let actual = matrix.[*, 1..2]
      Assert.That (actual, Is.TypeOf<IntegerMatrix> ())
      Assert.That (actual.[0, 0], Is.EqualTo (4))
      Assert.That (actual.[1, 0], Is.EqualTo (5))
      Assert.That (actual.[2, 0], Is.EqualTo (6))
      Assert.That (actual.[0, 1], Is.EqualTo (7))
      Assert.That (actual.[1, 1], Is.EqualTo (8))
      Assert.That (actual.[2, 1], Is.EqualTo (9))

   // Requires F# 3.1 to compile.
   //   +---+
   // 1 | 4 | 7  10
   // 2 | 5 | 8  11
   // 3 | 6 | 9  12
   //   +---+
   //[<Test>]
   //member this.``can slice matrix specific column`` () =
   //   let matrix =
   //      let engine = this.Engine
   //      engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
   //   let actual = matrix.[*, 1]
   //   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   //   Assert.That (actual, Is.EquivalentTo ([4..6]))