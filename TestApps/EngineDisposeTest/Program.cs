using RDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineDisposeTest
{
   class Program
   {
      /// <summary>
      /// Test that the disposal of the REngine and program exit does not lead to an exception. 
      /// Primarily for testing an issue with Debian: https://rdotnet.codeplex.com/workitem/60
      /// </summary>
      static void Main(string[] args)
      {
         bool keepSexpRefAfterDispose = false;
         if (args.Length > 0)
            keepSexpRefAfterDispose = bool.Parse(args[0]);

         REngine.SetEnvironmentVariables();
         REngine e = REngine.GetInstance();
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
         if (!keepSexpRefAfterDispose)
         {
            Console.WriteLine("Symbolic expression set to null before engine Dispose");
            f = null;
         }
         GC.Collect();
         GC.WaitForPendingFinalizers();
         e.Dispose();
         Console.WriteLine("Engine has been disposed of");
         if (keepSexpRefAfterDispose)
         {
            Console.WriteLine("Symbolic expression set to null after engine Dispose");
            f = null;
         }
         Console.WriteLine("Now performing final CLR GC collect");
         GC.Collect();
         GC.WaitForPendingFinalizers();
      }
   }
}
