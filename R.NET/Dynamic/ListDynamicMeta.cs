using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RDotNet.Dynamic
{
	public class ListDynamicMeta : SymbolicExpressionDynamicMeta
	{
		private static readonly Type[] IndexerNameType = new Type[] { typeof(string) };

		public ListDynamicMeta(System.Linq.Expressions.Expression parameter, GenericVector list)
			: base(parameter, list)
		{
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return base.GetDynamicMemberNames().Concat(GetNames());
		}

		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			if (!GetNames().Contains(binder.Name))
			{
				return base.BindGetMember(binder);
			}

			var instance = System.Linq.Expressions.Expression.Constant(Value, typeof(GenericVector));
			var name = System.Linq.Expressions.Expression.Constant(binder.Name, typeof(string));
			PropertyInfo indexer = typeof(GenericVector).GetProperty("Item", IndexerNameType);
			var call = System.Linq.Expressions.Expression.Property(instance, indexer, name);
			return new DynamicMetaObject(call, BindingRestrictions.GetTypeRestriction(call, typeof(SymbolicExpression)));
		}

		private string[] GetNames()
		{
			return ((GenericVector)Value).Names ?? Empty;
		}
	}
}
