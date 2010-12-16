using System;

namespace RDotNet.Internals
{
	/// <summary>
	/// SEXPTYPE enumeration.
	/// </summary>
	public enum SymbolicExpressionType
	{
		Null = 0,
		Symbol = 1,
		Pairlist = 2,
		Closure = 3,
		Environment = 4,
		Promise = 5,
		LanguageObject = 6,
		SpecialFunction = 7,
		BuiltinFunction = 8,
		InternalCharacterString = 9,
		LogicalVector = 10,
		IntegerVector = 13,
		NumericVector = 14,
		ComplexVector = 15,
		CharacterVector = 16,
		DotDotDotObject = 17,
		Any = 18,
		List = 19,
		ExpressionVector = 20,
		ByteCode = 21,
		ExternalPointer = 22,
		WeakReference = 23,
		RawVector = 24,
		S4 = 25,

		#region Original Definitions
		[Obsolete("Use SEXPTYPE.Null instead.")]
		NILSXP = Null,
		[Obsolete("Use SEXPTYPE.Symbol istead.")]
		SYMSXP = Symbol,
		[Obsolete("Use SEXPTYPE.Pairlist istead.")]
		LISTSXP = Pairlist,
		[Obsolete("Use SEXPTYPE.Closure instead.")]
		CLOSXP = Closure,
		[Obsolete("Use SEXPTYPE.Environment instead.")]
		ENVSXP = Environment,
		[Obsolete("Use SEXPTYPE.Promise instead.")]
		PROMSXP = Promise,
		[Obsolete("Use SEXPTYPE.LanguageObject instead.")]
		LANGSXP = LanguageObject,
		[Obsolete("Use SEXPTYPE.SpecialFunction instead.")]
		SPECIALSXP = SpecialFunction,
		[Obsolete("Use SEXPTYPE.BuiltinFunction instead.")]
		BUILTINSXP = BuiltinFunction,
		[Obsolete("Use SEXPTYPE.InternalCharacterString instead.")]
		CHARSXP = InternalCharacterString,
		[Obsolete("Use SEXPTYPE.LogicalVector instead.")]
		LGLSXP = LogicalVector,
		[Obsolete("Use SEXPTYPE.IntegerVector instead.")]
		INTSXP = IntegerVector,
		[Obsolete("Use SEXPTYPE.NumericVector instead.")]
		REALSXP = NumericVector,
		[Obsolete("Use SEXPTYPE.ComplexVector instead.")]
		CPLXSXP = ComplexVector,
		[Obsolete("Use SEXPTYPE.CharacterVector instead.")]
		STRSXP = CharacterVector,
		[Obsolete("Use SEXPTYPE.DotDotDotObject instead.")]
		DOTSXP = DotDotDotObject,
		[Obsolete("Use SEXPTYPE.Any instead.")]
		ANYSXP = Any,
		[Obsolete("Use SEXPTYPE.List instead.")]
		VECSXP = List,
		[Obsolete("Use SEXPTYPE.ExpressionVector instead.")]
		EXPRSXP = ExpressionVector,
		[Obsolete("Use SEXPTYPE.ByteCode instead.")]
		BCODESXP = ByteCode,
		[Obsolete("Use SEXPTYPE.ExternalPointer instead.")]
		EXTPTRSXP = ExternalPointer,
		[Obsolete("Use SEXPTYPE.WeakReference instead.")]
		WEAKREFSXP = WeakReference,
		[Obsolete("Use SEXPTYPE.RawVector instead.")]
		RAWSXP = RawVector,
		[Obsolete("Use SEXPTYPE.S4 instead.")]
		S4SXP = S4,
		[Obsolete("Use SEXPTYPE.Closure instead. But, note that value is different from SEXPTYPE.Closure.")]
		FUNSXP = 99,
		#endregion
	}
}
