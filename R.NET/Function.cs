using System;

namespace RDotNet
{
	/// <summary>
	/// A function is one of closure, built-in function, or special function.
	/// </summary>
	public abstract class Function : SymbolicExpression
	{
		/// <summary>
		/// Creates a function object.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		protected Function(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{}

		/// <summary>
		/// Executes the function.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns>The return value.</returns>
		public abstract SymbolicExpression Invoke(SymbolicExpression[] args);
	}
}
