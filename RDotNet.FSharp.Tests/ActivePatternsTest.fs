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