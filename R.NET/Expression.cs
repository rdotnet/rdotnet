using System;

namespace RDotNet
{
	public class Expression : SymbolicExpression
	{
		internal Expression(REngine engine, IntPtr pointer)
			: base(engine, pointer)
		{
		}

		public SymbolicExpression Evaluate(RDotNet.Environment environment)
		{
			return new SymbolicExpression(Engine, Engine.Proxy.Rf_eval((IntPtr)this, (IntPtr)environment));
		}
	}
}
