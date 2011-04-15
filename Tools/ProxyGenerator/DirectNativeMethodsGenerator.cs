using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace RDotNet.Tools
{
	public class DirectNativeMethodsGenerator : IProxyGenerator
	{
		public CodeCompileUnit GenerateProxy(string proxyTypeName, Type proxyInterface)
		{
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			CodeNamespace name = new CodeNamespace(proxyInterface.Namespace);
			compileUnit.Namespaces.Add(name);

			CodeTypeDeclaration type = new CodeTypeDeclaration(proxyTypeName);
			type.TypeAttributes = TypeAttributes.Class;
			type.IsPartial = true;
			name.Types.Add(type);

			foreach (var method in proxyInterface.GetMethods())
			{
				CodeSnippetTypeMember m = CreateExternMethod(method);
				type.Members.Add(m);

				CodeMemberMethod proxy = CreateImplementation(proxyTypeName, method);
				type.Members.Add(proxy);

				foreach (ParameterInfo parameter in method.GetParameters())
				{
					AddParameterForImplementation(proxy, parameter);
				}
			}

			return compileUnit;
		}

		private static CodeMemberMethod CreateImplementation(string proxyTypeName, MethodInfo method)
		{
			CodeMemberMethod proxy = new CodeMemberMethod();
			proxy.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			proxy.Name = method.Name;
			proxy.ReturnType = new CodeTypeReference(method.ReturnType);

			CodeExpression[] parameters = method.GetParameters().Select(
				parameter =>
				{
					const string dummy = " ";  // Bad way to delete type declaration.
					CodeParameterDeclarationExpression argument = new CodeParameterDeclarationExpression(dummy, parameter.Name);
					if (parameter.IsOut)
					{
						argument.Direction = FieldDirection.Out;
					}
					else if (parameter.ParameterType.IsByRef)
					{
						argument.Direction = FieldDirection.Ref;
					}
					return argument;
				}
			).ToArray();
			CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
					new CodeTypeReferenceExpression(proxyTypeName),
					"_" + method.Name
				),
				parameters
			);
			if (method.ReturnType == typeof(void))
			{
				proxy.Statements.Add(new CodeExpressionStatement(invoke));
			}
			else
			{
				proxy.Statements.Add(new CodeMethodReturnStatement(invoke));
			}

			return proxy;
		}

		private static CodeSnippetTypeMember CreateExternMethod(MethodInfo method)
		{
			string declaration = string.Format(@"[System.Runtime.InteropServices.DllImportAttribute(Constants.RDllName, CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = ""{0}"")]
private static extern {1} _{0}({2});",
				method.Name,
				method.ReturnType.GetReturnTypeString(),
				string.Join(", ", method.GetParameters().Select(parameter => GetParameterForExternMethod(parameter)))
			);
			CodeSnippetTypeMember m = new CodeSnippetTypeMember(declaration);
			CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(DllImportAttribute)));
			attribute.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("RDotNet.Internals.Constants"), "RDllName")));
			attribute.Arguments.Add(new CodeAttributeArgument("CallingConvention", new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(typeof(CallingConvention).FullName), CallingConvention.Cdecl.ToString())));
			attribute.Arguments.Add(new CodeAttributeArgument("EntryPoint", new CodePrimitiveExpression(method.Name)));
			m.CustomAttributes.Add(attribute);
			return m;
		}

		private static string GetParameterForExternMethod(ParameterInfo parameter)
		{
			StringBuilder builder = new StringBuilder();
			if (parameter.ParameterType == typeof(bool))
			{
				builder.Append("[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] ");
			}
			else if (parameter.ParameterType == typeof(string))
			{
				builder.Append("[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)] ");
			}
			else if (parameter.ParameterType == typeof(string[]))
			{
				builder.Append("[System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPArray, ArraySubType=System.Runtime.InteropServices.UnmanagedType.LPStr)] ");
			}

			if (parameter.IsOut)
			{
				builder.Append("out ");
			}
			else if (parameter.ParameterType.IsByRef)
			{
				builder.Append("ref ");
			}

			builder.Append(parameter.ParameterType.GetFullNameWithoutDirection()).Append(' ');
			builder.Append(parameter.Name);

			return builder.ToString();
		}

		private static void AddParameterForImplementation(CodeMemberMethod proxy, ParameterInfo parameter)
		{
			CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(parameter.ParameterType.GetFullNameWithoutDirection(), parameter.Name);
			if (parameter.IsOut)
			{
				p.Direction = FieldDirection.Out;
			}
			else if (parameter.ParameterType.IsByRef)
			{
				p.Direction = FieldDirection.Ref;
			}
			proxy.Parameters.Add(p);
		}
	}
}
