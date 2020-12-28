using System;
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
        /// <summary>
        /// A string array of length zero
        /// </summary>
        protected static readonly string[] Empty = new string[0];

        /// <summary>
        /// Dynamic and binding logic for S expressions
        /// </summary>
        /// <param name="parameter">The expression representing this new SymbolicExpressionDynamicMeta in the binding process</param>
        /// <param name="expression">The runtime value of this SymbolicExpression represented by this new SymbolicExpressionDynamicMeta</param>
        public SymbolicExpressionDynamicMeta(System.Linq.Expressions.Expression parameter, SymbolicExpression expression)
            : base(parameter, BindingRestrictions.Empty, expression)
        { }

        /// <summary>
        /// Creates the binding of the dynamic get member operation.
        /// </summary>
        /// <typeparam name="RType">The type of R object that this dynamic meta object represents</typeparam>
        /// <typeparam name="BType">The type passed to define the binding restrictions</typeparam>
        /// <param name="binder">The binder; its name must be one of the names of the R object represented by this meta object</param>
        /// <param name="indexerNameType"></param>
        /// <returns></returns>
        protected DynamicMetaObject BindGetMember<RType, BType>(GetMemberBinder binder, Type[] indexerNameType)
        {
            ConstantExpression instance;
            ConstantExpression name;
            BuildInstanceAndName<RType>(binder, out instance, out name);
            PropertyInfo indexer = typeof(RType).GetProperty("Item", indexerNameType);
            IndexExpression call = System.Linq.Expressions.Expression.Property(instance, indexer, name);
            return CreateDynamicMetaObject<BType>(call);
        }

        private static DynamicMetaObject CreateDynamicMetaObject<BType>(System.Linq.Expressions.Expression call)
        {
            return new DynamicMetaObject(call, BindingRestrictions.GetTypeRestriction(call, typeof(BType)));
        }

        private void BuildInstanceAndName<RType>(GetMemberBinder binder, out ConstantExpression instance, out ConstantExpression name)
        {
            instance = System.Linq.Expressions.Expression.Constant(Value, typeof(RType));
            name = System.Linq.Expressions.Expression.Constant(binder.Name, typeof(string));
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>The list of dynamic member names</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames().Concat(GetAttributeNames());
        }

        /// <summary>
        /// Performs the binding of the dynamic get member operation.
        /// </summary>
        /// <param name="binder">
        /// An instance of the System.Dynamic.GetMemberBinder that represents the details of the dynamic operation.
        /// </param>
        /// <returns>The new System.Dynamic.DynamicMetaObject representing the result of the binding.</returns>
        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (!GetAttributeNames().Contains(binder.Name))
            {
                return base.BindGetMember(binder);
            }

            ConstantExpression instance;
            ConstantExpression name;
            BuildInstanceAndName<SymbolicExpression>(binder, out instance, out name);
            MethodInfo getAttribute = typeof(SymbolicExpression).GetMethod("GetAttribute");
            MethodCallExpression call = System.Linq.Expressions.Expression.Call(instance, getAttribute, name);
            return CreateDynamicMetaObject<SymbolicExpression>(call);
        }

        private string[] GetAttributeNames()
        {
            return ((SymbolicExpression)Value).GetAttributeNames() ?? Empty;
        }
    }
}