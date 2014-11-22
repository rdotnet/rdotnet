using System;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter
{
    public interface IPlatformManager : IDisposable
    {
        bool FreeLibrary(SafeHandle handle);
        PlatformID GetPlatform();
        string GetLastError();
        IntPtr GetProcAddress(string procName);
        string ConstructRPath(string rHome);
        string FindRHome(string rPath);
        string GetRLibraryFileName();
        void DoPlatformSpecificPreStart(InstanceStartupParameter startupParameter, RInstance instance);
        string GetDllVersion();
        void DoPlatformSpecificPostStart();
        string GetProcessBitness();
        string GetLoadLibError();
        InstanceStartupParameter ConfigureCharacterDevice(InstanceStartupParameter parameter, ICharacterDevice device);
        InstanceStartupParameter DoPlatformSpecificPreInitialization(InstanceStartupParameter parameter);
        void Initialize();
    }
}
