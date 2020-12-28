namespace RDotNet.Internals.Windows
{
    /// <summary>
    /// User interface mode
    /// </summary>
    public enum UiMode
    {
        /// <summary>
        /// R graphical user interface
        /// </summary>
        RGui,

        /// <summary>
        /// R terminal console
        /// </summary>
        RTerminal,

        /// <summary>
        /// R dynamic (shared) library
        /// </summary>
        LinkDll
    }
}