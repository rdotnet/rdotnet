namespace RDotNet.Graphics.Internals
{
	internal static class Constants
	{
#if MAC
		public const string RDllName = "libR.dylib";
#elif LINUX
		public const string RDllName = "libR.so";
#elif WINDOWS
		public const string RDllName = "R.dll";
#endif
		public static readonly int R_GE_version = 8;
	}
}
