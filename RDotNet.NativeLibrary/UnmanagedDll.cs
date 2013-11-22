using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace RDotNet.NativeLibrary
{
   /// <summary>
   /// A proxy for unmanaged dynamic link library (DLL).
   /// </summary>
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class UnmanagedDll : SafeHandle
   {
      public override bool IsInvalid
      {
         get { return handle == IntPtr.Zero; }
      }

      /// <summary>
      /// Creates a proxy for the specified dll.
      /// </summary>
      /// <param name="dllName">The DLL's name.</param>
      public UnmanagedDll(string dllName)
         : base(IntPtr.Zero, true)
      {
         if (dllName == null)
         {
            throw new ArgumentNullException("dllName", "The name of the library to load is a null reference");
         }
         if (dllName == string.Empty)
         {
            throw new ArgumentException("The name of the library to load is an empty string", "dllName");
         }

         IntPtr handle = IsUnix
                            ? UnixLibraryLoader.LoadLibrary(dllName)
                            : WindowsLibraryLoader.LoadLibrary(dllName);
         if (handle == IntPtr.Zero)
         {
            ReportLoadLibError(dllName);
         }
         SetHandle(handle);
      }

      private static void ReportLoadLibError(string dllName)
      {
         string dllFullName = dllName;
         if (File.Exists(dllFullName))
            ThrowFailedLibraryLoad(dllFullName);
         else
         {
            // This below assumes that the PATH environment variable is what is relied on
            // TODO: check whether there is more to it: http://msdn.microsoft.com/en-us/library/ms682586.aspx

            // Also some pointers to relevant information if we want to check whether the attempt to load 
            // was made on a 32 or 64 bit library
            // For Windows:
            // http://stackoverflow.com/questions/1345632/determine-if-an-executable-or-library-is-32-or-64-bits-on-windows
            // http://www.neowin.net/forum/topic/732648-check-if-exe-is-x64/?p=590544108#entry590544108
            // Linux, and perhaps MacOS; the 'file' command seems the way to go.
            // http://stackoverflow.com/questions/5665228/in-linux-determine-if-a-a-library-archive-32-bit-or-64-bit

            dllFullName = FindFullPath(dllName);
            if (string.IsNullOrEmpty(dllFullName))
               throw new DllNotFoundException(string.Format("Could not find the library named {0} in the search paths", dllName));
            else
               ThrowFailedLibraryLoad(dllFullName);
         }
      }

      private static string FindFullPath(string dllName)
      {
         string dllFullName;
         var searchPaths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
         dllFullName = searchPaths.Select(directory => Path.Combine(directory, dllName)).FirstOrDefault(File.Exists);
         return dllFullName;
      }

      private static void ThrowFailedLibraryLoad(string dllFullName)
      {
         throw new Exception(string.Format("This {0} bits process failed to load the library {1}",
                                                      (Environment.Is64BitProcess ? "64" : "32"), dllFullName));
      }

      /// <summary>
      /// Creates the delegate function for the specified function defined in the DLL.
      /// </summary>
      /// <typeparam name="TDelegate">The type of delegate.</typeparam>
      /// <returns>The delegate.</returns>
      public TDelegate GetFunction<TDelegate>()
         where TDelegate : class
      {
         Type delegateType = typeof(TDelegate);
         if (!delegateType.IsSubclassOf(typeof(Delegate)))
         {
            throw new ArgumentException();
         }
         IntPtr function = GetFunctionAddress(delegateType.Name);
         if (function == IntPtr.Zero)
         {
            throw new EntryPointNotFoundException();
         }
         return Marshal.GetDelegateForFunctionPointer(function, delegateType) as TDelegate;
      }

      /// <summary>
      /// Creates the delegate function for the specified function defined in the DLL.
      /// </summary>
      /// <typeparam name="TDelegate">The type of delegate.</typeparam>
      /// <param name="entryPoint">The name of function.</param>
      /// <returns>The delegate.</returns>
      public TDelegate GetFunction<TDelegate>(string entryPoint)
         where TDelegate : class
      {
         if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
         {
            throw new ArgumentException();
         }
         if (entryPoint == null)
         {
            throw new ArgumentNullException("entryPoint");
         }
         IntPtr function = GetFunctionAddress(entryPoint);
         if (function == IntPtr.Zero)
         {
            throw new EntryPointNotFoundException();
         }
         return Marshal.GetDelegateForFunctionPointer(function, typeof(TDelegate)) as TDelegate;
      }

      private bool IsUnix {
         get { return NativeUtility.IsUnix; }
      }

      private IntPtr GetFunctionAddress(string lpProcName)
      {
         return IsUnix ? UnixLibraryLoader.GetFunctionAddress(handle, lpProcName)
            : WindowsLibraryLoader.GetFunctionAddress(handle, lpProcName);
      }

      private bool FreeLibrary()
      {
         return IsUnix ? UnixLibraryLoader.FreeLibrary(handle)
            : WindowsLibraryLoader.FreeLibrary(handle);
      }

      /// <summary>
      /// Gets the handle of the specified entry.
      /// </summary>
      /// <param name="entryPoint">The name of function.</param>
      /// <returns>The handle.</returns>
      public IntPtr DangerousGetHandle(string entryPoint)
      {
         if (entryPoint == null)
         {
            throw new ArgumentNullException("entryPoint");
         }
         return GetFunctionAddress(entryPoint);
      }

      protected override bool ReleaseHandle()
      {
         return FreeLibrary();
      }

      protected override void Dispose(bool disposing)
      {
         if (FreeLibrary())
         {
            SetHandleAsInvalid();
         }
         base.Dispose(disposing);
      }

   }
}