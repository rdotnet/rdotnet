using System;

namespace RDotNet.Graphics.Internals
{
	internal interface INativeMethodsProxy
	{
		void R_GE_checkVersionOrDie(int version);
		void R_CheckDeviceAvailable();
		void Rf_onintr();
		IntPtr GEcreateDevDesc(IntPtr dev);
		void GEaddDevice2(IntPtr dev, string name);
	}
}
