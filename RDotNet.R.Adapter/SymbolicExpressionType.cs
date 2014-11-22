using System;
using System.Runtime.Serialization;

namespace RDotNet.R.Adapter
{
    [DataContract]
    public enum SymbolicExpressionType
    {
        [EnumMember] Null = 0,
        [EnumMember] Symbol = 1,
        [EnumMember] Pairlist = 2,
        [EnumMember] Closure = 3,
        [EnumMember] Environment = 4,
        [EnumMember] Promise = 5,
        [EnumMember] LanguageObject = 6,
        [EnumMember] SpecialFunction = 7,
        [EnumMember] BuiltinFunction = 8,
        [EnumMember] InternalCharacterString = 9,
        [EnumMember] LogicalVector = 10,
        [EnumMember] IntegerVector = 13,
        [EnumMember] NumericVector = 14,
        [EnumMember] ComplexVector = 15,
        [EnumMember] CharacterVector = 16,
        [EnumMember] DotDotDotObject = 17,
        [EnumMember] Any = 18,
        [EnumMember] List = 19,
        [EnumMember] ExpressionVector = 20,
        [EnumMember] ByteCode = 21,
        [EnumMember] ExternalPointer = 22,
        [EnumMember] WeakReference = 23,
        [EnumMember] RawVector = 24,
        [EnumMember] S4 = 25,
        [EnumMember] [Obsolete("Use SEXPTYPE.Closure instead. But, note that value is different from SEXPTYPE.Closure.")] FUNSXP = 99,
    }
}
