using System;

namespace RDotNet.NativeLibrary
{
   /// <summary>
   /// An interface definition to hide the platform specific aspects of library loading
   /// </summary>
   /// <remarks>There are probably projects 'out there' doing this already, but nothing 
   /// is obvious from a quick search. Re-consider again if you need more features.</remarks>
   public interface IDynamicLibraryLoader
   {
      IntPtr LoadLibrary(string filename);

      string GetLastError();

      bool FreeLibrary(IntPtr handle);

      IntPtr GetFunctionAddress(IntPtr hModule, string lpProcName);
   }
}

