using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDotNet.Utilities
{
    public static class REngineExtensionsAdvanced
    {
        public static bool EqualsRNilValue(this REngine engine, IntPtr pointer)
        {
            return engine.NilValue.DangerousGetHandle() == pointer;
        }

        public static bool CheckUnbound(this REngine engine, IntPtr pointer)
        {
            return engine.UnboundValue.DangerousGetHandle() == pointer;
        }

    }
}
