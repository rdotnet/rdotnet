using NUnit.Framework;
using System;
using System.Linq;
using RDotNet.Internals;

namespace RDotNet
{
   public class RFunctionsTest : RDotNetTestFixture
   {
      [Test]
      public void TestBuiltinFunctions()
      {
         var engine = REngine.GetInstanceFromID(EngineName);

         // function (x)  .Primitive("abs")
         var abs = engine.GetSymbol("abs").AsFunction();
         Assert.AreEqual(SymbolicExpressionType.BuiltinFunction, abs.Type);

         var values = GenArrayDouble(-2, 2);

         var absValues = abs.Invoke(engine.CreateNumericVector(values)).AsNumeric().ToArray();

         Assert.AreEqual(values.Length, absValues.Length);
         CheckArrayEqual(new double[] { 2, 1, 0, 1, 2 }, absValues);

      }

      [Test]
      public void TestStatsFunctions()
      {
         var engine = REngine.GetInstanceFromID(EngineName);

         //> a <- dpois(0:7, lambda = 0.9)
         //> signif(a, 2)
         //[1] 4.1e-01 3.7e-01 1.6e-01 4.9e-02 1.1e-02 2.0e-03 3.0e-04 3.9e-05
         //> 
         var values = GenArrayDouble(0, 7);
         var dpois = engine.GetSymbol("dpois").AsFunction();


         //dpois(x, lambda, log = FALSE)
         // let's test how arguments with default values is treated.
         var x = engine.CreateNumericVector(values);
         var lambda = engine.CreateNumericVector(new[] { 0.9 });
         var log = engine.CreateLogicalVector(new[] { false });
         // just check that is passes without exceptions
         var distVal = dpois.Invoke(x, lambda, log);


         distVal = dpois.Invoke(x, lambda);
         var signif = engine.GetSymbol("signif").AsFunction();
         var signifValues = signif.Invoke(distVal, engine.CreateIntegerVector(new[] { 2 })).AsNumeric().ToArray();

         var expected = new[] { 4.1e-01, 3.7e-01, 1.6e-01, 4.9e-02, 1.1e-02, 2.0e-03, 3.0e-04, 3.9e-05 };
         CheckArrayEqual(expected, signifValues);

      }

      // https://rdotnet.codeplex.com/workitem/76
      [Test]
      public void TestGenericFunction()
      {
         var engine = REngine.GetInstanceFromID(EngineName);


         
         var funcDef=@"
printPairList <- function(...) {
  a <- list(...)
  namez <- names(a)
  r <- ''
  if(length(a)==0) return('empty pairlist')
  for(i in 1:length(a)) {
    name <- namez[i]
    r <- paste(r, paste0(name, '=', a[[i]], sep=';'))
  }
  substring(r, 1, (nchar(r)-1))
}
  

setGeneric( 'f', function(x, ...) {
	standardGeneric('f')
} )

setMethod( 'f', 'integer', function(x, ...) { paste( 'f.integer called:', printPairList(...) ) } )
setMethod( 'f', 'numeric', function(x, ...) { paste( 'f.numeric called:', printPairList(...) ) } )
";

         engine.Evaluate(funcDef);
         var f=engine.GetSymbol("f").AsFunction();


         // > f(1, b=2, c=3)
         // [1] "f.numeric called:  b=2; c=3"
         checkInvoke(f.InvokeNamed(tc("a", "1.0"), tc("b", "2"), tc("c", "3")), "f.numeric called:  b=2; c=3");
         // > f(1, b=2.1, c=3)
         // [1] "f.numeric called:  b=2.1; c=3"
         checkInvoke(f.InvokeNamed(tc("a", "1.0"), tc("b", "2.1"), tc("c", "3")), "f.numeric called:  b=2.1; c=3");
         // > f(1, c=3, b=2)
         // [1] "f.numeric called:  c=3; b=2"
         checkInvoke(f.InvokeNamed(tc("a", "1.0"), tc("c", "3"), tc("b", "2")), "f.numeric called:  c=3; b=2");
         // > f(1L, b=2, c=3)
         // [1] "f.integer called:  b=2; c=3"
         checkInvoke(f.InvokeNamed(tc("a", "1L"), tc("b", "2"), tc("c", "3")), "f.integer called:  b=2; c=3");
         // > f(1L, c=3, b=2)
         // [1] "f.integer called:  c=3; b=2"
         checkInvoke(f.InvokeNamed(tc("a", "1L"), tc("c", "3"), tc("b", "2")), "f.integer called:  c=3; b=2");
         
         // .NET Framework array to R vector.
         NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
         engine.SetSymbol("group1", group1);
         // Direct parsing from R script.
         NumericVector group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

         GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
         double p = testResult["p.value"].AsNumeric().First();
         Assert.AreEqual(0.09077332, Math.Round(p, 8));

         var s2 = engine.GetSymbol("t.test");
         var f2 = s2.AsFunction();
         GenericVector testResult2 = f2.Invoke(new[] { group1, group2 }).AsList();
         double p2 = testResult2["p.value"].AsNumeric().First();
         double p3 = testResult2[2].AsNumeric().First();
         Assert.AreEqual(0.09077332, p2);
      }

