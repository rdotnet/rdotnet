using System;
using System.Text;

namespace RDotNet
{
	internal static class Utility
	{
		public static T[] AddFirst<T>(T value, T[] array)
		{
			T[] newArray = new T[array.Length + 1];
			newArray[0] = value;
			Array.Copy(array, 0, newArray, 1, array.Length);
			return newArray;
		}

		// IntPtr.Add(IntPtr, int) in .NET 4.
		public static IntPtr OffsetPointer(IntPtr pointer, int offset)
		{
			return new IntPtr(pointer.ToInt64() + offset);
		}

		// StringBuilder.Clear() in .NET 4.
		public static void Clear(this StringBuilder builder)
		{
			builder.Length = 0;
		}
	}
}
