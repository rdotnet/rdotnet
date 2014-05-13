using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDotNet
{
   /// <summary>
   /// Exception signaling that the R engine failed to evaluate a statement
   /// </summary>
   public class EvaluationException : Exception 
   {
      /// <summary>
      /// Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
      /// </summary>
      /// <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
      public EvaluationException(string errorMsg)
         : base(errorMsg)
      {
      }

      /// <summary>
      /// Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
      /// </summary>
      /// <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
      /// <param name="innerException">The exception that was caught and triggered the creation of this evaluation exception</param>
      public EvaluationException(string errorMsg, Exception innerException)
         : base(errorMsg, innerException)
      {
      }
   }
}
