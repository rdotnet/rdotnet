using System;
using System.Linq;
using RDotNet.Internals;

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
		{}

		public override SymbolicExpression Invoke(SymbolicExpression[] args)
		{
			IntPtr argument = Engine.NilValue.DangerousGetHandle();
			foreach (SymbolicExpression arg in args.Reverse())
			{
				argument = Engine.GetFunction<Rf_cons>("Rf_cons")(arg.DangerousGetHandle(), argument);
			}
			IntPtr call = Engine.GetFunction<Rf_lcons>("Rf_lcons")(handle, argument);

			IntPtr result = Engine.GetFunction<Rf_eval>("Rf_eval")(call, Engine.GlobalEnvironment.DangerousGetHandle());
			return new SymbolicExpression(Engine, result);
		}
	}
}
