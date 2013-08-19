using RDotNet.Devices;
using RDotNet.Internals;
using RDotNet.NativeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace RDotNet
{
   /// <summary>
   /// REngine handles R environment through evaluation of R statement.
   /// </summary>
   /// <example>This example generates and outputs five random numbers from standard normal distribution.
   /// <code>
   /// Environment.SetEnvironmentVariable("PATH", @"C:\Program Files\R\R-2.12.0\bin\i386");
   /// using (REngine engine = REngine.CreateInstance("RDotNet"))
   /// {
   ///   engine.Initialize();
   ///   NumericVector random = engine.Evaluate("rnorm(5, 0, 1)").AsNumeric();
   ///   foreach (double r in random)
   ///   {
   ///     Console.Write(r + " ");
   ///   }
   /// }
   /// </code>
   /// </example>
   [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
   public class REngine : UnmanagedDll
   {
      private static readonly ICharacterDevice DefaultDevice = new ConsoleDevice();
      private static readonly Dictionary<string, REngine> instances = new Dictionary<string, REngine>();

      private readonly string id;
      private CharacterDeviceAdapter adapter;
      private bool isRunning;
      private StartupParameter parameter;

      private REngine(string id, string dll)
         : base(dll)
      {
         this.id = id;
         this.isRunning = false;
      }

      /// <summary>
      /// Gets whether this instance is running.
      /// </summary>
      public bool IsRunning
      {
         get { return this.isRunning; }
      }

      /// <summary>
      /// Gets the version of R.DLL.
      /// </summary>
      public string DllVersion
      {
         get
         {
            // As R's version definitions are defined in #define preprocessor,
            // C# cannot access them dynamically.
            // But, on Win32 platform, we can get the version string via getDLLVersion function.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
               throw new NotImplementedException();
            }
            var getVersion = GetFunction<_getDLLVersion>("getDLLVersion");
            return Marshal.PtrToStringAnsi(getVersion());
         }
      }

      /// <summary>
      /// Gets the ID of this instance.
      /// </summary>
      public string ID
      {
         get { return this.id; }
      }

      /// <summary>
      /// Gets the global environment.
      /// </summary>
      public REnvironment GlobalEnvironment
      {
         get
         {
            if (!IsRunning)
            {
               throw new InvalidOperationException();
            }
            return GetPredefinedSymbol("R_GlobalEnv").AsEnvironment();
         }
      }

      /// <summary>
      /// Gets the root environment.
      /// </summary>
      public REnvironment EmptyEnvironment
      {
         get
         {
            if (!IsRunning)
            {
               throw new InvalidOperationException();
            }
            return GetPredefinedSymbol("R_EmptyEnv").AsEnvironment();
         }
      }

      /// <summary>
      /// Gets the base environment.
      /// </summary>
      public REnvironment BaseNamespace
      {
         get
         {
            if (!IsRunning)
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
            if (!IsRunning)
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
            if (!IsRunning)
            {
               throw new InvalidOperationException();
            }
            return GetPredefinedSymbol("R_UnboundValue");
         }
      }

      /// <summary>
      /// Creates a new instance that handles R.DLL.
      /// </summary>
      /// <param name="id">ID.</param>
      /// <param name="dll">The core dll of R.</param>
      /// <returns>The engine.</returns>
      public static REngine CreateInstance(string id, string dll = null)
      {
         if (id == null)
         {
            throw new ArgumentNullException("id", "Empty ID is not allowed.");
         }
         if (id == string.Empty)
         {
            throw new ArgumentException("Empty ID is not allowed.", "id");
         }
         if (instances.ContainsKey(id))
         {
            throw new ArgumentException();
         }
         if (dll == null)
         {
            switch (NativeUtility.GetPlatform())
            {
               case PlatformID.Win32NT:
                  dll = "R.dll";
                  break;

               case PlatformID.MacOSX:
                  dll = "libR.dylib";
                  break;

               case PlatformID.Unix:
                  dll = "libR.so";
                  break;

               default:
                  throw new NotSupportedException();
            }
         }
         var engine = new REngine(id, dll);
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
         REngine engine;
         instances.TryGetValue(id, out engine);
         return engine;
      }

      /// <summary>
      /// Initializes R process.
      /// </summary>
      /// <param name="parameter">The startup parameter.</param>
      /// <param name="device">The IO device.</param>
      public void Initialize(StartupParameter parameter = null, ICharacterDevice device = null)
      {
         this.parameter = parameter ?? new StartupParameter();
         this.adapter = new CharacterDeviceAdapter(device ?? DefaultDevice);
         GetFunction<R_setStartTime>()();
         GetFunction<Rf_initialize_R>()(1, new[] { ID });
         this.adapter.Install(this, this.parameter);
         switch (Environment.OSVersion.Platform)
         {
            case PlatformID.Win32NT:
               GetFunction<R_SetParams_Windows>("R_SetParams")(ref this.parameter.start);
               break;

            case PlatformID.MacOSX:
            case PlatformID.Unix:
               GetFunction<R_SetParams_Unix>("R_SetParams")(ref this.parameter.start.Common);
               break;
         }
         GetFunction<setup_Rmainloop>()();
         this.isRunning = true;
      }

      /// <summary>
      /// Forces garbage collection.
      /// </summary>
      public void ForceGarbageCollection()
      {
         GetFunction<R_gc>()();
      }

      /// <summary>
      /// Gets a symbol defined in the global environment.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <returns>The symbol.</returns>
      public SymbolicExpression GetSymbol(string name)
      {
         if (!IsRunning)
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
      public SymbolicExpression GetSymbol(string name, REnvironment environment)
      {
         if (!IsRunning)
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
         if (!IsRunning)
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
      public void SetSymbol(string name, SymbolicExpression expression, REnvironment environment)
      {
         if (!IsRunning)
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
      public SymbolicExpression Evaluate(string statement)
      {
         if (!IsRunning)
         {
            throw new InvalidOperationException();
         }
         return Defer(statement).LastOrDefault();
      }

      /// <summary>
      /// Evaluates a statement in the given stream.
      /// </summary>
      /// <param name="stream">The stream.</param>
      /// <returns>Last evaluation.</returns>
      public SymbolicExpression Evaluate(Stream stream)
      {
         if (!IsRunning)
         {
            throw new InvalidOperationException();
         }
         return Defer(stream).LastOrDefault();
      }

      /// <summary>
      /// Evaluates a statement in the given string.
      /// </summary>
      /// <param name="statement">The statement.</param>
      /// <returns>Each evaluation.</returns>
      private IEnumerable<SymbolicExpression> Defer(string statement)
      {
         if (!IsRunning)
         {
            throw new InvalidOperationException();
         }
         if (statement == null)
         {
            throw new ArgumentNullException();
         }

         using (TextReader reader = new StringReader(statement))
         {
            var incompleteStatement = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
               foreach (var segment in Segment(line))
               {
                  var result = Parse(segment, incompleteStatement);
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
      public IEnumerable<SymbolicExpression> Defer(Stream stream)
      {
         if (!IsRunning)
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
            var incompleteStatement = new StringBuilder();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
               foreach (var segment in Segment(line))
               {
                  var result = Parse(segment, incompleteStatement);
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
         var segments = line.Split(';');
         for (var index = 0; index < segments.Length; index++)
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
         var s = GetFunction<Rf_mkString>()(incompleteStatement.ToString());

         using (new ProtectedPointer(this, s))
         {
            ParseStatus status;
            var vector = new ExpressionVector(this, GetFunction<R_ParseVector>()(s, -1, out status, NilValue.DangerousGetHandle()));
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
                  var errorStatement = incompleteStatement.ToString();
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
         if (!IsRunning)
         {
            throw new InvalidOperationException();
         }
         var newArgs = Utility.AddFirst(ID, args);
         GetFunction<R_set_command_line_arguments>()(newArgs.Length, newArgs);
      }

      public event EventHandler Disposing;

      protected virtual void OnDisposing(EventArgs e)
      {
         if (Disposing != null)
         {
            Disposing(this, e);
         }
      }

      protected override void Dispose(bool disposing)
      {
         if (isRunning)
         {
            this.isRunning = false;
            instances.Remove(ID);
         }
         OnDisposing(EventArgs.Empty);
         if (disposing)
         {
            GetFunction<R_RunExitFinalizers>()();
            GetFunction<Rf_CleanEd>()();
            GetFunction<R_CleanTempDir>()();
         }
         this.isRunning = false;

         if (disposing && this.adapter != null)
         {
            this.adapter.Dispose();
            this.adapter = null;
         }

         // Why is this here?
         GC.KeepAlive(this.parameter);
         base.Dispose(disposing);
      }

      /// <summary>
      /// Gets the predefined symbol with the specified name.
      /// </summary>
      /// <param name="name">The name.</param>
      /// <returns>The symbol.</returns>
      public SymbolicExpression GetPredefinedSymbol(string name)
      {
         if (!IsRunning)
         {
            throw new InvalidOperationException();
         }
         try
         {
            var pointer = DangerousGetHandle(name);
            return new SymbolicExpression(this, Marshal.ReadIntPtr(pointer));
         }
         catch (Exception ex)
         {
            throw new ArgumentException(null, ex);
         }
      }

      #region Nested type: _getDLLVersion

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      private delegate IntPtr _getDLLVersion();

      #endregion Nested type: _getDLLVersion
   }
}