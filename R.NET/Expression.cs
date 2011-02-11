using System;

namespace RDotNet
{
	/// <summary>
	/// An expression object.
	/// </summary>
	public class Expression : SymbolicExpression
	{
		/// <summary>
		/// Creates an expression object.
		/// </summary>
		/// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
		/// <param name="pointer">The pointer to an expression.</param>
		internal protected Expression(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		/// <summary>
		/// Evaluates the expression in the specified environment.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <returns>The evaluation result.</returns>
		public SymbolicExpression Evaluate(RDotNet.Environment environment)
		{
			if (environment == null)
			{
				throw new ArgumentNullException("environment");
			}
			if (this.Engine != environment.Engine)
			{
				throw new ArgumentException(null, "environment");
			}

			return new SymbolicExpression(Engine, Engine.Proxy.Rf_eval(this.handle, environment.DangerousGetHandle()));
		}

		/// <summary>
		/// Evaluates the expression in the specified environment.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="result">The evaluation result, or <c>null</c> if the evaluation failed</param>
		/// <returns><c>True</c> if the evaluation succeeded.</returns>
		public bool TryEvaluate(RDotNet.Environment environment, out SymbolicExpression result)
		{
			if (environment == null)
			{
				throw new ArgumentNullException("environment");
			}
			if (this.Engine != environment.Engine)
			{
				throw new ArgumentException(null, "environment");
			}

			bool errorOccurred;
			IntPtr pointer = Engine.Proxy.R_tryEval(this.handle, environment.DangerousGetHandle(), out errorOccurred);
			result = errorOccurred ? null : new SymbolicExpression(Engine, pointer);
			return !errorOccurred;
		}
	}
}
