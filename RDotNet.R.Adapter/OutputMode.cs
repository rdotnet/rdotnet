using System;

namespace RDotNet.R.Adapter
{
   [Flags]
   public enum OutputMode
   {
      None = 0x0,
      Quiet = 0x1,
      Slave = 0x2,
      Verbose = 0x4,
   }
}
