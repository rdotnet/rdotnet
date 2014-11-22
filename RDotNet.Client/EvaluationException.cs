using System;

namespace RDotNet.Client
{
   public class EvaluationException : Exception 
   {
      public EvaluationException(string message)
         : base(message)
      { }

      public EvaluationException(string message, Exception innerException)
         : base(message, innerException)
      { }
   }
}
