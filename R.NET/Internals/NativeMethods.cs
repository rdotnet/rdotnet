namespace RDotNet.Internals
{
	internal static class NativeMethods
	{
#if MAC
		public const string RDllName = "libR.dylib";
#elif LINUX
		public const string RDllName = "libR.so";
#else
		public const string RDllName = "R.dll";
#endif
		public const string RDimSymbolName = "R_DimSymbol";
		public const string RNamesSymbolName = "R_NamesSymbol";
		public const string RDimnamesSymbolName = "R_DimNamesSymbol";
	}
}