      [Test]
      public void TestArgumentMatching()
      {
         var engine = REngine.GetInstanceFromID(EngineName);

         var funcDef = @"function(a, b, cc=NULL, d='d', e=length(d), f=FALSE, g=123.4) {
  r <- paste0('a=', ifelse(missing(a),'missing_a',a))
  r <- paste(r, paste0('b=', ifelse(missing(b),'missing_b',b)), sep=';')
  r <- paste(r, paste0('cc=', ifelse(is.null(cc),'null_c',cc)), sep=';')
  r <- paste(r, paste0('d=', d), sep=';')
  r <- paste(r, paste0('e=', e), sep=';')
  r <- paste(r, paste0('f=', f), sep=';')
  r <- paste(r, paste0('g=', g), sep=';')
  r
}
";
         var f = engine.Evaluate(funcDef).AsFunction();

         //[1] "a=missing_a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.Invoke(), "a=missing_a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4");
         //> f('a')
         //[1] "a=a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.Invoke(CreateSexp("a")), "a=a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4");
         //> f(a='a')
         //[1] "a=a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("a", "a")), "a=a;b=missing_b;cc=null_c;d=d;e=1;f=FALSE;g=123.4");
         //> f(b='b')
         //[1] "a=missing_a;b=b;cc=null_c;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("b", "b")), "a=missing_a;b=b;cc=null_c;d=d;e=1;f=FALSE;g=123.4");
         //> f(b='b',cc='cc')
         //[1] "a=missing_a;b=b;cc=cc;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("b", "b"), tc("cc", "cc")), "a=missing_a;b=b;cc=cc;d=d;e=1;f=FALSE;g=123.4");
         //> f(cc='cc',b='b')
         //[1] "a=missing_a;b=b;cc=cc;d=d;e=1;f=FALSE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("cc", "cc"), tc("b", "b")), "a=missing_a;b=b;cc=cc;d=d;e=1;f=FALSE;g=123.4");
         //> f(d='ddd')
         //[1] "a=missing_a;b=missing_b;cc=null_c;d=ddd;e=1;f=FALSE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("d", "ddd")), "a=missing_a;b=missing_b;cc=null_c;d=ddd;e=1;f=FALSE;g=123.4");
         //> f(f=TRUE)
         //[1] "a=missing_a;b=missing_b;cc=null_c;d=d;e=1;f=TRUE;g=123.4"
         checkInvoke(f.InvokeNamed(tc("f", "TRUE")), "a=missing_a;b=missing_b;cc=null_c;d=d;e=1;f=TRUE;g=123.4");

      }

      private Tuple<string,SymbolicExpression> tc(string argname, object value)
      {
         return Tuple.Create(argname, CreateSexp(value));
      }

      private SymbolicExpression CreateSexp(object value)
      {
         var t = value.GetType();
         var engine = REngine.GetInstanceFromID(EngineName);
         if (t == typeof(double))
            return engine.CreateNumericVector((double)value);
         if (t == typeof(string))
            return engine.CreateCharacterVector((string)value);
         if (t == typeof(bool))
            return engine.CreateLogicalVector((bool)value);

         throw new NotSupportedException();
      }

      private void checkInvoke(SymbolicExpression result, string expected)
      {
         Assert.AreEqual(expected, result.AsCharacter().ToArray()[0]);
      }
   }
}