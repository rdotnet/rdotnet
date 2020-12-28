using RDotNet.NativeLibrary;

namespace RDotNet.Graphics
{
    internal static class Helper
    {
        internal static void SetEnvironmentVariables()
        {
            new NativeUtility().SetEnvironmentVariables();
            // TOCHECK: was the following deliberate?
            //switch (Environment.OSVersion.Platform)
            //{
            //   case PlatformID.Win32NT:
            //      Environment.SetEnvironmentVariable("PATH", FindRPathFromRegistry());
            //      break;
            //}
        }

        internal static string FindRPathFromRegistry()
        {
            return new NativeUtility().FindRPathFromRegistry();
        }
    }
}