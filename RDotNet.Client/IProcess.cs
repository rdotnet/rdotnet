namespace RDotNet.Client
{
    public interface IProcess
    {
        void Kill();
        string MainWindowTitle { get; }
        bool HasExited { get; }
    }
}
