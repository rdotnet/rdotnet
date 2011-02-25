using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp;

namespace RDotNet.Tools
{
	class Program
	{
		static void Main(string[] args)
		{
			// Enforces messages appeared in the generated files in English.
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

			string coreDirectory = args[0];
			string deployDirectory = args[1];
			string internalsDirectory = Path.Combine(coreDirectory, "Internals");
			string[] sourceFiles = new string[] {
				Path.Combine(internalsDirectory, "INativeMethodsProxy.cs"),
				Path.Combine(internalsDirectory, "Constants.cs"),
				Path.Combine(internalsDirectory, "SymbolicExpressionType.cs"),
				Path.Combine(internalsDirectory, "ParseStatus.cs"),
				Path.Combine(internalsDirectory, "BusyType.cs"),
				Path.Combine(internalsDirectory, "ConsoleOutputType.cs"),
				Path.Combine(internalsDirectory, "SaveActions.cs"),
				Path.Combine(internalsDirectory, "YesNoCancel.cs"),
				Path.Combine(internalsDirectory, "RStart.cs"),
			};

			using (CSharpCodeProvider provider = new CSharpCodeProvider())
			{
				CompilerParameters parameters = new CompilerParameters()
				{
					GenerateExecutable = false,
					GenerateInMemory = true,
				};
				CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourceFiles);
				Assembly assembly = results.CompiledAssembly;

				CodeGeneratorOptions options = new CodeGeneratorOptions()
				{
					BlankLinesBetweenMembers = true,
					IndentString = "\t",
					ElseOnClosing = true,
					VerbatimOrder = true,
				};

				Type generatorInterface = typeof(IProxyGenerator);
				Type proxyInterface = assembly.GetType("RDotNet.Internals.INativeMethodsProxy");
				var generatorTypes = Assembly.GetAssembly(generatorInterface).GetTypes().Where(type => type.GetInterfaces().Contains(generatorInterface));
				foreach (Type generatorType in generatorTypes)
				{
					IProxyGenerator generator = (IProxyGenerator)Activator.CreateInstance(generatorType);
					string targetName = generatorType.Name.Substring(0, generatorType.Name.Length - "Generator".Length);
					string generatedPath = Path.Combine(deployDirectory, targetName + ".generated.cs");
					using (StreamWriter writer = new StreamWriter(generatedPath, false, Encoding.UTF8))
					{
						CodeCompileUnit compileUnit = generator.GenerateProxy(targetName, proxyInterface);
						provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
					}
				}
			}
		}
	}
}
