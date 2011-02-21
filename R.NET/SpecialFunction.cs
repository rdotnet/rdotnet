using System;
using System.Linq;

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
		internal protected SpecialFunction(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		public override SymbolicExpression Invoke(SymbolicExpression[] args)
		{
			IntPtr argument = Engine.NilValue.DangerousGetHandle();
			foreach (var arg in args.Reverse())
			{
				argument = Engine.Proxy.Rf_cons(arg.DangerousGetHandle(), argument);
			}
			IntPtr call = Engine.Proxy.Rf_lcons(this.handle, argument);

			IntPtr result = Engine.Proxy.Rf_eval(call, Engine.GlobalEnvironment.DangerousGetHandle());
			return new SymbolicExpression(Engine, result);
		}
	}
}
