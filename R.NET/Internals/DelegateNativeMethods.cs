using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.Internals
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal class DelegateNativeMethods : INativeMethodsProxy
	{
		private readonly LateBoundUnmanagedDll dll;
		
		public DelegateNativeMethods(LateBoundUnmanagedDll dll)
		{
			if (dll == null)
			{
				throw new ArgumentNullException();
			}
			this.dll = dll;
		}
	
		public int Rf_initEmbeddedR (int argc, string[] argv)
		{
			var function = this.dll.GetFunction<_Rf_initEmbeddedR>("Rf_initEmbeddedR");
			return function(argc, argv);
		}

		public void Rf_endEmbeddedR (int fatal)
		{
			var function = this.dll.GetFunction<_Rf_endEmbeddedR>("Rf_endEmbeddedR");
			function(fatal);
		}

		public IntPtr getDLLVersion ()
		{
			var function = this.dll.GetFunction<_getDllVersion>("getDLLVersion");
			return function();
		}

		public IntPtr Rf_protect (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_protect>("Rf_protect");
			return function(sexp);
		}

		public void Rf_unprotect (int count)
		{
			var function = this.dll.GetFunction<_Rf_unprotect>("Rf_unprotect");
			function(count);
		}

		public void Rf_unprotect_ptr (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_unprotect_ptr>("Rf_unprotect_ptr");
			function(sexp);
		}

		public IntPtr Rf_install (string sexp)
		{
			var function = this.dll.GetFunction<_Rf_install>("Rf_install");
			return function(sexp);
		}

		public IntPtr Rf_mkString (string s)
		{
			var function = this.dll.GetFunction<_Rf_mkString>("Rf_mkString");
			return function(s);
		}

		public IntPtr Rf_mkChar (string s)
		{
			var function = this.dll.GetFunction<_Rf_mkChar>("Rf_mkChar");
			return function(s);
		}

		public IntPtr Rf_allocVector (SymbolicExpressionType type, int length)
		{
			var function = this.dll.GetFunction<_Rf_allocVector>("Rf_allocVector");
			return function(type, length);
		}

		public IntPtr Rf_coerceVector (IntPtr sexp, SymbolicExpressionType type)
		{
			var function = this.dll.GetFunction<_Rf_coerceVector>("Rf_coerceVector");
			return function(sexp, type);
		}

		public bool Rf_isVector (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_isVector>("Rf_isVector");
			return function(sexp);
		}

		public int Rf_length (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_length>("Rf_length");
			return function(sexp);
		}

		public IntPtr Rf_allocMatrix (SymbolicExpressionType type, int rowCount, int columnCount)
		{
			var function = this.dll.GetFunction<_Rf_allocMatrix>("Rf_allocMatrix");
			return function(type, rowCount, columnCount);
		}

		public bool Rf_isMatrix (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_isMatrix>("Rf_isMatrix");
			return function(sexp);
		}

		public int Rf_nrows (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_nrows>("Rf_nrows");
			return function(sexp);
		}

		public int Rf_ncols (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_ncols>("Rf_ncols");
			return function(sexp);
		}

		public IntPtr Rf_allocList (int length)
		{
			var function = this.dll.GetFunction<_Rf_allocList>("Rf_allocList");
			return function(length);
		}

		public bool Rf_isList (IntPtr sexp)
		{
			var function = this.dll.GetFunction<_Rf_isList>("Rf_isList");
			return function(sexp);
		}

		public IntPtr R_tryEval (IntPtr statement, IntPtr environment, out bool errorOccurred)
		{
			var function = this.dll.GetFunction<_R_tryEval>("R_tryEval");
			return function(statement, environment, out errorOccurred);
		}

		public IntPtr R_ParseVector (IntPtr statement, int statementCount, out ParseStatus status, IntPtr _)
		{
			var function = this.dll.GetFunction<_R_ParseVector>("R_ParseVector");
			return function(statement, statementCount, out status, _);
		}

		public IntPtr Rf_findVar (IntPtr name, IntPtr environment)
		{
			var function = this.dll.GetFunction<_Rf_findVar>("Rf_findVar");
			return function(name, environment);
		}

		public void Rf_setVar (IntPtr name, IntPtr value, IntPtr environment)
		{
			var function = this.dll.GetFunction<_Rf_setVar>("Rf_setVar");
			function(name, value, environment);
		}

		public IntPtr Rf_getAttrib (IntPtr sexp, IntPtr name)
		{
			var function = this.dll.GetFunction<_Rf_getAttrib>("Rf_getAttrib");
			return function(sexp, name);
		}

		public IntPtr Rf_setAttrib (IntPtr sexp, IntPtr name, IntPtr value)
		{
			var function = this.dll.GetFunction<_Rf_setAttrib>("Rf_setAttrib");
			return function(sexp, name, value);
		}
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _Rf_initEmbeddedR(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void _Rf_endEmbeddedR(int fatal);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _getDllVersion();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_protect(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void _Rf_unprotect(int count);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void _Rf_unprotect_ptr(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_install([MarshalAs(UnmanagedType.LPStr)] string sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_mkString([MarshalAs(UnmanagedType.LPStr)] string s);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_mkChar([MarshalAs(UnmanagedType.LPStr)] string s);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_allocVector(SymbolicExpressionType type, int length);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_coerceVector(IntPtr sexp, SymbolicExpressionType type);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool _Rf_isVector(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _Rf_length(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_allocMatrix(SymbolicExpressionType type, int rowCount, int columnCount);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool _Rf_isMatrix(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _Rf_nrows(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int _Rf_ncols(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_allocList(int length);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool _Rf_isList(IntPtr sexp);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _R_tryEval(IntPtr statement, IntPtr environment, [MarshalAs(UnmanagedType.Bool)] out bool errorOccurred);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _R_ParseVector(IntPtr statement, int statementCount, out ParseStatus status, IntPtr _);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_findVar(IntPtr name, IntPtr environment);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void _Rf_setVar(IntPtr name, IntPtr value, IntPtr environment);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_getAttrib(IntPtr sexp, IntPtr name);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _Rf_setAttrib(IntPtr sexp, IntPtr name, IntPtr value);
	}
}
