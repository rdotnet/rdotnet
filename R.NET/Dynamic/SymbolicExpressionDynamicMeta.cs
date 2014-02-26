using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RDotNet.Dynamic
{
   /// <summary>
   /// Dynamic and binding logic for S expressions
   /// </summary>
   public class SymbolicExpressionDynamicMeta : DynamicMetaObject
   {
      protected static readonly string[] Empty = new string[0];

      /// <summary>
      /// Dynamic and binding logic for S expressions
      /// </summary>
      /// <param name="parameter">The expression representing this new SymbolicExpressionDynamicMeta in the binding process</param>
      /// <param name="expression">The runtime value of this SymbolicExpression represented by this new SymbolicExpressionDynamicMeta</param>
      public SymbolicExpressionDynamicMeta(System.Linq.Expressions.Expression parameter, SymbolicExpression expression)
         : base(parameter, BindingRestrictions.Empty, expression)
      { }

      public override IEnumerable<string> GetDynamicMemberNames()
      {
         return base.GetDynamicMemberNames().Concat(GetAttributeNames());
      }

      public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
      {
         if (!GetAttributeNames().Contains(binder.Name))
         {
            return base.BindGetMember(binder);
         }

         ConstantExpression instance = System.Linq.Expressions.Expression.Constant(Value, typeof(SymbolicExpression));
         ConstantExpression name = System.Linq.Expressions.Expression.Constant(binder.Name, typeof(string));
         MethodInfo getAttribute = typeof(SymbolicExpression).GetMethod("GetAttribute");
         MethodCallExpression call = System.Linq.Expressions.Expression.Call(instance, getAttribute, name);
         return new DynamicMetaObject(call, BindingRestrictions.GetTypeRestriction(call, typeof(SymbolicExpression)));
      }

      private string[] GetAttributeNames()
      {
         return ((SymbolicExpression)Value).GetAttributeNames() ?? Empty;
      }
   }
}