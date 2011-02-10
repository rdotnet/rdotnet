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
			return new SymbolicExpression(Engine, Engine.Proxy.Rf_eval((IntPtr)this, (IntPtr)environment));
		}

		/// <summary>
		/// Evaluates the expression in the specified environment.
		/// </summary>
		/// <param name="environment">The environment.</param>
		/// <param name="result">The evaluation result, or <c>null</c> if the evaluation failed</param>
		/// <returns><c>True</c> if the evaluation succeeded.</returns>
		public bool TryEvaluate(RDotNet.Environment environment, out SymbolicExpression result)
		{
			bool errorOccurred;
			IntPtr pointer = Engine.Proxy.R_tryEval((IntPtr)this, (IntPtr)environment, out errorOccurred);
			if (errorOccurred)
			{
				result = null;
				return false;
			}
			result = new SymbolicExpression(Engine, pointer);
			return true;
		}
	}
}
