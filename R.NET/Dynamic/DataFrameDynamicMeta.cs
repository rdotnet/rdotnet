using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace RDotNet.Dynamic
{
    /// <summary>
    /// Dynamic and binding logic for R data frames
    /// </summary>
    public class DataFrameDynamicMeta : SymbolicExpressionDynamicMeta
    {
        private static readonly Type[] IndexerNameType = new[] { typeof(string) };

        /// <summary>
        /// Creates a new object dealing with the dynamic and binding logic for R data frames
        /// </summary>
        /// <param name="parameter">The expression representing this new DataFrameDynamicMeta in the binding process</param>
        /// <param name="frame">The runtime value of the DataFrame, that this new DataFrameDynamicMeta represents</param>
        public DataFrameDynamicMeta(System.Linq.Expressions.Expression parameter, DataFrame frame)
            : base(parameter, frame)
        { }

        /// <summary>
        /// Returns the enumeration of all dynamic member names.
        /// </summary>
        /// <returns>The list of dynamic member names</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames().Concat(GetNames());
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
            if (!GetNames().Contains(binder.Name))
            {
                return base.BindGetMember(binder);
            }
            return BindGetMember<DataFrame, DynamicVector>(binder, IndexerNameType);
        }

        private string[] GetNames()
        {
            return ((DataFrame)Value).ColumnNames ?? Empty;
        }
    }
}