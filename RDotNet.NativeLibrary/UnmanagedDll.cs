using System;
using System.Collections.Generic;
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
      private readonly IDynamicLibraryLoader libraryLoader;

      /// <summary>
      /// Gets whether the current handle is equal to the invalid handle
      /// </summary>
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

         if (NativeUtility.IsUnix)
            libraryLoader = new UnixLibraryLoader();
         else
            libraryLoader = new WindowsLibraryLoader();

         IntPtr handle = libraryLoader.LoadLibrary(dllName);
         if (handle == IntPtr.Zero)
         {
            ReportLoadLibError(dllName);
         }
         SetHandle(handle);
         DllFilename = dllName;
      }

      /// <summary>
      /// Gets the Dll file name used for this native Dll wrapper.
      /// </summary>
      public string DllFilename { get; private set; }


      private void ReportLoadLibError(string dllName)
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

            dllFullName = FindFullPath(dllName, throwIfNotFound: true);
            ThrowFailedLibraryLoad(dllFullName);
         }
      }

      private static string FindFullPath(string dllName, bool throwIfNotFound = false)
      {
         string dllFullName;
         if (File.Exists(dllName))
         {
            dllFullName = Path.GetFullPath(dllName);
            if (File.Exists(dllFullName))
               return dllFullName;
         }
         var searchPaths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
         dllFullName = searchPaths.Select(directory => Path.Combine(directory, dllName)).FirstOrDefault(File.Exists);
         if (throwIfNotFound)
            if (string.IsNullOrEmpty(dllFullName) || !File.Exists(dllFullName))
               throw new DllNotFoundException(string.Format("Could not find the library named {0} in the search paths", dllName));
         return dllFullName;
      }

      private string createLdLibPathMsg()
      {
         if (!NativeUtility.IsUnix)
            return null;
         var sampleldLibPaths = "/usr/local/lib/R/lib:/usr/local/lib:/usr/lib/jvm/java-7-openjdk-amd64/jre/lib/amd64/server";
         var ldLibPathEnv = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
         string msg = Environment.NewLine + Environment.NewLine;
         if (string.IsNullOrEmpty(ldLibPathEnv))
            msg = msg + "The environment variable LD_LIBRARY_PATH is not set.";
         else
            msg = msg + string.Format("The environment variable LD_LIBRARY_PATH is set to {0}.", ldLibPathEnv);

         msg = msg + string.Format(" For some Unix-like operating systems you may need to set it BEFORE launching the application. For instance to {0}.", sampleldLibPaths);
         msg = msg + " You can get the value as set by the R console application for your system, with the statement Sys.getenv('LD_LIBRARY_PATH'). For instance from your shell prompt:";
         msg = msg + Environment.NewLine;
         msg = msg + "Rscript -e \"Sys.getenv('LD_LIBRARY_PATH')\"";
         msg = msg + Environment.NewLine;
         msg = msg + "export LD_LIBRARY_PATH=/usr/the/paths/you/just/got/from/Rscript";
         msg = msg + Environment.NewLine + Environment.NewLine;

         return msg;
      }

      private void ThrowFailedLibraryLoad(string dllFullName)
      {
         var strMsg = string.Format("This {0}-bit process failed to load the library {1}",
                                    (Environment.Is64BitProcess ? "64" : "32"), dllFullName);
         var nativeError = libraryLoader.GetLastError();
         if (!string.IsNullOrEmpty(nativeError))
            strMsg = strMsg + string.Format(". Native error message is '{0}'", nativeError);
         var ldLibPathMsg = createLdLibPathMsg();
         if (!string.IsNullOrEmpty(ldLibPathMsg))
            strMsg = strMsg + string.Format(". {0}", ldLibPathMsg);
         throw new Exception(strMsg);
      }

      private Dictionary<string, object> delegateFunctionPointers = new Dictionary<string, object>();

      /// <summary>
      /// Creates the delegate function for the specified function defined in the DLL.
      /// </summary>
      /// <typeparam name="TDelegate">The type of delegate. The name of the native function is assumed to be the same as the delegate type name.</typeparam>
      /// <returns>The delegate.</returns>
      public TDelegate GetFunction<TDelegate>()
         where TDelegate : class
      {
         return GetFunction<TDelegate>(typeof(TDelegate).Name);
      }

      /// <summary>
      /// Creates the delegate function for the specified function defined in the DLL.
      /// </summary>
      /// <typeparam name="TDelegate">The type of delegate.</typeparam>
      /// <param name="entryPoint">The name of the function exported by the DLL</param>
      /// <returns>The delegate.</returns>
      public TDelegate GetFunction<TDelegate>(string entryPoint)
         where TDelegate : class
      {
         if (string.IsNullOrEmpty(entryPoint))
            throw new ArgumentNullException("entryPoint", "Native function name cannot be null or empty");
         Type delegateType = typeof(TDelegate);
         if (delegateFunctionPointers.ContainsKey(entryPoint))
            return (TDelegate)delegateFunctionPointers[entryPoint];
         if (!delegateType.IsSubclassOf(typeof(Delegate)))
         {
            throw new InvalidCastException();
         }
         IntPtr function = GetFunctionAddress(entryPoint);
         if (function == IntPtr.Zero)
         {
            throwEntryPointNotFound(entryPoint);
         }
         var dFunc = Marshal.GetDelegateForFunctionPointer(function, delegateType) as TDelegate;
         delegateFunctionPointers.Add(entryPoint, dFunc);
         return dFunc;
      }

      private void throwEntryPointNotFound(string entryPoint)
      {
         throw new EntryPointNotFoundException(string.Format("Function {0} not found in native library {1}", entryPoint, this.DllFilename));
      }

      private IntPtr GetFunctionAddress(string lpProcName)
      {
         return libraryLoader.GetFunctionAddress(handle, lpProcName);
      }

      private bool FreeLibrary()
      {
         bool freed = false;
         if (libraryLoader == null)
         {
            if (!this.IsInvalid)
            {
               try
               {
                  throw new ApplicationException("Warning: unexpected condition of library loader and native handle - some native resources may not be properly disposed of");
               }
               finally
               {
                  freed = false;
               }
            }
            else
               freed = true;
            return freed;
         }
         else
            return libraryLoader.FreeLibrary(handle);
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

      /// <summary>
      /// Frees the native library this objects represents
      /// </summary>
      /// <returns>The result of the call to FreeLibrary</returns>
      protected override bool ReleaseHandle()
      {
         return FreeLibrary();
      }

      /// <summary>
      /// Frees the native library this objects represents
      /// </summary>
      /// <param name="disposing"></param>
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