using RDotNet.Internals;
using RDotNet.Utilities;
using System;
using System.Runtime.InteropServices;

namespace RDotNet
{
    /// <summary>
    /// An environment object.
    /// </summary>
    public class REnvironment : SymbolicExpression
    {
        /// <summary>
        /// Creates an environment object.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="pointer">The pointer to an environment.</param>
        protected internal REnvironment(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Creates a new environment object.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="parent">The parent environment.</param>
        public REnvironment(REngine engine, REnvironment parent)
            : base(engine, engine.GetFunction<Rf_NewEnvironment>()(engine.NilValue.DangerousGetHandle(), engine.NilValue.DangerousGetHandle(), parent.handle))
        { }

        /// <summary>
        /// Gets the parental environment.
        /// </summary>
        public REnvironment Parent
        {
            get
            {
                SEXPREC sexp = GetInternalStructure();
                IntPtr parent = sexp.envsxp.enclos;
                return Engine.EqualsRNilValue(parent) ? null : new REnvironment(Engine, parent);
            }
        }

        /// <summary>
        /// Gets a symbol defined in this environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The symbol.</returns>
        public SymbolicExpression GetSymbol(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }
            if (name == string.Empty)
            {
                throw new ArgumentException();
            }

            IntPtr installedName = this.GetFunction<Rf_install>()(name);
            IntPtr value = this.GetFunction<Rf_findVar>()(installedName, handle);
            if (Engine.CheckUnbound(value))
            {
                throw new EvaluationException(string.Format("Error: object '{0}' not found", name));
            }

            var sexp = (SEXPREC)Marshal.PtrToStructure(value, typeof(SEXPREC));
            if (sexp.sxpinfo.type == SymbolicExpressionType.Promise)
            {
                value = this.GetFunction<Rf_eval>()(value, handle);
            }
            return new SymbolicExpression(Engine, value);
        }

        /// <summary>
        /// Defines a symbol in this environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The symbol.</param>
        public void SetSymbol(string name, SymbolicExpression expression)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "'name' cannot be null");
            }
            if (name == string.Empty)
            {
                throw new ArgumentException("'name' cannot be an empty string");
            }
            if (expression == null)
            {
                expression = Engine.NilValue;
            }
            if (expression.Engine != this.Engine)
            {
                throw new ArgumentException();
            }
            IntPtr installedName = this.GetFunction<Rf_install>()(name);
            this.GetFunction<Rf_defineVar>()(installedName, expression.DangerousGetHandle(), handle);
        }

        /// <summary>
        /// Gets the symbol names defined in this environment.
        /// </summary>
        /// <param name="all">Including special functions or not.</param>
        /// <returns>Symbol names.</returns>
        public string[] GetSymbolNames(bool all = false)
        {
            var symbolNames = new CharacterVector(Engine, this.GetFunction<R_lsInternal>()(handle, all));
            int length = symbolNames.Length;
            var copy = new string[length];
            symbolNames.CopyTo(copy, length);
            return copy;
        }
    }
}