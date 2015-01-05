using RDotNet.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RDotNet
{
    /// <summary>
    /// A closure.
    /// </summary>
    public class Closure : Function
    {
        /// <summary>
        /// Creates a closure object.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="pointer">The pointer.</param>
        protected internal Closure(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Gets the arguments list.
        /// </summary>
        public Pairlist Arguments
        {
            get
            {
                SEXPREC sexp = GetInternalStructure();
                return new Pairlist(Engine, sexp.closxp.formals);
            }
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public Language Body
        {
            get
            {
                SEXPREC sexp = GetInternalStructure();
                return new Language(Engine, sexp.closxp.body);
            }
        }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        public REnvironment Environment
        {
            get
            {
                SEXPREC sexp = GetInternalStructure();
                return new REnvironment(Engine, sexp.closxp.env);
            }
        }

        /// <summary>
        /// Invoke this function, using an ordered list of unnamed arguments.
        /// </summary>
        /// <param name="args">The arguments of the function</param>
        /// <returns>The result of the evaluation</returns>
        public override SymbolicExpression Invoke(params SymbolicExpression[] args)
        {
            //int count = Arguments.Count;
            //if (args.Length > count)
            //   throw new ArgumentException("Too many arguments provided for this function", "args");
            return InvokeOrderedArguments(args);
        }

        /// <summary>
        /// Invoke this function, using named arguments provided as key-value pairs
        /// </summary>
        /// <param name="args">the representation of named arguments, as a dictionary</param>
        /// <returns>The result of the evaluation</returns>
        public override SymbolicExpression Invoke(IDictionary<string, SymbolicExpression> args)
        {
            var a = args.ToArray();
            return InvokeViaPairlist(Array.ConvertAll(a, x => x.Key), Array.ConvertAll(a, x => x.Value));
        }

        private string[] GetArgumentNames()
        {
            return Arguments.Select(arg => arg.PrintName).ToArray();
        }
    }
}