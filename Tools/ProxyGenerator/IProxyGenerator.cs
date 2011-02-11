using System;
using System.CodeDom;

namespace RDotNet.Tools
{
	public interface IProxyGenerator
	{
		CodeCompileUnit GenerateProxy(string proxyTypeName, Type proxyInterface);
	}
}
