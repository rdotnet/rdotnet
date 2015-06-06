using RDotNet.Diagnostics;
using RDotNet.Internals;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;

namespace RDotNet
{
    /// <summary>
    /// Represents factors.
    /// </summary>
    [DebuggerDisplay("Length = {Length}; Ordered = {IsOrdered}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof(FactorDebugView))]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class Factor : IntegerVector
    {
        /// <summary>
        /// Creates a new instance for a factor vector.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="coerced">The pointer to a factor vector.</param>
        protected internal Factor(REngine engine, IntPtr coerced)
            : base(engine, coerced)
        { }

        /// <summary>
        /// Gets the levels of this factor.
        /// </summary>
        /// <returns>Levels of this factor</returns>
        public string[] GetLevels()
        {
            return GetAttribute(Engine.GetPredefinedSymbol("R_LevelsSymbol")).AsCharacter().ToArray();
        }

        /// <summary>
        /// Gets the values in this factor.
        /// </summary>
        /// <returns>Values of this factor</returns>
        public string[] GetFactors()
        {
            var levels = GetLevels();
            var levelIndices = this.GetArrayFast();
            return Array.ConvertAll(levelIndices, value => (value == NACode ? null : levels[value - 1]));
        }

        /// <summary>
        /// Gets the levels of the factor as the specific enum type.
        /// </summary>
        /// <remarks>
        /// Be careful to the underlying values.
        /// You had better set <c>levels</c> and <c>labels</c> argument explicitly.
        /// </remarks>
        /// <example>
        /// <code>
        /// public enum Group
        /// {
        ///    Treatment = 1,
        ///    Control = 2
        /// }
        ///
        /// // You must set 'levels' and 'labels' arguments explicitly in this case
        /// // because levels of factor is sorted by default and the names in R and in enum names are different.
        /// var code = @"factor(
        ///    c(rep('T', 5), rep('C', 5), rep('T', 4), rep('C', 5)),
        ///    levels=c('T', 'C'),
        ///    labels=c('Treatment', 'Control')
        /// )";
        /// var factor = engine.Evaluate(code).AsFactor();
        /// foreach (Group g in factor.GetFactors&lt;Group&gt;())
        /// {
        ///    Console.Write("{0} ", g);
        /// }
        /// </code>
        /// </example>
        /// <typeparam name="TEnum">The type of enum.</typeparam>
        /// <param name="ignoreCase">The value indicating case-sensitivity.</param>
        /// <returns>Factors.</returns>
        public TEnum[] GetFactors<TEnum>(bool ignoreCase = false)
           where TEnum : struct
        {
            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Only enum type is supported");
            }
            // The exact underlying type of factor is Int32.
            // But probably other types are available.
            //if (Enum.GetUnderlyingType(enumType) != typeof(Int32))
            //{
            //   throw new ArgumentException("Only Int32 is supported");
            //}
            var levels = GetLevels();
            return this.Select(value => levels[value - 1])
               .Select(value => (TEnum)Enum.Parse(enumType, value, ignoreCase))
               .ToArray();
        }

        /// <summary>
        /// Gets the value which indicating the factor is ordered or not.
        /// </summary>
        public bool IsOrdered
        {
            get
            {
                return this.GetFunction<Rf_isOrdered>()(this.handle);
            }
        }

        /// <summary>
        /// Gets the value of the vector of factors at an index
        /// </summary>
        /// <param name="index">the zero-based index of the vector</param>
        /// <returns>The string representation of the factor, or a null reference if the value in R is NA</returns>
        public string GetFactor(int index)
        {
            var intValue = this[index];
            if (intValue <= 0)
                return null;
            else
                return this.GetLevels()[intValue - 1]; // zero-based index in C#, but 1-based in R
        }

        /// <summary>
        /// Sets the value of a factor vector at an index
        /// </summary>
        /// <param name="index">the zero-based index item to set in the vector</param>
        /// <param name="factorValue">The value of the factor - can be a null reference</param>
        public void SetFactor(int index, string factorValue)
        {
            if (factorValue == null)
                this[index] = NACode;
            else
            {
                var levels = this.GetLevels();
                int factIndex = Array.IndexOf(levels, factorValue);
                if (factIndex >= 0)
                    this[index] = factIndex + 1; // zero-based index in C#, but 1-based in R
                else
                    this[index] = NACode;
            }
        }
    }
}