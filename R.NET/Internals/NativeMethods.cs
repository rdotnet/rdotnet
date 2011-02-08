using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.Internals
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal static class NativeMethods
	{
		public const string RDllName = "R.dll";
		public const string RDimSymbolName = "R_DimSymbol";
		public const string RNamesSymbolName = "R_NamesSymbol";
		public const string RDimnamesSymbolName = "R_DimNamesSymbol";

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int Rf_initEmbeddedR(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Rf_endEmbeddedR(int fatal);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "getDLLVersion")]
		public static extern IntPtr GetDllVersion();

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_protect(IntPtr sexp);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Rf_unprotect(int count);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Rf_unprotect_ptr(IntPtr sexp);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_install([MarshalAs(UnmanagedType.LPStr)] string expression);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_mkString([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_mkChar([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_allocVector(SymbolicExpressionType type, int length);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool Rf_isVector(IntPtr sexp);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int Rf_length(IntPtr expression);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool Rf_isMatrix(IntPtr sexp);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int Rf_nrows(IntPtr expression);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern int Rf_ncols(IntPtr expression);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_allocList(int length);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool Rf_isList(IntPtr sexp);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr R_tryEval(IntPtr statement, IntPtr environment, [MarshalAs(UnmanagedType.Bool)] out bool errorOccurred);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_findVar(IntPtr name, IntPtr environment);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern void Rf_setVar(IntPtr name, IntPtr value, IntPtr environment);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_getAttrib(IntPtr sexp, IntPtr name);

		[DllImport(RDllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value);
	}
}
