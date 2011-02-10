using System;

namespace RDotNet.Internals
{
	internal interface INativeMethodsProxy
	{
		int Rf_initEmbeddedR(int argc, string[] argv);
		void Rf_endEmbeddedR(int fatal);
		IntPtr Rf_protect(IntPtr sexp);
		void Rf_unprotect(int count);
		void Rf_unprotect_ptr(IntPtr sexp);
		IntPtr Rf_install(string sexp);
		IntPtr Rf_mkString(string s);
		IntPtr Rf_mkChar(string s);
		IntPtr Rf_allocVector(SymbolicExpressionType type, int length);
		IntPtr Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type);
		bool Rf_isVector(IntPtr sexp);
		int Rf_length(IntPtr sexp);
		IntPtr Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount);
		bool Rf_isMatrix(IntPtr sexp);
		int Rf_nrows(IntPtr sexp);
		int Rf_ncols(IntPtr sexp);
		IntPtr Rf_allocList(int length);
		bool Rf_isList(IntPtr sexp);
		IntPtr R_tryEval(IntPtr statement, IntPtr environment, out bool errorOccurred);
		IntPtr R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _);
		IntPtr Rf_findVar(IntPtr name, IntPtr environment);
		void Rf_setVar(IntPtr name, IntPtr value, IntPtr environment);
		IntPtr Rf_getAttrib(IntPtr sexp, IntPtr name);
		IntPtr Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value);
	}
}

