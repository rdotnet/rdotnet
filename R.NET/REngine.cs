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

		private readonly IDictionary<string, SymbolicExpression> predefinedExpressions;

		/// <summary>
		/// Gets the version of R.DLL.
		/// </summary>
		public static Version DllVersion
		{
			get
			{
				return new Version(NativeMethods.GetDllVersion());
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
		public SymbolicExpression GlobalEnvironment
		{
			get
			{
				return CallPredefinedExpression("R_GlobalEnv");
			}
		}

		/// <summary>
		/// Gets the <c>NULL</c> value.
		/// </summary>
		public SymbolicExpression NilValue
		{
			get
			{
				return CallPredefinedExpression("R_NilValue");
			}
		}

		/// <summary>
		/// Gets the unbound value.
		/// </summary>
		public SymbolicExpression UnboundValue
		{
			get
			{
				return CallPredefinedExpression("R_UnboundValue");
			}
		}

		private REngine(string id, string[] args)
			: base(NativeMethods.RDllName)
		{
			this.id = id;
			string[] newArgs = Utility.AddFirst(id, args);
			NativeMethods.Rf_initEmbeddedR(newArgs.Length, newArgs);

			predefinedExpressions = new Dictionary<string, SymbolicExpression>();
			isRunning = true;
		}

		/// <summary>
		/// Creates a new instance that handles R.DLL.
		/// </summary>
		/// <param name="id">ID.</param>
		/// <param name="args">Arguments for initializing.</param>
		/// <returns>Instance.</returns>
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
		/// <param name="id"></param>
		/// <returns></returns>
		public static REngine GetInstanceFromID(string id)
		{
			return instances.ContainsKey(id) ? instances[id] : null;
		}

		/// <summary>
		/// Gets a symbol defined in R environment.
		/// </summary>
		/// <param name="name">Symbol name you want.</param>
		/// <returns>Symbol.</returns>
		public SymbolicExpression GetSymbol(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			if (name == string.Empty)
			{
				throw new ArgumentException();
			}

			IntPtr installedName = NativeMethods.Rf_install(name);
			IntPtr value = NativeMethods.Rf_findVar(installedName, (IntPtr)GlobalEnvironment);
			if (value == (IntPtr)UnboundValue)
			{
				return null;
			}

			return new SymbolicExpression(this, value);
		}

		/// <summary>
		/// Defines a symbol in R environment.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="expression"></param>
		public void SetSymbol(string name, SymbolicExpression expression)
		{
			IntPtr installedName = NativeMethods.Rf_install(name);
			NativeMethods.Rf_setVar(installedName, (IntPtr)expression, (IntPtr)GlobalEnvironment);
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
		/// <param name="statement">The statement.</param>
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
		/// <param name="statement">The statement.</param>
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
			IntPtr s = NativeMethods.Rf_mkString(incompleteStatement.ToString());

			using (new ProtectedPointer(s))
			{
				ParseStatus status;
				IntPtr vector = NativeMethods.R_ParseVector(s, statementCount, out status, (IntPtr)NilValue);

				switch (status)
				{
					case ParseStatus.OK:
						incompleteStatement.Clear();
						using (new ProtectedPointer(vector))
						{
							SEXPREC sexp = (SEXPREC)Marshal.PtrToStructure(vector, typeof(SEXPREC));
							bool errorOccurred;
							IntPtr result = NativeMethods.R_tryEval(sexp.listsxp.tagval, (IntPtr)GlobalEnvironment, out errorOccurred);
							if (errorOccurred)
							{
								throw new ParseException();
							}
							return new SymbolicExpression(this, result);
						}
					case ParseStatus.Incomplete:
						return null;
					default:
						incompleteStatement.Clear();
						throw new ParseException(status);
				}
			}
		}

		protected override bool ReleaseHandle()
		{
			NativeMethods.Rf_endEmbeddedR(0);
			return base.ReleaseHandle();
		}

		protected override void Dispose(bool disposing)
		{
			isRunning = false;
			if (ID != null && instances.ContainsKey(ID))
			{
				instances.Remove(ID);
			}

			if (predefinedExpressions != null)
			{
				foreach (SymbolicExpression e in predefinedExpressions.Values)
				{
					e.Dispose();
				}
				predefinedExpressions.Clear();
			}

			base.Dispose(disposing);
		}

		private SymbolicExpression CallPredefinedExpression(string name)
		{
			if (!predefinedExpressions.ContainsKey(name))
			{
				IntPtr pointer = GetProcAddress(this.handle, name);
				SymbolicExpression expression = new SymbolicExpression(this, Marshal.ReadIntPtr(pointer));
				predefinedExpressions.Add(name, expression);
			}
			return predefinedExpressions[name];
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
	}
}
