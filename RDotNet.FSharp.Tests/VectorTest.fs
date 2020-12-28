namespace RDotNet

open Xunit

type VectorTest () =

    inherit RDotNetTestFixture ()

    [<Fact>]
    member this.``can slice vector all`` () =
        this.SetUpTest()
        let vector =
            let engine = this.Engine
            engine.Evaluate ("""0:5""") |> S.AsInteger
        let actual = vector.[*]
        Assert.IsType<IntegerVector>(actual)
        Assert.Equal (actual, ([0..5]))

    [<Fact>]
    member this.``can slice vector from start to specific index`` () =
        this.SetUpTest()
        let vector =
            let engine = this.Engine
            engine.Evaluate ("""0:5""") |> S.AsInteger
        let actual = vector.[..4]
        Assert.IsType<IntegerVector> (actual)
        Assert.Equal (actual, ([0..4]))

    [<Fact>]
    member this.``can slice vector from specific index to end`` () =
        this.SetUpTest()
        let vector =
            let engine = this.Engine
            engine.Evaluate ("""0:5""") |> S.AsInteger
        let actual = vector.[2..]
        Assert.IsType<IntegerVector> (actual)
        Assert.Equal (actual, ([2..5]))

    [<Fact>]
    member this.``can slice vector from specific index to specific index`` () =
        this.SetUpTest()
        let vector =
            let engine = this.Engine
            engine.Evaluate ("""0:5""") |> S.AsInteger
        let actual = vector.[2..4]
        Assert.IsType<IntegerVector> (actual)
        Assert.Equal (actual, ([2..4]))