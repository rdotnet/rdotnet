namespace RDotNet

open NUnit.Framework

type ActivePatternsTest () =

   inherit RDotNetTestFixture ()

   [<Test>]
   member this.``match CharacterVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""LETTERS""") with
      | CharacterVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match ComplexVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""1i""") with
      | ComplexVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match IntegerVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""1L""") with
      | IntegerVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match LogicalVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""TRUE""") with
      | LogicalVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match NumericVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""1""") with
      | NumericVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match RawVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""raw(0)""") with
      | RawVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match Factor pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""factor(letters)""") with
      | Factor (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``factor should not match IntegerVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""factor(letters)""") with
      | IntegerVector (_) -> Assert.Fail ("factor matched with integer vector")
      | _ -> ()

   [<Test>]
   member this.``character vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""LETTERS""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``complex vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""1i""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``integer vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""1L""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``logical vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""TRUE""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``numeric vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""pi""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``raw vector matches UntypedVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""raw(0)""") with
      | UntypedVector (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match CharacterMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
      | CharacterMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match ComplexMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(1i, 1, 1)""") with
      | ComplexMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match IntegerMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(1L, 1, 1)""") with
      | IntegerMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match LogicalMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
      | LogicalMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match NumericMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
      | NumericMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``match RawMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
      | RawMatrix (_) -> ()
      | _ -> Assert.Fail ("not matched")

   [<Test>]
   member this.``chracter matrix should not match CharacterVector pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
      | CharacterVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``complex matrix should not match ComplexMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(1i, 1, 1)""") with
      | ComplexVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``integer matrix should not match IntegerMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(1L, 1, 1)""") with
      | ComplexVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``logical matrix should not match LogicalMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
      | LogicalVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``numeric matrix should not match NumericMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
      | NumericVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``raw matrix should not match RawMatrix pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
      | RawVector (_) -> Assert.Fail ("matched with vector pattern")
      | _ -> ()

   [<Test>]
   member this.``match DataFrame pattern`` () =
      let engine = this.Engine
      match engine.Evaluate ("""data.frame()""") with
      | DataFrame (_) -> ()
      | _ -> Assert.Fail ("not matched")