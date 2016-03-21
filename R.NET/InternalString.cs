using RDotNet.Internals;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace RDotNet
{
    /// <summary>
    /// Internal string.
    /// </summary>
    [DebuggerDisplay("Content = {ToString()}; RObjectType = {Type}")]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]

    public class InternalString : SymbolicExpression
    {
        /// <summary>
        /// Convert string to utf8
        /// </summary>
        /// <param name="stringToConvert">string to convert</param>

        public static IntPtr NativeUtf8FromString(string stringToConvert)
        {
          int len = Encoding.UTF8.GetByteCount(stringToConvert);
          byte[] buffer = new byte[len + 1];
          Encoding.UTF8.GetBytes(stringToConvert, 0, stringToConvert.Length, buffer, 0);
          IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
          Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
          return nativeUtf8;
        }

        /// <summary>
        /// Convert utf8 to string
        /// </summary>
        /// <param name="utf8">utf8 to convert</param>

        public static string StringFromNativeUtf8(IntPtr utf8)
        {
          int len = 0;
          while (Marshal.ReadByte(utf8, len) != 0) ++len;
          byte[] buffer = new byte[len];
          Marshal.Copy(utf8, buffer, 0, buffer.Length);
          return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="pointer">The pointer to a string.</param>

        public InternalString(REngine engine, IntPtr pointer)
            : base(engine, pointer)
        { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="engine">The <see cref="REngine"/> handling this instance.</param>
        /// <param name="s">The string</param>
        public InternalString(REngine engine, string s)
            : base(engine, engine.GetFunction<Rf_mkChar>()(s))
        { }

        /// <summary>
        /// Converts to the string into .NET Framework string.
        /// </summary>
        /// <param name="s">The R string.</param>
        /// <returns>The .NET Framework string.</returns>
        public static implicit operator string(InternalString s)
        {
            return s.ToString();
        }

        /// <summary>
        /// Gets the string representation of the string object.
        /// This returns <c>"NA"</c> if the value is <c>NA</c>, whereas <see cref="GetInternalValue()"/> returns <c>null</c>.
        /// </summary>
        /// <returns>The string representation.</returns>
        /// <seealso cref="GetInternalValue()"/>
        public override string ToString()
        {
            IntPtr pointer = IntPtr.Add(handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            return StringFromNativeUtf8(pointer);
        }

        /// <summary>
        /// Gets the string representation of the string object.
        /// This returns <c>null</c> if the value is <c>NA</c>, whereas <see cref="ToString()"/> returns <c>"NA"</c>.
        /// </summary>
        /// <returns>The string representation.</returns>
        public string GetInternalValue()
        {
            if (handle == Engine.NaStringPointer)
            {
                return null;
            }
            IntPtr pointer = IntPtr.Add(handle, Marshal.SizeOf(typeof(VECTOR_SEXPREC)));
            return StringFromNativeUtf8(pointer);
        }
    }
}