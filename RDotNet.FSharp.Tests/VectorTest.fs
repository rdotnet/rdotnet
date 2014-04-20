namespace RDotNet

open NUnit.Framework

type VectorTest () =

   inherit RDotNetTestFixture ()

   [<Test>]
   member this.``can slice vector all`` () =
      let vector =
         let engine = this.Engine
         engine.Evaluate ("""0:5""") |> S.AsInteger
      let actual = vector.[*]
      Assert.That (actual, Is.TypeOf<IntegerVector> ())
      Assert.That (actual, Is.EquivalentTo ([0..5]))

   [<Test>]
   member this.``can slice vector from start to specific index`` () =
      let vector =
         let engine = this.Engine
         engine.Evaluate ("""0:5""") |> S.AsInteger
      let actual = vector.[..4]
      Assert.That (actual, Is.TypeOf<IntegerVector> ())
      Assert.That (actual, Is.EquivalentTo ([0..4]))

   [<Test>]
   member this.``can slice vector from specific index to end`` () =
      let vector =
         let engine = this.Engine
         engine.Evaluate ("""0:5""") |> S.AsInteger
      let actual = vector.[2..]
      Assert.That (actual, Is.TypeOf<IntegerVector> ())
      Assert.That (actual, Is.EquivalentTo ([2..5]))

   [<Test>]
   member this.``can slice vector from specific index to specific index`` () =
      let vector =
         let engine = this.Engine
         engine.Evaluate ("""0:5""") |> S.AsInteger
      let actual = vector.[2..4]
      Assert.That (actual, Is.TypeOf<IntegerVector> ())
      Assert.That (actual, Is.EquivalentTo ([2..4]))