using System;

namespace RDotNet.Utilities
{
    /// <summary>
    /// Advanced, less usual extension methods for the R.NET REngine
    /// </summary>
    public static class REngineExtensionsAdvanced
    {
        /// <summary>
        /// Checks the equality in native memory of a pointer against a pointer to the R 'NULL' value
        /// </summary>
        /// <param name="engine">R.NET Rengine</param>
        /// <param name="pointer">Pointer to test</param>
        /// <returns>True if the pointer and pointer to R NULL are equal</returns>
        public static bool EqualsRNilValue(this REngine engine, IntPtr pointer)
        {
            return engine.NilValue.DangerousGetHandle() == pointer;
        }

        /// <summary>
        /// Checks the equality in native memory of a pointer against a pointer to the R 'R_UnboundValue',
        /// i.e. whether a symbol exists (i.e. functional equivalent to "exists('varName')" in R)
        /// </summary>
        /// <param name="engine">R.NET Rengine</param>
        /// <param name="pointer">Pointer to test</param>
        /// <returns>True if the pointer is not bound to a value</returns>
        public static bool CheckUnbound(this REngine engine, IntPtr pointer)
        {
            return engine.UnboundValue.DangerousGetHandle() == pointer;
        }
    }
}