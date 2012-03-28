using System;
using System.IO;

namespace RDotNet.Tests
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
				case PlatformID.MacOSX:
					Environment.SetEnvironmentVariable("R_HOME", "/Library/Frameworks/R.framework/R");
					Environment.SetEnvironmentVariable("PATH", "/Library/Frameworks/R.framework/Libraries");
					break;
				case PlatformID.Unix:
					Environment.SetEnvironmentVariable("R_HOME", "/usr/lib/R");
					Environment.SetEnvironmentVariable("PATH", "/usr/lib");
					break;
			}
		}

		internal static string FindRPathFromRegistry()
		{
			Microsoft.Win32.RegistryKey rCore = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core");
			if (rCore == null)
			{
				throw new ApplicationException("Registry key is not found.");
			}
			bool is64Bit = Environment.Is64BitProcess;
			Microsoft.Win32.RegistryKey r = rCore.OpenSubKey(is64Bit ? "R64" : "R");
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
