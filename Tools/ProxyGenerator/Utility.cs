using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RDotNet.Tools
{
	internal static class Utility
	{
		public const string CodeFileExtension = ".cs";
		public const string GeneratedCodeFileExtension = ".generated" + CodeFileExtension;

		public static string GetFullNameWithoutDirection(this Type type)
		{
			string name = type.FullName;
			if (name.EndsWith("&"))
			{
				return name.Substring(0, name.Length - 1);
			}
			return name;
		}

		public static string GetReturnTypeString(this Type returnType)
		{
			return returnType == typeof(void) ? "void" : returnType.FullName;
		}
	}
}
