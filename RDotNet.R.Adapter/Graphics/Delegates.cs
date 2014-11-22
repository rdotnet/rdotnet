using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter.Graphics
{
   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   internal delegate void R_GE_checkVersionOrDie(int version);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void R_CheckDeviceAvailable();

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void Rf_onintr();

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate IntPtr GEcreateDevDesc(IntPtr dev);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void GEaddDevice2(IntPtr dev, string name);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   public delegate void GEkillDevice(IntPtr dev);
}