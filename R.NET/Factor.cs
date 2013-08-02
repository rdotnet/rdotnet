using RDotNet.Diagnostics;
using RDotNet.Internals;
using System;
using System.Collections.Generic;
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
      {}

      /// <summary>
      /// Gets the levels of the factor.
      /// </summary>
      public string[] Levels
      {
         get
         {
            return GetAttribute(Engine.GetPredefinedSymbol("R_LevelsSymbol")).AsCharacter().ToArray();
         }
      }

      /// <summary>
      /// Gets the levels of the factor.
      /// </summary>
      /// <returns>Factors.</returns>
      public IEnumerable<string> GetFactors()
      {
         var levels = Levels;
         return this.Select(value => levels[value - 1]);
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
      public IEnumerable<TEnum> GetFactors<TEnum>(bool ignoreCase = false)
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
         return GetFactors().Select(value => (TEnum)Enum.Parse(enumType, value, ignoreCase));
      }

      /// <summary>
      /// Gets the value which indicating the factor is ordered or not.
      /// </summary>
      public bool IsOrdered
      {
         get
         {
            return Engine.GetFunction<Rf_isOrdered>()(this.handle);
         }
      }
   }
}
