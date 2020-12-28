using System;
using System.Collections.Generic;

namespace RDotNet
{
    /// <summary>
    /// A special function.
    /// </summary>
    public class SpecialFunction : Function
    {
        /// <summary>
        /// Creates a special function proxy.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="pointer">The pointer.</param>
        protected internal SpecialFunction(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Invoke this special function, using an ordered list of unnamed arguments.
        /// </summary>
        /// <param name="args">The arguments of the function</param>
        /// <returns>The result of the evaluation</returns>
        public override SymbolicExpression Invoke(params SymbolicExpression[] args)
        {
            return InvokeOrderedArguments(args);
        }

        // Invoke this special function, using a list of named arguments in the form of a dictionary
        /// <summary>
        /// NotSupportedException
        /// </summary>
        /// <param name="args">key-value pairs</param>
        /// <returns>Always throws an exception</returns>
        public override SymbolicExpression Invoke(IDictionary<string, SymbolicExpression> args)
        {
            throw new NotSupportedException();
        }
    }
}