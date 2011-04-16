using System;

namespace RDotNet.Graphics
{
	public class GraphicsDeviceEventArgs : EventArgs
	{
		private readonly DeviceDescription description;
		public DeviceDescription Description
		{
			get
			{
				return this.description;
			}
		}

		private readonly GraphicsContext context;
		public GraphicsContext Context
		{
			get
			{
				return this.context;
			}
		}

		public GraphicsDeviceEventArgs(DeviceDescription description, GraphicsContext context = null)
		{
			this.description = description;
			this.context = context;
		}
	}
}
