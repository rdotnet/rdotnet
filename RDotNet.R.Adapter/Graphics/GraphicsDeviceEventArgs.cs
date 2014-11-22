using System;

namespace RDotNet.R.Adapter.Graphics
{
   public class GraphicsDeviceEventArgs : EventArgs
   {
       public GraphicsDeviceEventArgs(DeviceDescription description, GraphicsContext context = null)
      {
         Description = description;
         Context = context;
      }

       public DeviceDescription Description { get; private set; }
       public GraphicsContext Context { get; private set; }
   }
}
