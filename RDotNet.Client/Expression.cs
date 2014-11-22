using System;

namespace RDotNet.Client
{
   public class Expression : SymbolicExpression
   {
       public Expression(IRSafeHandle handle)
           : base(handle)
       { }
       
       public SymbolicExpression Evaluate(REnvironment environment)
       {
           if (environment == null) throw new ArgumentNullException("environment");

           var evaluated = environment.Evaluate(this);
           return evaluated;
       }

       public bool TryEvaluate(REnvironment environment, out SymbolicExpression evaluated)
       {
           if (environment == null) throw new ArgumentNullException("environment");

           var result = environment.TryEvaluate(this, out evaluated);
           return result;
       }

       public string ThrowWithLastError()
       {
           const string statement = "geterrmessage()\n";

           SymbolicExpression expression;
           bool result = InternalTryEvaluate(statement, out expression);
           if (!result) throw new EvaluationException("Could not retrieve last error message. The R Engine may be corrupted.");

           var msgs = expression.ToCharacterVector();
           if (msgs.Length > 1) throw new EvaluationException("Unexpected multiple error messages returned.");
           if (msgs.Length == 0) throw new EvaluationException("No error messages returned (zero length).");

           throw new EvaluationException(msgs[0]);
       }
   }
}
