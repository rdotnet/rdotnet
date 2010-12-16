using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet
{
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
