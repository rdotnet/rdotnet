using System;
using System.Runtime.InteropServices;

namespace RDotNet.Graphics.Internals
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void R_GE_checkVersionOrDie(int version);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void R_CheckDeviceAvailable();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void Rf_onintr();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate IntPtr GEcreateDevDesc(IntPtr dev);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void GEaddDevice2(IntPtr dev, string name);
}
