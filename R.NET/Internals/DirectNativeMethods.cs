using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.Internals
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	internal partial class DirectNativeMethods : INativeMethodsProxy
	{
		public static readonly DirectNativeMethods Instance = new DirectNativeMethods();

		private DirectNativeMethods()
		{
		}
	}
}
