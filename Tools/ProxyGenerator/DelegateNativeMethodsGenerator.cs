using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace RDotNet.Tools
{
	public class DelegateNativeMethodsGenerator : IProxyGenerator
	{
		public CodeCompileUnit GenerateProxy(string proxyTypeName, Type proxyInterface)
		{
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			CodeNamespace name = new CodeNamespace("RDotNet.Internals");
			compileUnit.Namespaces.Add(name);

			CodeTypeDeclaration type = new CodeTypeDeclaration(proxyTypeName);
			type.TypeAttributes = TypeAttributes.Class;
			type.IsPartial = true;
			name.Types.Add(type);

			foreach (var method in proxyInterface.GetMethods())
			{
				CodeTypeDelegate d = CreateDelegate(method);
				type.Members.Add(d);
				
				CodeMemberMethod proxy = CreateImplementation(d, method);
				type.Members.Add(proxy);

				foreach (ParameterInfo parameter in method.GetParameters())
				{
					AddParameterForDelegate(d, parameter);
					AddParameterForImplementation(proxy, parameter);
				}
			}

			return compileUnit;
		}

		private static CodeMemberMethod CreateImplementation(CodeTypeDelegate d, MethodInfo method)
		{
			CodeMemberMethod proxy = new CodeMemberMethod();
			const string name = "function";
			proxy.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			proxy.Name = method.Name;
			proxy.ReturnType = new CodeTypeReference(method.ReturnType);
			CodeVariableDeclarationStatement function = new CodeVariableDeclarationStatement(
				"var", name,
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "dll"),
						"GetFunction",
						new CodeTypeReference[] { new CodeTypeReference(d.Name) }
					),
					new CodePrimitiveExpression(method.Name)
				)
			);
			proxy.Statements.Add(function);

			CodeExpression[] parameters = method.GetParameters().Select(
				parameter =>
				{
					const string dummy = " ";  // Bad way to delete type declaration.
					CodeParameterDeclarationExpression argument = new CodeParameterDeclarationExpression(dummy, parameter.Name);
					if (parameter.IsOut)
					{
						argument.Direction = FieldDirection.Out;
					}
					return argument;
				}
			).ToArray();
			CodeDelegateInvokeExpression invoke = new CodeDelegateInvokeExpression(
				new CodeVariableReferenceExpression(name),
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

		private static CodeTypeDelegate CreateDelegate(MethodInfo method)
		{
			CodeTypeDelegate d = new CodeTypeDelegate("_" + method.Name);
			d.TypeAttributes = TypeAttributes.NotPublic | TypeAttributes.NestedPrivate;
			d.ReturnType = new CodeTypeReference(method.ReturnType);
			CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(UnmanagedFunctionPointerAttribute)));
			CodeAttributeArgument attributeArgument = new CodeAttributeArgument(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(typeof(CallingConvention).FullName), CallingConvention.Cdecl.ToString()));
			attribute.Arguments.Add(attributeArgument);
			d.CustomAttributes.Add(attribute);
			return d;
		}

		private static void AddParameterForDelegate(CodeTypeDelegate d, ParameterInfo parameter)
		{
			CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(parameter.ParameterType.GetFullNameWithoutDirection(), parameter.Name);
			if (parameter.IsOut)
			{
				p.Direction = FieldDirection.Out;
			}
			if (parameter.ParameterType == typeof(bool))
			{
				CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(MarshalAsAttribute)));
				attribute.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(typeof(UnmanagedType).FullName), UnmanagedType.Bool.ToString())));
				p.CustomAttributes.Add(attribute);
			}
			else if (parameter.ParameterType == typeof(string))
			{
				CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(MarshalAsAttribute)));
				attribute.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(typeof(UnmanagedType).FullName), UnmanagedType.LPStr.ToString())));
				p.CustomAttributes.Add(attribute);
			}
			else if (parameter.ParameterType == typeof(string[]))
			{
				CodeAttributeDeclaration attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(MarshalAsAttribute)));
				attribute.Arguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeVariableReferenceExpression(typeof(UnmanagedType).FullName), UnmanagedType.LPArray.ToString())));
				attribute.Arguments.Add(new CodeAttributeArgument("ArraySubType", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(typeof(UnmanagedType).FullName), UnmanagedType.LPStr.ToString())));
				p.CustomAttributes.Add(attribute);
			}
			d.Parameters.Add(p);
		}

		private static void AddParameterForImplementation(CodeMemberMethod proxy, ParameterInfo parameter)
		{
			CodeParameterDeclarationExpression p = new CodeParameterDeclarationExpression(parameter.ParameterType.GetFullNameWithoutDirection(), parameter.Name);
			if (parameter.IsOut)
			{
				p.Direction = FieldDirection.Out;
			}
			proxy.Parameters.Add(p);
		}
	}
}
