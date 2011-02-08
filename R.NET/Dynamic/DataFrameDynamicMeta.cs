using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RDotNet.Dynamic
{
	public class DataFrameDynamicMeta : SymbolicExpressionDynamicMeta
	{
		private static readonly Type[] IndexerNameType = new Type[] { typeof(string) };

		public DataFrameDynamicMeta(Expression parameter, DataFrame frame)
			: base(parameter, frame)
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

			Expression instance = Expression.Constant(Value, typeof(DataFrame));
			Expression name = Expression.Constant(binder.Name, typeof(string));
			PropertyInfo indexer = typeof(DataFrame).GetProperty("Item", IndexerNameType);
			Expression call = Expression.Property(instance, indexer, name);
			return new DynamicMetaObject(call, BindingRestrictions.GetTypeRestriction(call, typeof(DynamicVector)));
		}

		private string[] GetNames()
		{
			return ((DataFrame)Value).ColumnNames ?? Empty;
		}
	}
}
