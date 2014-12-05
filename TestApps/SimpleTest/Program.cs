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
      static void Main(string[] args)
      {
         string rHome = null;
         string rPath = null;
         if (args.Length > 0)
            rPath = args[0];
         if (args.Length > 1)
            rHome = args[1];

         REngine.SetEnvironmentVariables(rPath: rPath, rHome:rHome);
         REngine e = REngine.GetInstance();

         System.Console.WriteLine(RDotNet.NativeLibrary.NativeUtility.SetEnvironmentVariablesLog);

         e.Dispose();
      }

      private static void TestPlot(REngine e)
      {
         e.Evaluate("library(DAAG)");
         for (int i = 0; i < 100; i++)
         {
            e.Evaluate("p <- plot(northing ~ easting, data=frogs, pch=c(1,16)[frogs$pres.abs+1], xlab='Meters east of reference point', ylab='Meters north')");
            e.Evaluate(string.Format("print(paste('plot iteration number', {0}))", i));
         }
      }

      private static void TestPendingFinalizersThreadingIssues(REngine e)
      {
         e.Evaluate("f <- function(a) {if (length(a)!= 1) stop('What goes on?')}");
         var f = e.Evaluate("f").AsFunction();
         try
         {
            e.Evaluate("f(letters[1:3])");
         }
         catch (EvaluationException)
         {
         }
         f.Invoke(e.CreateCharacterVector(new[] { "blah" }));
         try
         {
            f.Invoke(e.CreateCharacterVector(new[] { "blah", "blah" }));
         }
         catch (EvaluationException)
         {
            Console.WriteLine("Caught the expected exception");
         }
         f = null;
         GC.Collect();
         GC.WaitForPendingFinalizers();
         e.Dispose();
         Console.WriteLine("Just waiting for crash...");
         GC.Collect();
         GC.WaitForPendingFinalizers();
      }
   }
}
