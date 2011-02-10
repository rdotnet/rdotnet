using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.Internals
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal class DirectNativeMethods : INativeMethodsProxy
	{
		public static readonly DirectNativeMethods Instance = new DirectNativeMethods();

		private DirectNativeMethods()
		{
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_initEmbeddedR")]
		private static extern int _Rf_initEmbeddedR(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);
		public int Rf_initEmbeddedR(int argc, string[] argv)
		{
			return _Rf_initEmbeddedR(argc, argv);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_endEmbeddedR")]
		private static extern void _Rf_endEmbeddedR(int fatal);
		public void Rf_endEmbeddedR(int fatal)
		{
			_Rf_endEmbeddedR(fatal);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_protect")]
		private static extern IntPtr _Rf_protect(IntPtr sexp);
		public IntPtr Rf_protect(IntPtr sexp)
		{
			return _Rf_protect(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_unprotect")]
		private static extern void _Rf_unprotect(int count);
		public void Rf_unprotect(int count)
		{
			_Rf_unprotect(count);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_unprotect_ptr")]
		private static extern void _Rf_unprotect_ptr(IntPtr sexp);
		public void Rf_unprotect_ptr(IntPtr sexp)
		{
			_Rf_unprotect_ptr(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_install")]
		private static extern IntPtr _Rf_install([MarshalAs(UnmanagedType.LPStr)] string s);
		public IntPtr Rf_install(string s)
		{
			return _Rf_install(s);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_mkString")]
		private static extern IntPtr _Rf_mkString([MarshalAs(UnmanagedType.LPStr)] string s);
		public IntPtr Rf_mkString(string s)
		{
			return _Rf_mkString(s);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_mkChar")]
		private static extern IntPtr _Rf_mkChar([MarshalAs(UnmanagedType.LPStr)] string s);
		public IntPtr Rf_mkChar(string s)
		{
			return _Rf_mkChar(s);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocVector")]
		private static extern IntPtr _Rf_allocVector(SymbolicExpressionType type, int length);
		public IntPtr Rf_allocVector(SymbolicExpressionType type, int length)
		{
			return _Rf_allocVector(type, length);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_coerceVector")]
		private static extern IntPtr _Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type);
		public IntPtr Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type)
		{
			return _Rf_coerceVector(sexp, type);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isVector")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isVector(IntPtr sexp);
		public bool Rf_isVector(IntPtr sexp)
		{
			return _Rf_isVector(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_length")]
		private static extern int _Rf_length(IntPtr sexp);
		public int Rf_length(IntPtr sexp)
		{
			return _Rf_length(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocMatrix")]
		private static extern IntPtr _Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount);
		public IntPtr Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount)
		{
			return _Rf_allocMatrix(type, rowCount, columnCount);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isMatrix")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isMatrix(IntPtr sexp);
		public bool Rf_isMatrix(IntPtr sexp)
		{
			return _Rf_isMatrix(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_nrows")]
		private static extern int _Rf_nrows(IntPtr sexp);
		public int Rf_nrows(IntPtr sexp)
		{
			return _Rf_nrows(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_ncols")]
		private static extern int _Rf_ncols(IntPtr sexp);
		public int Rf_ncols(IntPtr sexp)
		{
			return _Rf_ncols(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocList")]
		private static extern IntPtr _Rf_allocList(int length);
		public IntPtr Rf_allocList(int length)
		{
			return _Rf_allocList(length);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isList")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isList(IntPtr sexp);
		public bool Rf_isList(IntPtr sexp)
		{
			return _Rf_isList(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_tryEval")]
		private static extern IntPtr _Rf_eval(IntPtr statement, IntPtr environment);
		public IntPtr Rf_eval(IntPtr statement, IntPtr environment)
		{
			return _Rf_eval(statement, environment);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_tryEval")]
		private static extern IntPtr _R_tryEval(IntPtr statement, IntPtr environment, [MarshalAs(UnmanagedType.Bool)] out bool errorOccurred);
		public IntPtr R_tryEval(IntPtr statement, IntPtr environment, out bool errorOccurred)
		{
			return _R_tryEval(statement, environment, out errorOccurred);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_ParseVector")]
		private static extern IntPtr _R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _);
		public IntPtr R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _)
		{
			return _R_ParseVector(statement, statementCount, out status, _);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_findVar")]
		private static extern IntPtr _Rf_findVar(IntPtr name, IntPtr environment);
		public IntPtr Rf_findVar(IntPtr name, IntPtr environment)
		{
			return _Rf_findVar(name, environment);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_setVar")]
		private static extern void _Rf_setVar(IntPtr name, IntPtr value, IntPtr environment);
		public void Rf_setVar(IntPtr name, IntPtr value, IntPtr environment)
		{
			_Rf_setVar(name, value, environment);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_getAttrib")]
		private static extern IntPtr _Rf_getAttrib(IntPtr sexp, IntPtr name);
		public IntPtr Rf_getAttrib(IntPtr sexp, IntPtr name)
		{
			return _Rf_getAttrib(sexp, name);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_setAttrib")]
		private static extern IntPtr _Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value);
		public IntPtr Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value)
		{
			return _Rf_setAttrib(sexp, name, value);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isEnvironment")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isEnvironment(IntPtr sexp);
		public bool Rf_isEnvironment(IntPtr sexp)
		{
			return _Rf_isEnvironment(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isExpression")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isExpression(IntPtr sexp);
		public bool Rf_isExpression(IntPtr sexp)
		{
			return _Rf_isExpression(sexp);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_lsInternal")]
		private static extern IntPtr _R_lsInternal(IntPtr environment, [MarshalAs(UnmanagedType.Bool)] bool all);
		public IntPtr R_lsInternal(IntPtr environment, bool all)
		{
			return _R_lsInternal(environment, all);
		}
	}
}
