namespace RDotNet

open Xunit

type ActivePatternsTest () =

    inherit RDotNetTestFixture ()

    [<Fact>]
    member this.``match CharacterVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""LETTERS""") with
        | CharacterVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match ComplexVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""1i""") with
        | ComplexVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match IntegerVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""1L""") with
        | IntegerVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match LogicalVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""TRUE""") with
        | LogicalVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match NumericVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""1""") with
        | NumericVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match RawVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""raw(0)""") with
        | RawVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match Factor pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""factor(letters)""") with
        | Factor (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``factor should not match IntegerVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""factor(letters)""") with
        | IntegerVector (_) -> this.AssertFail ("factor matched with integer vector")
        | _ -> ()

    [<Fact>]
    member this.``character vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""LETTERS""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``complex vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""1i""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``integer vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""1L""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``logical vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""TRUE""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``numeric vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""pi""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``raw vector matches UntypedVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""raw(0)""") with
        | UntypedVector (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match CharacterMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
        | CharacterMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match ComplexMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(1i, 1, 1)""") with
        | ComplexMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match IntegerMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(1L, 1, 1)""") with
        | IntegerMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match LogicalMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
        | LogicalMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match NumericMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
        | NumericMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``match RawMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
        | RawMatrix (_) -> ()
        | _ -> this.AssertFail ("not matched")

    [<Fact>]
    member this.``chracter matrix should not match CharacterVector pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
        | CharacterVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``complex matrix should not match ComplexMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(1i, 1, 1)""") with
        | ComplexVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``integer matrix should not match IntegerMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(1L, 1, 1)""") with
        | ComplexVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``logical matrix should not match LogicalMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
        | LogicalVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``numeric matrix should not match NumericMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
        | NumericVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``raw matrix should not match RawMatrix pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
        | RawVector (_) -> this.AssertFail ("matched with vector pattern")
        | _ -> ()

    [<Fact>]
    member this.``match DataFrame pattern`` () =
        this.SetUpTest()
        let engine = this.Engine
        match engine.Evaluate ("""data.frame()""") with
        | DataFrame (_) -> ()
        | _ -> this.AssertFail ("not matched")