using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RDotNet.Dynamic
{
   /// <summary>
   /// Dynamic and binding logic for R data frames
   /// </summary>
   public class DataFrameDynamicMeta : SymbolicExpressionDynamicMeta
   {
      private static readonly Type[] IndexerNameType = new[] { typeof(string) };

      public DataFrameDynamicMeta(System.Linq.Expressions.Expression parameter, DataFrame frame)
         : base(parameter, frame)
      { }

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

         ConstantExpression instance = System.Linq.Expressions.Expression.Constant(Value, typeof(DataFrame));
         ConstantExpression name = System.Linq.Expressions.Expression.Constant(binder.Name, typeof(string));
         PropertyInfo indexer = typeof(DataFrame).GetProperty("Item", IndexerNameType);
         IndexExpression call = System.Linq.Expressions.Expression.Property(instance, indexer, name);
         return new DynamicMetaObject(call, BindingRestrictions.GetTypeRestriction(call, typeof(DynamicVector)));
      }

      private string[] GetNames()
      {
         return ((DataFrame)Value).ColumnNames ?? Empty;
      }
   }
}