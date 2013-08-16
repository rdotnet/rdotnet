[<NUnit.Framework.TestFixture>]
module RDotNet.ActivePatternsTest

open NUnit.Framework

let engineName = "RDotNetTest"

[<TestFixtureSetUp>]
let setUpFixture () =
   RDotNet.Helper.SetEnvironmentVariables ()
   let engine = REngine.CreateInstance (engineName)
   engine.Initialize ()

[<TestFixtureTearDown>]
let tearDownFixture () =
   match REngine.GetInstanceFromID (engineName) with
      | null -> ()
      | engine -> engine.Dispose ()

[<SetUp>]
let setUp () =
   match REngine.GetInstanceFromID (engineName) with
      | null -> failwith "engine not found"
      | engine -> engine.Evaluate ("""rm(list=ls())""") |> ignore

[<Test>]
let ``match CharacterVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""LETTERS""") with
   | CharacterVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match ComplexVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""1i""") with
   | ComplexVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match IntegerVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""1L""") with
   | IntegerVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match LogicalVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""TRUE""") with
   | LogicalVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match NumericVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""1""") with
   | NumericVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match RawVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""raw(0)""") with
   | RawVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match Factor pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""factor(letters)""") with
   | Factor (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``factor should not match IntegerVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""factor(letters)""") with
   | IntegerVector (_) -> Assert.Fail ("factor matched with integer vector")
   | _ -> ()

[<Test>]
let ``character vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""LETTERS""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``complex vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""1i""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``integer vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""1L""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``logical vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""TRUE""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``numeric vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""pi""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``raw vector matches UntypedVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""raw(0)""") with
   | UntypedVector (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match CharacterMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
   | CharacterMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match ComplexMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(1i, 1, 1)""") with
   | ComplexMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match IntegerMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(1L, 1, 1)""") with
   | IntegerMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match LogicalMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
   | LogicalMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match NumericMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
   | NumericMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``match RawMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
   | RawMatrix (_) -> ()
   | _ -> Assert.Fail ("not matched")

[<Test>]
let ``chracter matrix should not match CharacterVector pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""as.matrix(LETTERS, 13, 2)""") with
   | CharacterVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()

[<Test>]
let ``complex matrix should not match ComplexMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(1i, 1, 1)""") with
   | ComplexVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()

[<Test>]
let ``integer matrix should not match IntegerMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(1L, 1, 1)""") with
   | ComplexVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()

[<Test>]
let ``logical matrix should not match LogicalMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(c(TRUE, FALSE), 1, 2)""") with
   | LogicalVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()

[<Test>]
let ``numeric matrix should not match NumericMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(runif(12), 3, 4)""") with
   | NumericVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()

[<Test>]
let ``raw matrix should not match RawMatrix pattern`` () =
   let engine = REngine.GetInstanceFromID (engineName)
   match engine.Evaluate ("""matrix(as.raw(seq_len(32)), 4, 8)""") with
   | RawVector (_) -> Assert.Fail ("matched with vector pattern")
   | _ -> ()
