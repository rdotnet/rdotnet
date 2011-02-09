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
	}
}
