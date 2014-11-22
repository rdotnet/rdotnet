using System.Collections.Generic;
using RDotNet.Client.Diagnostics;
using System;
using System.Diagnostics;
using System.Linq;

namespace RDotNet.Client
{
    [DebuggerDisplay("Length = {Length}; Ordered = {IsOrdered}; RObjectType = {Type}")]
    [DebuggerTypeProxy(typeof (FactorDebugView))]
    public class Factor : IntegerVector
    {
        public const int NACode = int.MinValue;

        public Factor(IRSafeHandle sexp)
            : base(sexp)
        { }

        public IList<string> GetLevels()
        {
            var levels = GetFactorLevels();
            var attribute = GetAttribute(levels);
            var result =  attribute.ToCharacterVector().ToList();
            return result;
        }

        public IList<string> GetFactors()
        {
            var levels = GetLevels();

            return this.Select(value => (value == NACode ? null : levels[value - 1])).ToList();
        }

        public IList<TEnum> GetFactors<TEnum>(bool ignoreCase = false) where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) throw new ArgumentException("Only enum type is supported");

            var levels = GetLevels();
            return this.Select(value => levels[value - 1])
                .Select(value => (TEnum)Enum.Parse(enumType, value, ignoreCase))
                .ToList();
        }

        public bool IsOrdered()
        {
            return HasOrderedFactors();
        }

        public string GetFactor(int index)
        {
            var intValue = this[index];
            if (intValue <= 0)
                return null;
            
            var result = GetLevels()[intValue - 1]; // zero-based index in C#, but 1-based in R
            return result;
        }

        public void SetFactor(int index, string factor)
        {
            if (factor == null)
            {
                this[index] = NACode;
            }
            else
            {
                var levels = GetFactorLevels().ToCharacterVector().ToList();
                int factIndex = levels.IndexOf(factor);
                if (factIndex >= 0)
                    this[index] = factIndex + 1; // zero-based index in C#, but 1-based in R
                else
                    this[index] = NACode;
            }
        }
    }
}
