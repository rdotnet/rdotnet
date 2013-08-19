using System;
using System.Linq;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// A built-in function.
	/// </summary>
	public class BuiltinFunction : Function
	{
		/// <summary>
		/// Creates a built-in function proxy.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="pointer">The pointer.</param>
		protected internal BuiltinFunction(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{}

		public override SymbolicExpression Invoke(SymbolicExpression[] args)
		{
			IntPtr argument = Engine.NilValue.DangerousGetHandle();
			foreach (SymbolicExpression arg in args.Reverse())
			{
				argument = Engine.GetFunction<Rf_cons>()(arg.DangerousGetHandle(), argument);
			}
			IntPtr call = Engine.GetFunction<Rf_lcons>()(handle, argument);

			IntPtr result = Engine.GetFunction<Rf_eval>()(call, Engine.GlobalEnvironment.DangerousGetHandle());
			return new SymbolicExpression(Engine, result);
		}
	}
}
