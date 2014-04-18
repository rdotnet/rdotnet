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
         REngine.SetEnvironmentVariables();
         using (REngine e = REngine.GetInstance())
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
            f.Invoke(e.CreateCharacterVector(new[] { "blah", "blah" }));
         }
      }
   }
}
