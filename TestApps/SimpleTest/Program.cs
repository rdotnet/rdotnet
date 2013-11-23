using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTest
{
   class Program
   {
      static void Main (string[] args)
      {
         var rargs = REngine.BuildRArgv (new StartupParameter());
         foreach (var item in rargs) {
            Console.Write ("\""+item +"\", "+ " ");
         }
         Console.WriteLine ();
         REngine.SetEnvironmentVariables ();
         using (REngine engine = REngine.CreateInstance("RDotNet")) {
            // From v1.5, REngine requires explicit initialization.
            // You can set some parameters.cx
            engine.Initialize ();

            for (int i = 0; i < 10; i++) {

               NumericVector blah = engine.CreateNumericVector (new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
               GC.Collect ();
               GC.WaitForPendingFinalizers ();
               Console.WriteLine ("before nullify"); 
               blah = null;
               GC.Collect ();
               GC.WaitForPendingFinalizers ();
               Console.WriteLine ("after nullify and gccollect"); 
            }

            // .NET Framework array to R vector.
            NumericVector group1 = engine.CreateNumericVector (new double[] { 30.02, 29.99, 30.11, 29.97, 30.01, 29.99 });
            engine.SetSymbol ("group1", group1);
            // Direct parsing from R script.
            NumericVector group2 = engine.Evaluate ("group2 <- c(29.89, 29.93, 29.72, 29.98, 30.02, 29.98)").AsNumeric ();

            // Test difference of mean and get the P-value.
            GenericVector testResult = engine.Evaluate ("t.test(group1, group2)").AsList ();
            double p = testResult ["p.value"].AsNumeric ().First ();

            Console.WriteLine ("Group1: [{0}]", string.Join (", ", group1));
            Console.WriteLine ("Group2: [{0}]", string.Join (", ", group2));
            Console.WriteLine ("P-value = {0:0.000}", p);


         }
      }
   }
}
