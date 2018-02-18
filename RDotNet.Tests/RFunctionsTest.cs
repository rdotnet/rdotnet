using Xunit;
using RDotNet.Internals;
using System;
using System.Linq;

namespace RDotNet
{
    public class RFunctionsTest : RDotNetTestFixture
    {
        [Fact]
        public void TestBuiltinFunctions()
        {
            SetUpTest();
            var engine = this.Engine;

            // function (x)  .Primitive("abs")
            var abs = engine.GetSymbol("abs").AsFunction();
            Assert.Equal(SymbolicExpressionType.BuiltinFunction, abs.Type);

            var values = GenArrayDouble(-2, 2);

            var absValues = abs.Invoke(engine.CreateNumericVector(values)).AsNumeric().ToArray();

            Assert.Equal(values.Length, absValues.Length);
            CheckArrayEqual(new double[] { 2, 1, 0, 1, 2 }, absValues);
        }

        [Fact]
        public void TestStatsFunctions()
        {
            SetUpTest();
            var engine = this.Engine;

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

        private static string defPrintPairlist = @"
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

";

        [Fact]
        public void TestSpecialFunctions()
        {
            SetUpTest();
            var e = Engine;
            var plus = e.Evaluate("`if`").AsFunction();
            // tisoneCall <- call("if", quote(a==1), "this is one", "this is not one")
            e.Evaluate("a <- 1");
            Assert.Equal("this is one", plus.InvokeStrArgs("quote(a==1)", "'this is one'", "'this is not one'").AsCharacter().ToArray()[0]);
            e.Evaluate("a <- 2");
            Assert.Equal("this is not one", plus.InvokeStrArgs("quote(a==1)", "'this is one'", "'this is not one'").AsCharacter().ToArray()[0]);
        }

        // https://rdotnet.codeplex.com/workitem/76
        [Fact]
        public void TestGenericFunction()
        {
            SetUpTest();
            var engine = this.Engine;

            var funcDef = @"
setGeneric( 'f', function(x, ...) {
	standardGeneric('f')
} )

setMethod( 'f', 'integer', function(x, ...) { paste( 'f.integer called:', printPairList(...) ) } )
setMethod( 'f', 'numeric', function(x, ...) { paste( 'f.numeric called:', printPairList(...) ) } )
";

            engine.Evaluate(defPrintPairlist);
            engine.Evaluate(funcDef);
            var f = engine.GetSymbol("f").AsFunction();

            // > f(1, b=2, c=3)
            // [1] "f.numeric called:  b=2; c=3"
            checkInvoke(f.InvokeNamed(tc("x", 1.0), tc("b", "2"), tc("c", "3")), "f.numeric called:  b=2; c=3");
            // > f(1, b=2.1, c=3)
            // [1] "f.numeric called:  b=2.1; c=3"
            checkInvoke(f.InvokeNamed(tc("x", 1.0), tc("b", "2.1"), tc("c", "3")), "f.numeric called:  b=2.1; c=3");
            // > f(1, c=3, b=2)
            // [1] "f.numeric called:  c=3; b=2"
            checkInvoke(f.InvokeNamed(tc("x", 1.0), tc("c", "3"), tc("b", "2")), "f.numeric called:  c=3; b=2");
            // > f(1L, b=2, c=3)
            // [1] "f.integer called:  b=2; c=3"
            checkInvoke(f.InvokeNamed(tc("x", 1), tc("b", "2"), tc("c", "3")), "f.integer called:  b=2; c=3");
            // > f(1L, c=3, b=2)
            // [1] "f.integer called:  c=3; b=2"
            checkInvoke(f.InvokeNamed(tc("x", 1), tc("c", "3"), tc("b", "2")), "f.integer called:  c=3; b=2");

            // .NET Framework array to R vector.
            NumericVector group1 = engine.CreateNumericVector(new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            engine.SetSymbol("group1", group1);
            // Direct parsing from R script.
            NumericVector group2 = engine.Evaluate("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric();

            GenericVector testResult = engine.Evaluate("t.test(group1, group2)").AsList();
            double p = testResult["p.value"].AsNumeric().First();
            Assert.Equal(0.09077332, Math.Round(p, 8));

            var studentTest = engine.Evaluate("t.test").AsFunction();
            GenericVector testResult2 = studentTest.Invoke(new[] { group1, group2 }).AsList();
            double p2 = testResult2["p.value"].AsNumeric().First();
            double p3 = testResult2[2].AsNumeric().First();
            Assert.Equal(0.09077332, Math.Round(p2, 8));

            var sexp = studentTest.Invoke(engine.Evaluate("1:10"), engine.Evaluate("7:20"));
            // > format((t.test(1:10, y = c(7:20)) )$p.value, digits=12)
            // [1] "1.85528183251e-05"
            Assert.True(Math.Abs( 1.85528183251e-05 - sexp.AsList()["p.value"].AsNumeric()[0]) < 1e-12);
        }

        [Fact]
        public void TestDataFrameReturned()
        {
            SetUpTest();
            // Investigates http://stackoverflow.com/questions/26803752/r-net-invoke-function-does-not-work/26950937#26950937
            // Consider removing or morphing into another test on data coercion; there did not appear to be any issue as reported.

            var engine = this.Engine;

            var funcDef = @"function() {return(data.frame(a=1:4, b=5:8))}";
            var f = engine.Evaluate(funcDef).AsFunction();
            var x = f.Invoke();
            Assert.True(x.IsDataFrame());
            Assert.True(x.IsList());
            var df = x.AsDataFrame();
            Assert.NotNull(df);
            // above passes.
            // Now try to get closer to user's report test case with a character passed in
            funcDef = @"function(lyrics) {return(data.frame(a=1:4, b=5:8))}";
            f = engine.Evaluate(funcDef).AsFunction();
            x = f.Invoke(engine.CreateCharacter("Wo willst du hin?"));
            Assert.True(x.IsDataFrame());
            Assert.True(x.IsList());
            df = x.AsDataFrame();
            Assert.NotNull(df);

            funcDef = @"function() {return(as.matrix(data.frame(a=1:4, b=5:8)))}";
            f = engine.Evaluate(funcDef).AsFunction();
            x = f.Invoke();
            Assert.False(x.IsDataFrame());
            Assert.False(x.IsList());
            df = x.AsDataFrame();
            Assert.Null(df);
            var nm = x.AsNumericMatrix();
            Assert.NotNull(nm);
        }

        [Fact]
        public void TestArgumentMatching()
        {
            SetUpTest();
            var engine = this.Engine;

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
            //> f(g=456,'a',f=TRUE,'b','cc',e=11)
            //[1] "a=a;b=b;cc=cc;d=d;e=11;f=TRUE;g=456"
            checkInvoke(f.InvokeNamed(tc("g", 456), tc("", "a"), tc("f", true), tc(null, "b"), tc("", "cc"), tc("e", 11)), "a=a;b=b;cc=cc;d=d;e=11;f=TRUE;g=456");
        }

        [Fact]
        public void TestDotPairArguments()
        {
            SetUpTest();
            var e = Engine;
            // What if one of several 'formals' is a dotted pairlist
            var funcDef = @"
g <- function(x, ..., y) {
   paste0( 'x=', ifelse(missing(x), 'missing_x', x),  '; y=', ifelse(missing(y), 'missing_y', y), '; ', printPairList(...) )
}
";

            e.Evaluate(defPrintPairlist);
            e.Evaluate(funcDef);
            var g = e.GetSymbol("g").AsFunction();

            var exp = "x=missing_x; y=missing_y; empty pairlist";
            checkInvoke(g.Invoke(), exp);

            // > g(1)
            // [1] "x=1; y=missing_y; empty pairlist"
            exp = "x=1; y=missing_y; empty pairlist";
            checkInvoke(g.InvokeNamed(tc("x", "1")), exp);
            checkInvoke(g.InvokeStrArgs("1"), exp);
            //> g(1,'b')
            //[1] "x=1; y=missing_y;  =b"
            exp = "x=1; y=missing_y;  =b";
            checkInvoke(g.InvokeStrArgs("1", "'b'"), exp);
            //> g(1,y='b')
            //[1] "x=1; y=b; empty pairlist"
            //> g(1,c='c',y='b')
            //[1] "x=1; y=b;  c=c"
            exp = "x=1; y=b;  c=c";
            checkInvoke(g.InvokeNamed(tc("x", 1), tc("y", "b"), tc("c", "c")), exp);
            //> g(1,y='b',c='c')
            //[1] "x=1; y=b;  c=c"
            //> g(d=1,y='b',c='c')
            //[1] "x=missing_x; y=b;  d=1; c=c"
            //>
            //> g(1,2,3,y=4,5,6,7)
            //[1] "x=1; y=4;  =2; =3; =5; =6; =7"
            exp = "x=1; y=4;  =2; =3; =5; =6; =7";
            checkInvoke(g.InvokeNamed(tc("", "1"), tc("", "2"), tc("", "3"), tc("y", "4"), tc("", "5"), tc("", "6"), tc("", "7")), exp);
        }

        private Tuple<string, SymbolicExpression> tc(string argname, object value)
        {
            return Tuple.Create(argname, CreateSexp(value));
        }

        private SymbolicExpression CreateSexp(object value)
        {
            var t = value.GetType();
            var engine = this.Engine;
            if (t == typeof(int))
                return engine.CreateInteger((int)value);
            if (t == typeof(double))
                return engine.CreateNumeric((double)value);
            if (t == typeof(string))
                return engine.CreateCharacter((string)value);
            if (t == typeof(bool))
                return engine.CreateLogical((bool)value);
            if (t == typeof(byte))
                return engine.CreateRaw((byte)value);

            throw new NotSupportedException();
        }

        private void checkInvoke(SymbolicExpression result, string expected)
        {
            Assert.Equal(expected, result.AsCharacter().ToArray()[0]);
        }
    }
}