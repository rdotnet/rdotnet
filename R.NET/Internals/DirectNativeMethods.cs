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
		
		public void Rf_endEmbeddedR(int fatal)
		{
			_Rf_endEmbeddedR(fatal);
		}
		
		public int Rf_initEmbeddedR(int argc, string[] argv)
		{
			return _Rf_initEmbeddedR(argc, argv);
		}
		
		public IntPtr getDLLVersion()
		{
			return _getDllVersion();
		}
		
		public IntPtr Rf_protect(IntPtr sexp)
		{
			return _Rf_protect(sexp);
		}
		
		public void Rf_unprotect(int count)
		{
			_Rf_unprotect(count);
		}
		
		public void Rf_unprotect_ptr(IntPtr sexp)
		{
			_Rf_unprotect_ptr(sexp);
		}
		
		public IntPtr Rf_install(string sexp)
		{
			return _Rf_install(sexp);
		}
		
		public IntPtr Rf_mkString(string s)
		{
			return _Rf_mkString(s);
		}
		
		public IntPtr Rf_mkChar(string s)
		{
			return _Rf_mkChar(s);
		}
		
		public IntPtr Rf_allocVector(SymbolicExpressionType type, int length)
		{
			return _Rf_allocVector(type, length);
		}
		
		public IntPtr Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type)
		{
			return _Rf_coerceVector(sexp, type);
		}
		
		public bool Rf_isVector(IntPtr sexp)
		{
			return _Rf_isVector(sexp);
		}
		
		public int Rf_length(IntPtr sexp)
		{
			return _Rf_length(sexp);
		}
		
		public IntPtr Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount)
		{
			return _Rf_allocMatrix(type, rowCount, columnCount);
		}
		
		public bool Rf_isMatrix(IntPtr sexp)
		{
			return _Rf_isMatrix(sexp);
		}
		
		public int Rf_nrows(IntPtr sexp)
		{
			return _Rf_nrows(sexp);
		}
		
		public int Rf_ncols(IntPtr sexp)
		{
			return _Rf_ncols(sexp);
		}
		public IntPtr Rf_allocList(int length)
		{
			return _Rf_allocList(length);
		}
		
		public bool Rf_isList(IntPtr sexp)
		{
			return _Rf_isList(sexp);
		}
		
		public IntPtr R_tryEval(IntPtr statement, IntPtr environment, out bool errorOccurred)
		{
			return _R_tryEval(statement, environment, out errorOccurred);
		}
		
		public IntPtr R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _)
		{
			return _R_ParseVector(statement, statementCount, out status, _);
		}
		
		public IntPtr Rf_findVar(IntPtr name, IntPtr environment)
		{
			return _Rf_findVar(name, environment);
		}
		
		public void Rf_setVar(IntPtr name, IntPtr value, IntPtr environment)
		{
			_Rf_setVar(name, value, environment);
		}
		
		public IntPtr Rf_getAttrib(IntPtr sexp, IntPtr name)
		{
			return _Rf_getAttrib(sexp, name);
		}
		
		public IntPtr Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value)
		{
			return _Rf_setAttrib(sexp, name, value);
		}

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_initEmbeddedR")]
		private static extern int _Rf_initEmbeddedR(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_endEmbeddedR")]
		private static extern void _Rf_endEmbeddedR(int fatal);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "getDLLVersion")]
		private static extern IntPtr _getDllVersion();

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_protect")]
		private static extern IntPtr _Rf_protect(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_unprotect")]
		private static extern void _Rf_unprotect(int count);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_unprotect_ptr")]
		private static extern void _Rf_unprotect_ptr(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_install")]
		private static extern IntPtr _Rf_install([MarshalAs(UnmanagedType.LPStr)] string sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_mkString")]
		private static extern IntPtr _Rf_mkString([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_mkChar")]
		private static extern IntPtr _Rf_mkChar([MarshalAs(UnmanagedType.LPStr)] string s);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocVector")]
		private static extern IntPtr _Rf_allocVector(SymbolicExpressionType type, int length);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_coerceVector")]
		private static extern IntPtr _Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isVector")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isVector(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_length")]
		private static extern int _Rf_length(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocMatrix")]
		private static extern IntPtr _Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isMatrix")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isMatrix(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_nrows")]
		private static extern int _Rf_nrows(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_ncols")]
		private static extern int _Rf_ncols(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_allocList")]
		private static extern IntPtr _Rf_allocList(int length);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_isList")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool _Rf_isList(IntPtr sexp);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_tryEval")]
		private static extern IntPtr _R_tryEval(IntPtr statement, IntPtr environment, [MarshalAs(UnmanagedType.Bool)] out bool errorOccurred);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "R_ParseVector")]
		private static extern IntPtr _R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_findVar")]
		private static extern IntPtr _Rf_findVar(IntPtr name, IntPtr environment);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_setVar")]
		private static extern void _Rf_setVar(IntPtr name, IntPtr value, IntPtr environment);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_getAttrib")]
		private static extern IntPtr _Rf_getAttrib(IntPtr sexp, IntPtr name);

		[DllImport(NativeMethods.RDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rf_setAttrib")]
		private static extern IntPtr _Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value);
	}
}

