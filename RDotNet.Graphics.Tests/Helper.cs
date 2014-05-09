using Microsoft.Win32;
using System;
using System.IO;
using RDotNet.NativeLibrary;

namespace RDotNet.Graphics
{
   internal static class Helper
   {
      internal static void SetEnvironmentVariables()
      {
         NativeUtility.SetEnvironmentVariables();
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
         return NativeUtility.FindRPathFromRegistry();
      }
   }
}