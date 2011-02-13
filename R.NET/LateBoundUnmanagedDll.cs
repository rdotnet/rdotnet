using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
	/// <summary>
	/// A proxy for unmanaged dynamic link library (DLL).
	/// </summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class LateBoundUnmanagedDll : SafeHandle
	{	
		public override bool IsInvalid
		{
			get
			{
				return IsClosed || this.handle == IntPtr.Zero;
			}
		}

		/// <summary>
		/// Creates a proxy for the specified dll.
		/// </summary>
		/// <param name="dllName">The DLL's name.</param>
		public LateBoundUnmanagedDll(string dllName)
			: base(IntPtr.Zero, true)
		{
			if (dllName == null)
			{
				throw new ArgumentNullException("dllName");
			}
			if (dllName == string.Empty)
			{
				throw new ArgumentException("dllName");
			}
			
			IntPtr handle = LoadLibrary(dllName);
			if (handle == IntPtr.Zero)
			{
				throw new DllNotFoundException();
			}
			SetHandle(handle);
		}

		/// <summary>
		/// Creates the delegate function for the specified function defined in the DLL.
		/// </summary>
		/// <typeparam name="TDelegate">The type of delegate.</typeparam>
		/// <param name="entryPoint">The name of function.</param>
		/// <returns>The delegate.</returns>
		public TDelegate GetFunction<TDelegate>(string entryPoint)
			where TDelegate : class
		{
			if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
			{
				throw new ArgumentException();
			}
			if (entryPoint == null)
			{
				throw new ArgumentNullException("entryPoint");
			}
			
			IntPtr function = GetFunctionAddress(this.handle, entryPoint);
			if (function == IntPtr.Zero)
			{
				throw new EntryPointNotFoundException();
			}

			return Marshal.GetDelegateForFunctionPointer(function, typeof(TDelegate)) as TDelegate;
		}

		protected override bool ReleaseHandle()
		{
			return FreeLibrary(this.handle);
		}

#if MAC
		private const string LibraryPath = "DYLD_LIBRARY_PATH";
		private const string DynamicLoadingLibraryName = "libdl.dylib";
#elif LINUX
		private const string LibraryPath = "LD_LIBRARY_PATH";
		private const string DynamicLoadingLibraryName = "libdl.so";
#endif

		/// <summary>
		/// Adds a directory to the search path used to locate DLLs for the application.
		/// </summary>
		/// <remarks>
		/// Calls <c>SetDllDirectory</c> in the kernel32.dll on Windows.
		/// </remarks>
		/// <param name="dllDirectory">
		/// The directory to be added to the search path.
		/// If this parameter is an empty string (""), the call removes the current directory from the default DLL search order.
		/// If this parameter is NULL, the function restores the default search order.
		/// </param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
#if MAC || LINUX
		public static bool SetDllDirectory(string dllDirectory)
		{
			if (dllDirectory == null)
			{
				System.Environment.SetEnvironmentVariable(LibraryPath, DefaultSearchPath, EnvironmentVariableTarget.Process);
			}
			else if (dllDirectory == string.Empty)
			{
				throw new NotImplementedException();
			}
			else
			{
				if (!Directory.Exists(dllDirectory))
				{
					return false;
				}
				string path = System.Environment.GetEnvironmentVariable(LibraryPath, EnvironmentVariableTarget.Process);
				if (string.IsNullOrEmpty(path))
				{
					path = dllDirectory;
				}
				else
				{
					path = dllDirectory + Path.PathSeparator + path;
				}
				System.Environment.SetEnvironmentVariable(LibraryPath, path, EnvironmentVariableTarget.Process);
			}
			return true;
		}
				
		private static readonly string DefaultSearchPath = System.Environment.GetEnvironmentVariable(LibraryPath, EnvironmentVariableTarget.Process);
#else
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetDllDirectory([MarshalAs(UnmanagedType.LPStr)] string dllDirectory);
#endif
		
#if MAC || LINUX
		private static IntPtr LoadLibrary(string filename)
		{
			const int RTLD_LAZY = 0x1;
			
			if (filename.StartsWith("/"))
			{
				return dlopen(filename, RTLD_LAZY);
			}
			
			string[] searchPaths = (System.Environment.GetEnvironmentVariable(LibraryPath, EnvironmentVariableTarget.Process) ?? "").Split(Path.PathSeparator);
			foreach (string directory in searchPaths)
			{
				string path = Path.Combine(directory, filename);
				if (File.Exists(path))
				{
					return dlopen(path, RTLD_LAZY);
				}
			}
			return IntPtr.Zero;
		}
		
		[DllImport(DynamicLoadingLibraryName)]
		private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPStr)] string filename, int flag);
#else	
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
#endif

#if MAC || LINUX
		[DllImport(DynamicLoadingLibraryName, EntryPoint = "dlclose")]
#else
		[DllImport("kernel32.dll")]
#endif
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

#if MAC || LINUX
		[DllImport(DynamicLoadingLibraryName, EntryPoint = "dlsym")]
#else
		[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
#endif
		private static extern IntPtr GetFunctionAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
	}
}
