using System;

namespace RDotNet.Graphics
{
    public class GraphicsDeviceEventArgs : EventArgs
    {
        private readonly GraphicsContext context;
        private readonly DeviceDescription description;

        public GraphicsDeviceEventArgs(DeviceDescription description, GraphicsContext context = null)
        {
            this.description = description;
            this.context = context;
        }

        public DeviceDescription Description
        {
            get { return this.description; }
        }

        public GraphicsContext Context
        {
            get { return this.context; }
        }
    }
}