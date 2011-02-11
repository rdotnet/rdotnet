using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using RDotNet.Internals;

namespace RDotNet
{
	/// <summary>
	/// REngine handles R environment through evaluation of R statement.
	/// </summary>
	/// <example>This example generates and outputs five random numbers from standard normal distribution.
	/// <code>
	/// REngine.SetDllDirectory(@"C:\Program Files\R\R-2.12.0\bin\i386");
	/// using (REngine engine = REngine.CreateInstance("RDotNet"))
	/// {
	///	NumericVector random = engine.EagerEvaluate("rnorm(5, 0, 1)").AsNumeric();
	///	foreach (double r in random)
	///	{
	///		Console.Write(r + " ");
	///	}
	/// }
	/// </code>
	/// </example>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public class REngine : LateBoundUnmanagedDll
	{
		private static readonly IDictionary<string, REngine> instances = new Dictionary<string, REngine>();

#if !MAC && !LINUX
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr _getDLLVersion();
#endif

		/// <summary>
		/// Gets the version of R.DLL.
		/// </summary>
		public Version DllVersion
		{
			get
			{
				// As R's version definitions are defined in #define preprocessor,
				// C# cannot access them dynamically.
				// But, on Win32 platform, we can get the version string via getDLLVersion function.
#if MAC || LINUX
				throw new NotImplementedException();
#else
				var getVersion = GetFunction<_getDLLVersion>("getDLLVersion");
				string version = Marshal.PtrToStringAnsi(getVersion());
				return new Version(version);
#endif
			}
		}

		public override bool IsInvalid
		{
			get
			{
				return !IsRunning || base.IsInvalid;
			}
		}

		private bool isRunning;
		/// <summary>
		/// Gets whether this instance is running.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return isRunning;
			}
		}

		private readonly string id;
		/// <summary>
		/// Gets the ID of this instance.
		/// </summary>
		public string ID
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets the global environment.
		/// </summary>
		public RDotNet.Environment GlobalEnvironment
		{
			get
			{
				return GetPredefinedSymbol("R_GlobalEnv").AsEnvironment();
			}
		}

		/// <summary>
		/// Gets the root environment.
		/// </summary>
		public RDotNet.Environment EmptyEnvironment
		{
			get
			{
				return GetPredefinedSymbol("R_EmptyEnv").AsEnvironment();
			}
		}

		/// <summary>
		/// Gets the <c>NULL</c> value.
		/// </summary>
		public SymbolicExpression NilValue
		{
			get
			{
				return GetPredefinedSymbol("R_NilValue");
			}
		}

		/// <summary>
		/// Gets the unbound value.
		/// </summary>
		public SymbolicExpression UnboundValue
		{
			get
			{
				return GetPredefinedSymbol("R_UnboundValue");
			}
		}
		
		private readonly INativeMethodsProxy proxy;
		internal INativeMethodsProxy Proxy
		{
			get
			{
				return proxy;
			}
		}

		private REngine(string id, string[] args)
			: base(NativeMethods.RDllName)
		{
			this.id = id;
			this.proxy = GetDefaultProxy();
			
			string[] newArgs = Utility.AddFirst(id, args);
			Proxy.Rf_initEmbeddedR(newArgs.Length, newArgs);

			isRunning = true;
		}
		
		private INativeMethodsProxy GetDefaultProxy()
		{
			OperatingSystem os = System.Environment.OSVersion;
			switch (os.Platform)
			{
				case PlatformID.Win32NT:
					return DirectNativeMethods.Instance;
				case PlatformID.MacOSX:
				case PlatformID.Unix:
					return new DelegateNativeMethods(this);
				default:
					throw new NotSupportedException(os.ToString());
			}
		}

		/// <summary>
		/// Creates a new instance that handles R.DLL.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="args">Arguments for initializing.</param>
		/// <returns>The engine.</returns>
		public static REngine CreateInstance(string id, params string[] args)
		{
			if (id == null)
			{
				throw new ArgumentNullException();
			}
			if (id == string.Empty)
			{
				throw new ArgumentException();
			}
			if (instances.ContainsKey(id))
			{
				throw new ArgumentException();
			}

			REngine engine = new REngine(id, args);
			instances.Add(id, engine);

			return engine;
		}

		/// <summary>
		/// Gets an instance specified in the given ID.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <returns>The engine.</returns>
		public static REngine GetInstanceFromID(string id)
		{
			return instances.ContainsKey(id) ? instances[id] : null;
		}

		/// <summary>
		/// Gets a symbol defined in the global environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The symbol.</returns>
		public SymbolicExpression GetSymbol(string name)
		{
			return GlobalEnvironment.GetSymbol(name);
		}

		/// <summary>
		/// Gets a symbol defined in the global environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
		/// <returns>The symbol.</returns>
		public SymbolicExpression GetSymbol(string name, RDotNet.Environment environment)
		{
			if (environment == null)
			{
				environment = GlobalEnvironment;
			}
			return environment.GetSymbol(name);
		}

		/// <summary>
		/// Defines a symbol in the global environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="expression">The symbol.</param>
		public void SetSymbol(string name, SymbolicExpression expression)
		{
			GlobalEnvironment.SetSymbol(name, expression);
		}

		/// <summary>
		/// Defines a symbol in the specified environment.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="expression">The symbol.</param>
		/// <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
		public void SetSymbol(string name, SymbolicExpression expression, RDotNet.Environment environment)
		{
			if (environment == null)
			{
				environment = GlobalEnvironment;
			}
			environment.SetSymbol(name, expression);
		}

		/// <summary>
		/// Evaluates a statement in the given string.
		/// </summary>
		/// <param name="statement">The statement.</param>
		/// <returns>Last evaluation.</returns>
		public SymbolicExpression EagerEvaluate(string statement)
		{
			return Evaluate(statement).Last();
		}

		/// <summary>
		/// Evaluates a statement in the given stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Last evaluation.</returns>
		public SymbolicExpression EagerEvaluate(Stream stream)
		{
			return Evaluate(stream).Last();
		}

		/// <summary>
		/// Evaluates a statement in the given string.
		/// </summary>
		/// <param name="statement">The statement.</param>
		/// <returns>Each evaluation.</returns>
		public IEnumerable<SymbolicExpression> Evaluate(string statement)
		{
			if (statement == null)
			{
				throw new ArgumentNullException();
			}

			using (TextReader reader = new StringReader(statement))
			{
				StringBuilder incompleteStatement = new StringBuilder();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					foreach (string segment in Segment(line))
					{
						SymbolicExpression result = Parse(segment, 1, incompleteStatement);
						if (result != null)
						{
							yield return result;
						}
					}
				}
			}
		}

		/// <summary>
		/// Evaluates a statement in the given stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Each evaluation.</returns>
		public IEnumerable<SymbolicExpression> Evaluate(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException();
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException();
			}

			using (TextReader reader = new StreamReader(stream))
			{
				StringBuilder incompleteStatement = new StringBuilder();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					foreach (string segment in Segment(line))
					{
						SymbolicExpression result = Parse(segment, 1, incompleteStatement);
						if (result != null)
						{
							yield return result;
						}
					}
				}
			}
		}

		private static IEnumerable<string> Segment(string line)
		{
			string[] segments = line.Split(';');
			for (int index = 0; index < segments.Length; index++)
			{
				if (index == segments.Length - 1)
				{
					if (segments[index] != string.Empty)
					{
						yield return segments[index];
					}
				}
				else
				{
					yield return segments[index] + ";";
				}
			}
		}

		private SymbolicExpression Parse(string statement, int statementCount, StringBuilder incompleteStatement)
		{
			incompleteStatement.Append(statement);
			IntPtr s = Proxy.Rf_mkString(incompleteStatement.ToString());

			using (new ProtectedPointer(this, s))
			{
				ParseStatus status;
				ExpressionVector vector = new ExpressionVector(this, Proxy.R_ParseVector(s, -1, out status, NilValue.DangerousGetHandle()));

				switch (status)
				{
					case ParseStatus.OK:
						incompleteStatement.Clear();
						using (new ProtectedPointer(vector))
						{
							SymbolicExpression result;
							if (!vector.First().TryEvaluate(GlobalEnvironment, out result))
							{
								throw new ParseException();
							}
							return result;
						}
					case ParseStatus.Incomplete:
						return null;
					default:
						string errorStatement = incompleteStatement.ToString();
						incompleteStatement.Clear();
						throw new ParseException(status, errorStatement);
				}
			}
		}

		protected override bool ReleaseHandle()
		{
			Proxy.Rf_endEmbeddedR(0);
			return base.ReleaseHandle();
		}

		protected override void Dispose(bool disposing)
		{
			isRunning = false;
			if (ID != null && instances.ContainsKey(ID))
			{
				instances.Remove(ID);
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets the predefined symbol with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The symbol.</returns>
		public SymbolicExpression GetPredefinedSymbol(string name)
		{
			try
			{
				IntPtr pointer = GetSymbolPointer(this.handle, name);
				return new SymbolicExpression(this, Marshal.ReadIntPtr(pointer));
			}
			catch (Exception ex)
			{
				throw new ArgumentException(null, ex);
			}
		}

#if MAC
		[DllImport("libdl.dylib", EntryPoint = "dlsym")]
#elif LINUX
		[DllImport("libdl.so", EntryPoint = "dlsym")]
#else
		[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
#endif
		private static extern IntPtr GetSymbolPointer(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);
	}
}
