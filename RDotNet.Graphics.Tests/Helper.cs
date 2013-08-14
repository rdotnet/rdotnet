using System;
using System.IO;
using Microsoft.Win32;

namespace RDotNet.Graphics
{
	internal static class Helper
	{
		internal static void SetEnvironmentVariables()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					Environment.SetEnvironmentVariable("PATH", FindRPathFromRegistry());
					break;
			}
		}

		internal static string FindRPathFromRegistry()
		{
			var rCore = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
			if (rCore == null)
			{
				throw new ApplicationException("Registry key is not found.");
			}
			var is64Bit = Environment.Is64BitProcess;
			var r = rCore.OpenSubKey(is64Bit ? "R64" : "R");
			if (r == null)
			{
				throw new ApplicationException("Registry key is not found.");
			}
			var currentVersion = new Version((string)r.GetValue("Current Version"));
			var installPath = (string)r.GetValue("InstallPath");
			var bin = Path.Combine(installPath, "bin");
			// Up to 2.11.x, DLLs are installed in R_HOME\bin.
			// From 2.12.0, DLLs are installed in the one level deeper directory.
			return currentVersion < new Version(2, 12) ? bin : Path.Combine(bin, is64Bit ? "x64" : "i386");
		}
	}
}
