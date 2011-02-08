using System;

namespace RDotNet.Internals
{
	/// <summary>
	/// SEXPTYPE enumeration.
	/// </summary>
	public enum SymbolicExpressionType
	{
		/// <summary>
		/// Null.
		/// </summary>
		Null = 0,
		/// <summary>
		/// Symbols.
		/// </summary>
		Symbol = 1,
		/// <summary>
		/// Pairlists.
		/// </summary>
		Pairlist = 2,
		/// <summary>
		/// Closures.
		/// </summary>
		Closure = 3,
		/// <summary>
		/// Environments.
		/// </summary>
		Environment = 4,
		/// <summary>
		/// To be evaluated.
		/// </summary>
		Promise = 5,
		/// <summary>
		/// Pairlists for function calls.
		/// </summary>
		LanguageObject = 6,
		/// <summary>
		/// Special functions.
		/// </summary>
		SpecialFunction = 7,
		/// <summary>
		/// Builtin functions.
		/// </summary>
		BuiltinFunction = 8,
		/// <summary>
		/// Internal character string.
		/// </summary>
		InternalCharacterString = 9,
		/// <summary>
		/// Boolean vectors.
		/// </summary>
		LogicalVector = 10,
		/// <summary>
		/// Integer vectors.
		/// </summary>
		IntegerVector = 13,
		/// <summary>
		/// Numeric vectors.
		/// </summary>
		NumericVector = 14,
		/// <summary>
		/// Complex number vectors.
		/// </summary>
		ComplexVector = 15,
		/// <summary>
		/// Character vectors.
		/// </summary>
		CharacterVector = 16,
		/// <summary>
		/// Dot-dot-dot object.
		/// </summary>
		DotDotDotObject = 17,
		/// <summary>
		/// Place holders for any type. 
		/// </summary>
		Any = 18,
		/// <summary>
		/// Generic vectors.
		/// </summary>
		List = 19,
		/// <summary>
		/// Expression vectors.
		/// </summary>
		ExpressionVector = 20,
		/// <summary>
		/// Byte code.
		/// </summary>
		ByteCode = 21,
		/// <summary>
		/// External pointer.
		/// </summary>
		ExternalPointer = 22,
		/// <summary>
		/// Weak reference.
		/// </summary>
		WeakReference = 23,
		/// <summary>
		/// Raw vectors.
		/// </summary>
		RawVector = 24,
		/// <summary>
		/// S4 classes.
		/// </summary>
		S4 = 25,

		#region Original Definitions
		//[Obsolete("Use SEXPTYPE.Null instead.")]
		//NILSXP = Null,
		//[Obsolete("Use SEXPTYPE.Symbol istead.")]
		//SYMSXP = Symbol,
		//[Obsolete("Use SEXPTYPE.Pairlist istead.")]
		//LISTSXP = Pairlist,
		//[Obsolete("Use SEXPTYPE.Closure instead.")]
		//CLOSXP = Closure,
		//[Obsolete("Use SEXPTYPE.Environment instead.")]
		//ENVSXP = Environment,
		//[Obsolete("Use SEXPTYPE.Promise instead.")]
		//PROMSXP = Promise,
		//[Obsolete("Use SEXPTYPE.LanguageObject instead.")]
		//LANGSXP = LanguageObject,
		//[Obsolete("Use SEXPTYPE.SpecialFunction instead.")]
		//SPECIALSXP = SpecialFunction,
		//[Obsolete("Use SEXPTYPE.BuiltinFunction instead.")]
		//BUILTINSXP = BuiltinFunction,
		//[Obsolete("Use SEXPTYPE.InternalCharacterString instead.")]
		//CHARSXP = InternalCharacterString,
		//[Obsolete("Use SEXPTYPE.LogicalVector instead.")]
		//LGLSXP = LogicalVector,
		//[Obsolete("Use SEXPTYPE.IntegerVector instead.")]
		//INTSXP = IntegerVector,
		//[Obsolete("Use SEXPTYPE.NumericVector instead.")]
		//REALSXP = NumericVector,
		//[Obsolete("Use SEXPTYPE.ComplexVector instead.")]
		//CPLXSXP = ComplexVector,
		//[Obsolete("Use SEXPTYPE.CharacterVector instead.")]
		//STRSXP = CharacterVector,
		//[Obsolete("Use SEXPTYPE.DotDotDotObject instead.")]
		//DOTSXP = DotDotDotObject,
		//[Obsolete("Use SEXPTYPE.Any instead.")]
		//ANYSXP = Any,
		//[Obsolete("Use SEXPTYPE.List instead.")]
		//VECSXP = List,
		//[Obsolete("Use SEXPTYPE.ExpressionVector instead.")]
		//EXPRSXP = ExpressionVector,
		//[Obsolete("Use SEXPTYPE.ByteCode instead.")]
		//BCODESXP = ByteCode,
		//[Obsolete("Use SEXPTYPE.ExternalPointer instead.")]
		//EXTPTRSXP = ExternalPointer,
		//[Obsolete("Use SEXPTYPE.WeakReference instead.")]
		//WEAKREFSXP = WeakReference,
		//[Obsolete("Use SEXPTYPE.RawVector instead.")]
		//RAWSXP = RawVector,
		//[Obsolete("Use SEXPTYPE.S4 instead.")]
		//S4SXP = S4,
		/// <summary>
		/// Closures, builtin functions or special functions.
		/// </summary>
		[Obsolete("Use SEXPTYPE.Closure instead. But, note that value is different from SEXPTYPE.Closure.")]
		FUNSXP = 99,
		#endregion
	}
}
