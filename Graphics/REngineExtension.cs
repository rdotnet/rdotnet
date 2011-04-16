using System;
using System.Collections.Generic;
using System.Text;

namespace RDotNet.Graphics
{
	public static class REngineExtension
	{
		public static void Install(this REngine engine, GraphicsDevice device)
		{
			//GraphicsDeviceAdapter adapter = new GraphicsDeviceAdapter(device);
			//adapter.Install(engine);
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			device.SetEngine(engine);
		}
	}
}
