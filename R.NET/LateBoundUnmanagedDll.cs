using System;
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

			IntPtr function = GetProcAddress(this.handle, entryPoint);
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

		/// <summary>
		/// Adds a directory to the search path used to locate DLLs for the application.
		/// </summary>
		/// <remarks>
		/// Calls <c>SetDllDirectory</c> in the kernel32.dll.
		/// </remarks>
		/// <param name="dllDirectory">
		/// The directory to be added to the search path.
		/// If this parameter is an empty string (""), the call removes the current directory from the default DLL search order.
		/// If this parameter is NULL, the function restores the default search order.
		/// </param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetDllDirectory([MarshalAs(UnmanagedType.LPStr)] string dllDirectory);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
	}
}
