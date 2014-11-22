using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RDotNet.R.Adapter
{
    public class DelegateCache
    {
        private readonly IPlatformManager _loader;
        private readonly Dictionary<string, object> _delegatePointers = new Dictionary<string, object>();

        public DelegateCache(IPlatformManager loader)
        {
            if (loader == null) throw new ArgumentNullException("loader");
            _loader = loader;
        }

        public T GetFunction<T>() where T : class
        {
            return GetFunction<T>(typeof(T).Name);
        }

        public T GetFunction<T>(string entryPoint) where T : class
        {
            if (string.IsNullOrEmpty(entryPoint)) throw new ArgumentException("entryPoint");
            if (!typeof(T).IsSubclassOf(typeof(Delegate))) throw new ArgumentException("Class is not of type delegate");

            object function;
            var found = _delegatePointers.TryGetValue(entryPoint, out function);
            if (found) return (T)function;

            var ptr = _loader.GetProcAddress(entryPoint);
            if (ptr == IntPtr.Zero)
            {
                var msg = string.Format("Function {0} not found in R subsystem.", entryPoint);
                throw new EntryPointNotFoundException(msg);
            }

            function = Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
            _delegatePointers.Add(entryPoint, function);
            return (T)function;
        }
    }
}
