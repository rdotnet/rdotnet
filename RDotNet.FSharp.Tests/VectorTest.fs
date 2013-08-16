[<NUnit.Framework.TestFixture>]
module RDotNet.VectorTest

open NUnit.Framework

type S = SymbolicExpressionExtension

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
let ``can slice vector all`` () =
   let vector =
      let engine = REngine.GetInstanceFromID (engineName)
      engine.Evaluate ("""0:5""") |> S.AsInteger
   let actual = vector.[*]
   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   Assert.That (actual, Is.EquivalentTo ([0..5]))

[<Test>]
let ``can slice vector from start to specific index`` () =
   let vector =
      let engine = REngine.GetInstanceFromID (engineName)
      engine.Evaluate ("""0:5""") |> S.AsInteger
   let actual = vector.[..4]
   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   Assert.That (actual, Is.EquivalentTo ([0..4]))

[<Test>]
let ``can slice vector from specific index to end`` () =
   let vector =
      let engine = REngine.GetInstanceFromID (engineName)
      engine.Evaluate ("""0:5""") |> S.AsInteger
   let actual = vector.[2..]
   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   Assert.That (actual, Is.EquivalentTo ([2..5]))

[<Test>]
let ``can slice vector from specific index to specific index`` () =
   let vector =
      let engine = REngine.GetInstanceFromID (engineName)
      engine.Evaluate ("""0:5""") |> S.AsInteger
   let actual = vector.[2..4]
   Assert.That (actual, Is.TypeOf<IntegerVector> ())
   Assert.That (actual, Is.EquivalentTo ([2..4]))