using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using RDotNet.Devices;
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

#if WINDOWS
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
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
				// As R's version definitions are defined in #define preprocessor,
				// C# cannot access them dynamically.
				// But, on Win32 platform, we can get the version string via getDLLVersion function.
#if MAC || LINUX
				throw new NotImplementedException();
#elif WINDOWS
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

		/// <summary>
		/// Gets whether this instance is running.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return Proxy != null;
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
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
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
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
				return GetPredefinedSymbol("R_EmptyEnv").AsEnvironment();
			}
		}

		/// <summary>
		/// Gets the base environment.
		/// </summary>
		public RDotNet.Environment BaseNamespace
		{
			get
			{
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
				return GetPredefinedSymbol("R_BaseNamespace").AsEnvironment();
			}
		}

		/// <summary>
		/// Gets the <c>NULL</c> value.
		/// </summary>
		public SymbolicExpression NilValue
		{
			get
			{
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
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
				if (IsInvalid)
				{
					throw new InvalidOperationException();
				}
				return GetPredefinedSymbol("R_UnboundValue");
			}
		}

		private INativeMethodsProxy proxy;
		internal INativeMethodsProxy Proxy
		{
			get
			{
				return proxy;
			}
		}

		private CharacterDeviceAdapter adapter;

		private REngine(string id, string[] args)
			: base(Constants.RDllName)
		{
			this.id = id;
			this.proxy = GetDefaultProxy();
			this.adapter = null;

			string[] newArgs = Utility.AddFirst(id, args ?? new string[0]);
			Proxy.Rf_initEmbeddedR(newArgs.Length, newArgs);
		}

		private REngine(string id, string[] args, CharacterDeviceAdapter adapter)
			: base(Constants.RDllName)
		{

			this.id = id;
			this.proxy = GetDefaultProxy();
			this.adapter = adapter;

			string[] newArgs = Utility.AddFirst(id, args ?? new string[0]);
			int argc = newArgs.Length;

			Proxy.R_setStartTime();
			Proxy.Rf_initialize_R(argc, newArgs);
			RStart start;
			Proxy.R_DefParams(out start);
			adapter.Install(this, ref start);
			Proxy.R_common_command_line(ref argc, newArgs, ref start);
			Proxy.R_set_command_line_arguments(newArgs.Length, newArgs);
			Proxy.R_SetParams(ref start);
			Proxy.setup_Rmainloop();
		}

		private REngine(string id, OutputMode output, CharacterDeviceAdapter adapter)
			: base(Constants.RDllName)
		{
			this.id = id;
			this.proxy = GetDefaultProxy();
			this.adapter = adapter;

			Proxy.R_setStartTime();
			Proxy.Rf_initialize_R(1, new string[] { id });
			RStart start;
			Proxy.R_DefParams(out start);
			adapter.Install(this, ref start);
			start.R_Quiet = (output & OutputMode.Quiet) == OutputMode.Quiet;
			start.R_Slave = (output & OutputMode.Slave) == OutputMode.Slave;
			start.R_Verbose = (output & OutputMode.Verbose) == OutputMode.Verbose;
			Proxy.R_SetParams(ref start);
			Proxy.setup_Rmainloop();
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
		/// <param name="device">The IO device.</param>
		/// <returns>The engine.</returns>
		[Obsolete]
		public static REngine CreateInstance(string id, string[] args, ICharacterDevice device = null)
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

			CharacterDeviceAdapter adapter = new CharacterDeviceAdapter(device ?? new ConsoleDevice());
			REngine engine = new REngine(id, args, adapter);
			instances.Add(id, engine);

			return engine;
		}

		/// <summary>
		/// Creates a new instance that handles R.DLL.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="output">The output mode.</param>
		/// <param name="device">The IO device.</param>
		/// <returns>The engine.</returns>
		public static REngine CreateInstance(string id, OutputMode output = OutputMode.None, ICharacterDevice device = null)
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

			CharacterDeviceAdapter adapter = new CharacterDeviceAdapter(device ?? new ConsoleDevice());
			REngine engine = new REngine(id, output, adapter);
			instances.Add(id, engine);

			return engine;
		}

		/// <summary>
		/// Creates a new instance that handles R.DLL.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="device">The IO device.</param>
		/// <returns>The engine.</returns>
		public static REngine CreateInstance(string id, ICharacterDevice device)
		{
			return CreateInstance(id, OutputMode.None, device);
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			return Evaluate(statement).LastOrDefault();
		}

		/// <summary>
		/// Evaluates a statement in the given stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Last evaluation.</returns>
		public SymbolicExpression EagerEvaluate(Stream stream)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			return Evaluate(stream).LastOrDefault();
		}

		/// <summary>
		/// Evaluates a statement in the given string.
		/// </summary>
		/// <param name="statement">The statement.</param>
		/// <returns>Each evaluation.</returns>
		public IEnumerable<SymbolicExpression> Evaluate(string statement)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
						SymbolicExpression result = Parse(segment, incompleteStatement);
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
						SymbolicExpression result = Parse(segment, incompleteStatement);
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

		private SymbolicExpression Parse(string statement, StringBuilder incompleteStatement)
		{
			incompleteStatement.Append(statement);
			IntPtr s = Proxy.Rf_mkString(incompleteStatement.ToString());

			using (new ProtectedPointer(this, s))
			{
				ParseStatus status;
				ExpressionVector vector = new ExpressionVector(this, Proxy.R_ParseVector(s, -1, out status, NilValue.DangerousGetHandle()));
				if (vector.Length == 0)
				{
					return null;
				}

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

		/// <summary>
		/// Sets the command line arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		public void SetCommandLineArguments(string[] args)
		{
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
			string[] newArgs = Utility.AddFirst(ID, args);
			Proxy.R_set_command_line_arguments(newArgs.Length, newArgs);
		}

		protected override void Dispose(bool disposing)
		{
			if (instances.ContainsKey(ID))
			{
				instances.Remove(ID);
			}

			if (IsRunning)
			{
				if (disposing)
				{
					Proxy.Rf_endEmbeddedR(0);
				}
				proxy = null;
				if (adapter != null)
				{
					adapter.Dispose();
					adapter = null;
				}
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
			if (IsInvalid)
			{
				throw new InvalidOperationException();
			}
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
#elif WINDOWS
		[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
#endif
		private static extern IntPtr GetSymbolPointer(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string symbol);
	}
}
