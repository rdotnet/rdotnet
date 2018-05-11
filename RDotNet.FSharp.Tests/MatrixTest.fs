namespace RDotNet

open Xunit

type S = SymbolicExpressionExtension

type MatrixTest () =
    inherit RDotNetTestFixture ()

    // 1  4  7  10
    // 2  5  8  11
    // 3  6  9  12
    [<Fact>]
    member this.``can slice matrix all`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[*, *]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (1))
        Assert.Equal (actual.[1, 0], (2))
        Assert.Equal (actual.[2, 0], (3))
        Assert.Equal (actual.[0, 1], (4))
        Assert.Equal (actual.[1, 1], (5))
        Assert.Equal (actual.[2, 1], (6))
        Assert.Equal (actual.[0, 2], (7))
        Assert.Equal (actual.[1, 2], (8))
        Assert.Equal (actual.[2, 2], (9))
        Assert.Equal (actual.[0, 3], (10))
        Assert.Equal (actual.[1, 3], (11))
        Assert.Equal (actual.[2, 3], (12))

    // | 1  4  7  10 |
    // | 2  5  8  11 |
    // +-------------+
    //   3  6  9  12
    [<Fact>]
    member this.``can slice matrix rows from start to specific index`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[..1, *]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (1))
        Assert.Equal (actual.[1, 0], (2))
        Assert.Equal (actual.[0, 1], (4))
        Assert.Equal (actual.[1, 1], (5))
        Assert.Equal (actual.[0, 2], (7))
        Assert.Equal (actual.[1, 2], (8))
        Assert.Equal (actual.[0, 3], (10))
        Assert.Equal (actual.[1, 3], (11))

    //   1  4  7  10
    // +-------------+
    // | 2  5  8  11 |
    // | 3  6  9  12 |
    [<Fact>]
    member this.``can slice matrix rows from specific index to end`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[1.., *]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (2))
        Assert.Equal (actual.[1, 0], (3))
        Assert.Equal (actual.[0, 1], (5))
        Assert.Equal (actual.[1, 1], (6))
        Assert.Equal (actual.[0, 2], (8))
        Assert.Equal (actual.[1, 2], (9))
        Assert.Equal (actual.[0, 3], (11))
        Assert.Equal (actual.[1, 3], (12))

    //   1  4  7  10
    // +-------------+
    // | 2  5  8  11 |
    // +-------------+
    //   3  6  9  12
    [<Fact>]
    member this.``can slice matrix rows from specific index to specific index`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[1..1, *]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (2))
        Assert.Equal (actual.[0, 1], (5))
        Assert.Equal (actual.[0, 2], (8))
        Assert.Equal (actual.[0, 3], (11))

    // Requires F# 3.1 to compile.
    //   1  4  7  10
    // +-------------+
    // | 2  5  8  11 |
    // +-------------+
    //   3  6  9  12
    //[<Fact>]
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
    [<Fact>]
    member this.``can slice matrix columns from start to specific index`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[*, ..2]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (1))
        Assert.Equal (actual.[1, 0], (2))
        Assert.Equal (actual.[2, 0], (3))
        Assert.Equal (actual.[0, 1], (4))
        Assert.Equal (actual.[1, 1], (5))
        Assert.Equal (actual.[2, 1], (6))
        Assert.Equal (actual.[0, 2], (7))
        Assert.Equal (actual.[1, 2], (8))
        Assert.Equal (actual.[2, 2], (9))

    //      +------
    // 1  4 | 7  10
    // 2  5 | 8  11
    // 3  6 | 9  12
    //      +------
    [<Fact>]
    member this.``can slice matrix columns from specific index to end`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[*, 2..]
        Assert.IsType<IntegerMatrix>(actual)
        Assert.Equal (actual.[0, 0], (7))
        Assert.Equal (actual.[1, 0], (8))
        Assert.Equal (actual.[2, 0], (9))
        Assert.Equal (actual.[0, 1], (10))
        Assert.Equal (actual.[1, 1], (11))
        Assert.Equal (actual.[2, 1], (12))

    //   +------+
    // 1 | 4  7 | 10
    // 2 | 5  8 | 11
    // 3 | 6  9 | 12
    //   +------+
    [<Fact>]
    member this.``can slice matrix columns from specific index to specific index`` () =
        this.SetUpTest()
        let matrix =
            let engine = this.Engine
            engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
        let actual = matrix.[*, 1..2]
        Assert.IsType<IntegerMatrix> (actual)
        Assert.Equal (actual.[0, 0], (4))
        Assert.Equal (actual.[1, 0], (5))
        Assert.Equal (actual.[2, 0], (6))
        Assert.Equal (actual.[0, 1], (7))
        Assert.Equal (actual.[1, 1], (8))
        Assert.Equal (actual.[2, 1], (9))

    // Requires F# 3.1 to compile.
    //   +---+
    // 1 | 4 | 7  10
    // 2 | 5 | 8  11
    // 3 | 6 | 9  12
    //   +---+
    //[<Fact>]
    //member this.``can slice matrix specific column`` () =
    //   let matrix =
    //      let engine = this.Engine
    //      engine.Evaluate ("""matrix(1:12, 3, 4)""") |> S.AsIntegerMatrix
    //   let actual = matrix.[*, 1]
    //   Assert.That (actual, Is.TypeOf<IntegerVector> ())
    //   Assert.That (actual, Is.EquivalentTo ([4..6]))