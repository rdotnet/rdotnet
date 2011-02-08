using System;

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
	}
}
