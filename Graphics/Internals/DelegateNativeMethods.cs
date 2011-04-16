using System;
using System.Security.Permissions;

namespace RDotNet.Graphics.Internals
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal partial class DelegateNativeMethods : INativeMethodsProxy
	{
		private readonly LateBoundUnmanagedDll dll;

		public DelegateNativeMethods(LateBoundUnmanagedDll dll)
		{
			if (dll == null)
			{
				throw new ArgumentNullException();
			}
			this.dll = dll;
		}
	}
}
