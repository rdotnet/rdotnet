using RDotNet.Devices;
using RDotNet.Internals;
using RDotNet.NativeLibrary;
using RDotNet.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

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
    ///	NumericVector random = engine.Evaluate("rnorm(5, 0, 1)").AsNumeric();
    ///	foreach (double r in random)
    ///	{
    ///		Console.Write(r + " ");
    ///	}
    /// }
    /// </code>
    /// </example>
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class REngine : DynamicInterop.UnmanagedDll
    {
        private static readonly ICharacterDevice DefaultDevice = new ConsoleDevice();

        private readonly string id;
        private CharacterDeviceAdapter adapter;
        private bool isRunning;
        private StartupParameter parameter;
        private static bool environmentIsSet = false;
        private static REngine engine = null;

        /// <summary>
        /// Create a new REngine instance
        /// </summary>
        /// <param name="id">The identifier of this object</param>
        /// <param name="dll">The name of the file that is the shared R library, e.g. "R.dll"</param>
        protected REngine(string id, string dll)
            : base(dll)
        {
            this.id = id;
            this.isRunning = false;
            this.Disposed = false;
            this.EnableLock = true; // See https://rdotnet.codeplex.com/workitem/113; it seems wise to enable it by default.
            this.AutoPrint = true;
        }

        /// <summary>
        /// Gets/sets whether the call to Preserve and Unpreserve on symbolic expressions
        /// should be using a lock to prevent thread concurrency issues. Default is false;
        /// </summary>
        /// <remarks>Thanks to gchapman for proposing the fix. See https://rdotnet.codeplex.com/workitem/67 for details</remarks>
        public bool EnableLock { get; set; }

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
                CheckEngineIsRunning();
                return GetPredefinedSymbol("R_GlobalEnv").AsEnvironment();
            }
        }

        private void CheckEngineIsRunning()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("This engine is not running. You may have forgotten to call Initialize");
            }
        }

        /// <summary>
        /// Gets the root environment.
        /// </summary>
        public REnvironment EmptyEnvironment
        {
            get
            {
                CheckEngineIsRunning();
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
                CheckEngineIsRunning();
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
                CheckEngineIsRunning();
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
                CheckEngineIsRunning();
                return GetPredefinedSymbol("R_UnboundValue");
            }
        }

        /// <summary>
        /// Gets the name of the R engine instance (singleton).
        /// </summary>
        public static string EngineName { get { return "R.NET"; } }

        /// <summary>
        /// Gets a reference to the R engine, creating and initializing it if necessary. In most cases users need not provide any parameter to this method.
        /// </summary>
        /// <param name="dll">The file name of the library to load, e.g. "R.dll" for Windows. You usually do not need need to provide this optional parameter</param>
        /// <param name="initialize">Initialize the R engine after its creation. Default is true</param>
        /// <param name="parameter">If 'initialize' is 'true', you can optionally specify the specific startup parameters for the R native engine</param>
        /// <param name="device">If 'initialize' is 'true', you can optionally specify a character device for the R engine to use</param>
        /// <returns>The engine.</returns>
        /// <example>
        /// <p>A minimalist approach is to just call GetInstance</p>
        /// <code>
        /// REngine.SetEnvironmentVariables();
        /// var engine = REngine.GetInstance();
        /// engine.Evaluate("letters[1:26]");
        /// </code>
        /// <p>In unusual circumstances you may need to elaborate on the initialization in a separate method call</p>
        /// <code>
        /// REngine.SetEnvironmentVariables(rPath=@"c:\my\peculiar\path\to\R\bin\x64");
        /// var engine = REngine.GetInstance(initialize=false);
        /// StartupParameter sParams=new StartupParameter(){NoRenviron=true;};
        /// ICharacterDevice device = new YourCustomDevice();
        /// engine.Initialize(parameter: sParams, device: device);
        /// engine.Evaluate("letters[1:26]");
        /// </code>
        /// </example>
        public static REngine GetInstance(string dll = null, bool initialize = true, StartupParameter parameter = null, ICharacterDevice device = null)
        {
            if (!environmentIsSet) // should there be a warning? and how?
                SetEnvironmentVariables();
            if (engine == null)
            {
                engine = CreateInstance(EngineName, dll);
                if (initialize)
                    engine.Initialize(parameter, device);
            }
            if (engine.Disposed)
                throw new InvalidOperationException("The single REngine instance has already been disposed of (i.e. shut down). Multiple engine restart is not possible.");
            return engine;
        }

        /// <summary>
        /// Creates a new instance that handles R.DLL.
        /// </summary>
        /// <param name="id">ID.</param>
        /// <param name="dll">The file name of the library to load, e.g. "R.dll" for Windows. You should usually not provide this optional parameter</param>
        /// <returns>The engine.</returns>
        private static REngine CreateInstance(string id, string dll = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id", "Empty ID is not allowed.");
            }
            if (id == string.Empty)
            {
                throw new ArgumentException("Empty ID is not allowed.", "id");
            }
            dll = ProcessRDllFileName(dll);
            var engine = new REngine(id, dll);
            return engine;
        }

        /// <summary>
        /// if the parameter is null or empty string, return the default names of the R shared library file depending on the platform
        /// </summary>
        /// <param name="dll">The name of the library provided, possibly null or empty</param>
        /// <returns>A candidate for the file name of the R shared library</returns>
        protected static string ProcessRDllFileName(string dll)
        {
            if (!string.IsNullOrEmpty(dll)) return dll;
            return NativeUtility.GetRLibraryFileName();
        }

        static private string EncodeNonAsciiCharacters(string value)
        {
          StringBuilder sb = new StringBuilder();
          foreach (char c in value)
          {
            if (c > 127)
            {
              string encodedValue = "\\u" + ((int)c).ToString("x4");
              sb.Append(encodedValue);
            }
            else {
              sb.Append(c);
            }
          }
          return sb.ToString();
        }

        /// <summary>
        /// Perform the necessary setup for the PATH and R_HOME environment variables.
        /// </summary>
        /// <param name="rPath">The path of the directory containing the R native library.
        /// If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux</param>
        /// <param name="rHome">The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set,
        /// the function uses platform specific sensible default behaviors.</param>
        /// <remarks>
        /// This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
        /// </remarks>
        public static void SetEnvironmentVariables(string rPath = null, string rHome = null)
        {
            environmentIsSet = true;
            NativeUtility.SetEnvironmentVariables(rPath: rPath, rHome: rHome);
        }

        /// <summary>
        /// Initialize this REngine object. Only the first call has an effect. Subsequent calls to this function are ignored.
        /// </summary>
        /// <param name="parameter">The optional startup parameters</param>
        /// <param name="device">The optional character device to use for the R engine</param>
        /// <param name="setupMainLoop">if true, call the functions to initialise the embedded R</param>
        public void Initialize(StartupParameter parameter = null, ICharacterDevice device = null, bool setupMainLoop = true)
        {
            //         Console.WriteLine("REngine.Initialize start");
            if (this.isRunning)
                return;
            //         Console.WriteLine("REngine.Initialize, after isRunning checked as false");
            this.parameter = parameter ?? new StartupParameter();
            this.adapter = new CharacterDeviceAdapter(device ?? DefaultDevice);
            // Disabling the stack checking here, to try to avoid the issue on Linux.
            // The disabling used to be here around end Nov 2013. Was moved later in this
            // function to cater for disabling on Windows, @ rev 305, however this may have
            // re-broken on Linux. so we may need to call it twice.
            SetCstackChecking();
            //         Console.WriteLine("Initialize-SetCstackChecking; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

            if (!setupMainLoop)
            {
                this.isRunning = true;
                return;
            }

            string[] R_argv = BuildRArgv(this.parameter);
            //string[] R_argv = new[]{"rdotnet_app",  "--interactive",  "--no-save",  "--no-restore-data",  "--max-ppsize=50000"};
            //rdotnet_app --quiet --interactive --no-save --no-restore-data --max-mem-size=18446744073709551615 --max-ppsize=50000
            GetFunction<R_setStartTime>()();
            int R_argc = R_argv.Length;
            //         Console.WriteLine("Initialize-R_setStartTime; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

            if (NativeUtility.GetPlatform() == PlatformID.Win32NT)
            {
                // Attempted Fix for https://rdotnet.codeplex.com/workitem/110; not working
                // Tried to make sure that the command line options are taken into account. They are NOT effectively so via Rf_initialize_R only.
                // The problem is that cmdlineoptions assumes it is called by RGui.exe or RTerm.exe, and overrides R_HOME

                //   GetFunction<R_set_command_line_arguments>()(R_argc, R_argv);
                //   GetFunction<cmdlineoptions>()(R_argc, R_argv);
            }

            var status = GetFunction<Rf_initialize_R>()(R_argc, R_argv);
            if (status != 0)
                throw new Exception("A call to Rf_initialize_R returned a non-zero; status=" + status);
            //         Console.WriteLine("Initialize-Rf_initialize_R; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
            SetCstackChecking();

            // following in RInside: may not be needed.
            //GetFunction<R_ReplDLLinit> () ();
            //this.parameter.Interactive = true;
            this.adapter.Install(this, this.parameter);
            //Console.WriteLine("Initialize-adapter installation; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
            switch (NativeUtility.GetPlatform())
            {
                case PlatformID.Win32NT:
                    GetFunction<R_SetParams_Windows>("R_SetParams")(ref this.parameter.start);
                    break;

                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    GetFunction<R_SetParams_Unix>("R_SetParams")(ref this.parameter.start.Common);
                    //Console.WriteLine("Initialize-R_SetParams_Unix; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));
                    break;
            }
            GetFunction<setup_Rmainloop>()();
            //Console.WriteLine("Initialize-after setup_Rmainloop; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

            // See comments in the first call to SetCstackChecking in this function as to why we (may) need it twice.
            SetCstackChecking();
            this.isRunning = true;

            //Console.WriteLine("Initialize-just before leaving; R_CStackLimit value is " + GetDangerousInt32("R_CStackLimit"));

            // Partial Workaround (hopefully temporary) for https://rdotnet.codeplex.com/workitem/110
            if (NativeUtility.GetPlatform() == PlatformID.Win32NT)
            {
                Evaluate(string.Format("invisible(memory.limit({0}))", (this.parameter.MaxMemorySize / 1048576UL)));
            }
        }

        private void SetCstackChecking()
        {
            // Don't do any stack checking, see R Exts, '8.1.5 Threading issues',
            // https://rdotnet.codeplex.com/discussions/462947
            // https://rdotnet.codeplex.com/workitem/115
            WriteInt32("R_CStackLimit", -1);
            switch (NativeUtility.GetPlatform())
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    WriteInt32("R_SignalHandlers", 0);
                    // RInside does this for non-WIN32.
                    break;
            }
        }

        /// <summary>
        /// Creates the command line arguments corresponding to the specified startup parameters
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <remarks>While not obvious from the R documentation, it seems that command line arguments need to be passed
        /// to get the startup parameters taken into account. Passing the StartupParameter to the API seems not to work as expected.
        /// While this function may appear like an oddity to a reader, it proved necessary to the initialisation of the R engine
        /// after much trial and error.</remarks>
        public static string[] BuildRArgv(StartupParameter parameter)
        {
            var platform = NativeUtility.GetPlatform();
            var argv = new List<string>();
            argv.Add("rdotnet_app");
            // Not sure whether I should add no-readline
            //[MarshalAs(UnmanagedType.Bool)]
            //public bool R_Quiet;
            if (parameter.Quiet && !parameter.Interactive) argv.Add("--quiet");  // --quite --interactive to R embedded crashed...

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool R_Slave;
            if (parameter.Slave) argv.Add("--slave");

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool R_Interactive;
            if (platform != PlatformID.Win32NT) // RTerm.exe --help shows no such option; Unix only.
                if (parameter.Interactive) argv.Add("--interactive");

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool R_Verbose;
            if (parameter.Verbose) argv.Add("--verbose");

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool LoadSiteFile;
            if (!parameter.LoadSiteFile) argv.Add("--no-site-file");

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool LoadInitFile;
            if (!parameter.LoadInitFile) argv.Add("--no-init-file");

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool DebugInitFile;
            //if (parameter.Quiet) argv.Add("--quiet");

            //public StartupRestoreAction RestoreAction;
            //public StartupSaveAction SaveAction;
            //internal UIntPtr vsize;
            //internal UIntPtr nsize;
            //internal UIntPtr max_vsize;
            //internal UIntPtr max_nsize;
            //internal UIntPtr ppsize;

            //[MarshalAs(UnmanagedType.Bool)]
            //public bool NoRenviron;
            if (parameter.NoRenviron) argv.Add("--no-environ");

            switch (parameter.SaveAction)
            {
                case StartupSaveAction.NoSave:
                    argv.Add("--no-save");
                    break;

                case StartupSaveAction.Save:
                    argv.Add("--save");
                    break;
            }
            switch (parameter.RestoreAction)
            {
                case StartupRestoreAction.NoRestore:
                    argv.Add("--no-restore-data");
                    break;

                case StartupRestoreAction.Restore:
                    argv.Add("--restore");
                    break;
            }

            if (parameter.MaxMemorySize == (Environment.Is64BitProcess ? ulong.MaxValue : uint.MaxValue))
            {
                // This creates a nasty crash if using the default MaxMemorySize. found out in Rdotnet workitem 72
                // do nothing
            }
            else
            {
                if (platform == PlatformID.Win32NT) // On unix, otherwise led to https://rdotnet.codeplex.com/workitem/137
                    argv.Add("--max-mem-size=" + parameter.MaxMemorySize);
            }
            argv.Add("--max-ppsize=" + parameter.StackSize);
            return argv.ToArray();
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
            CheckEngineIsRunning();
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
            CheckEngineIsRunning();
            if (environment == null)
            {
                environment = GlobalEnvironment;
            }
            return environment.GetSymbol(name);
        }

        /// <summary>
        /// Assign a value to a name in the global environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The symbol.</param>
        public void SetSymbol(string name, SymbolicExpression expression)
        {
            CheckEngineIsRunning();
            GlobalEnvironment.SetSymbol(name, expression);
        }

        /// <summary>
        /// Assign a value to a name in a specific environment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="expression">The symbol.</param>
        /// <param name="environment">The environment. If <c>null</c> is passed, <see cref="GlobalEnvironment"/> is used.</param>
        public void SetSymbol(string name, SymbolicExpression expression, REnvironment environment)
        {
            CheckEngineIsRunning();
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
            CheckEngineIsRunning();
            return Defer(EncodeNonAsciiCharacters(statement)).LastOrDefault();
        }

        /// <summary>
        /// Evaluates a statement in the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Last evaluation.</returns>
        public SymbolicExpression Evaluate(Stream stream)
        {
            CheckEngineIsRunning();
            return Defer(stream).LastOrDefault();
        }

        /// <summary>
        /// Evaluates a statement in the given string.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>Each evaluation.</returns>
        private IEnumerable<SymbolicExpression> Defer(string statement)
        {
            CheckEngineIsRunning();
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
            CheckEngineIsRunning();
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
            var segments = processInputString(line);
            for (var index = 0; index < segments.Length; index++)
            {
                if (index == segments.Length - 1)
                {
                    if (segments[index] != string.Empty)
                    {
                        yield return segments[index] + "\n";
                    }
                }
                else
                {
                    yield return segments[index] + ";";
                }
            }
        }

        private static string[] processInputString(string input)
        {
            // Fixes for
            // https://rdotnet.codeplex.com/workitem/165
            // https://github.com/jmp75/rdotnet/issues/14
            string[] lines = splitOnNewLines(input);
            List<string> statements = new List<string>();
            for (int i = 0; i < lines.Length; i++)
			{
			    statements.AddRange(processLine(lines[i]));
			}
            return statements.ToArray();
        }

        private static string[] splitOnNewLines(string input)
        {
            input = input.Replace("\n\r", "\n");
            return input.Split('\n');
        }

        private static string[] processLine(string line)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine == string.Empty)
                return new string[]{};
            if (trimmedLine.StartsWith("#"))
                return new string[]{line};

            string theRest;
            string statement = splitOnFirst(line, out theRest, ';');

            var result = new List<string>();
            if(!statement.Contains("#"))
            {
                result.Add(statement);
                result.AddRange(processLine(theRest));
            }
            else
            {
                // paste('this contains ### characters', " this too ###", 'Oh, and this # one too') # but "this" 'rest' is commented
                // Find the fist # character such that before that, there is an 
                // even number of " and an even number of ' characters
                
                int[] whereHash = IndexOfAll(statement, "#");
                int firstComment = EvenStringDelimitors(statement, whereHash);
                if(firstComment < 0 ) 
                    // incomplete statement??? such as:
                    // paste('this is the # ', ' start of an incomplete # statement
                {
                    result.Add(statement);
                    result.AddRange(processLine(theRest));
                }
                else
                {
                    result.Add(statement.Substring(0, firstComment));
                    // firstComment is a valid comment marker - not need to process "the rest"
                }
                string restFirstStatement;
                string beforeComment = splitOnFirst(statement, out restFirstStatement, '#');
            }
            return result.ToArray();
        }

        private static int EvenStringDelimitors(string statement, int[] whereHash)
        {
            for (int i = 0; i < whereHash.Length; i++)
            {
                var s = statement.Substring(0, whereHash[i]);
                if (IsClosedString(s))
                    return whereHash[i];
            }
            return -1;
        }

        private static bool IsClosedString(string s)
        {
            // paste("#hashtag")
            // paste("#hashtag''''")
            // paste('#hashtag""""')
            // paste('#hashtag""#""')
            // paste('#hashtag""#""', "#hash ''' ")
            bool inSingleQuote = false, inDoubleQuotes=false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\'')
                {
                    if(i > 0)
                        if(s[i-1]=='\\')
                            continue;
                    if (inDoubleQuotes)
                        continue;
                    inSingleQuote = !inSingleQuote;
                }
                if (s[i] == '"')
                {
                    if (i > 0)
                        if (s[i - 1] == '\\')
                            continue;
                    if (inSingleQuote)
                        continue;
                    inDoubleQuotes = !inDoubleQuotes;
                }
            }
            return (!inSingleQuote) && (!inDoubleQuotes);
        }

        private static string splitOnFirst(string statement, out string rest, char sep)
        {
            var split = statement.Split(new[] { sep }, 2);
            if (split.Length == 1)
                rest = string.Empty;
            else
                rest = split[1];
            return split[0];
        }

        public static int[] IndexOfAll(string sourceString, string matchString)
        {
            matchString = Regex.Escape(matchString);
            var res = (from Match match in Regex.Matches(sourceString, matchString) select match.Index);
            return res.ToArray();
        }

        private static string splitOnStatementSeparators(string line, out string theRest)
        {
            throw new NotImplementedException();
        }

        private SymbolicExpression Parse(string statement, StringBuilder incompleteStatement)
        {
            incompleteStatement.Append(statement);
            var s = GetFunction<Rf_mkString>()(InternalString.NativeUtf8FromString(incompleteStatement.ToString()));
            string errorStatement;
            using (new ProtectedPointer(this, s))
            {
                ParseStatus status;
                var vector = new ExpressionVector(this, GetFunction<R_ParseVector>()(s, -1, out status, NilValue.DangerousGetHandle()));

                switch (status)
                {
                    case ParseStatus.OK:
                        incompleteStatement.Clear();
                        if (vector.Length == 0)
                        {
                            return null;
                        }
                        using (new ProtectedPointer(vector))
                        {
                            SymbolicExpression result;
                            if (!vector.First().TryEvaluate(GlobalEnvironment, out result))
                            {
                                throw new EvaluationException(LastErrorMessage);
                            }

                            if (AutoPrint && !result.IsInvalid && GetVisible())
                            {
                                GetFunction<Rf_PrintValue>()(result.DangerousGetHandle());
                            }
                            return result;
                        }
                    case ParseStatus.Incomplete:
                        return null;

                    case ParseStatus.Error:
                        // TODO: use LastErrorMessage if below is just a subset
                        var parseErrorMsg = this.GetAnsiString("R_ParseErrorMsg");
                        errorStatement = incompleteStatement.ToString();
                        incompleteStatement.Clear();
                        throw new ParseException(status, errorStatement, parseErrorMsg);
                    default:
                        errorStatement = incompleteStatement.ToString();
                        incompleteStatement.Clear();
                        throw new ParseException(status, errorStatement, "");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RDotNet.REngine"/> auto print R evaluation results, if they are visible.
        /// </summary>
        /// <value><c>true</c> if auto print; otherwise, <c>false</c>.</value>
        public bool AutoPrint { get; set; }

        private bool GetVisible()
        {
            var symbol = DangerousGetHandle("R_Visible");
            // If the R_Visible symbol is not exported by the current R engine (happens on R-2.14.1), then just return 'true'
            // https://github.com/BlueMountainCapital/FSharpRProvider/pull/152
            if (symbol == (System.IntPtr)0) return true;
            else
            {
              var value = Marshal.ReadInt32(symbol);
              var result = Convert.ToBoolean(value);
              return result;
            }
        }

        /// <summary>
        /// A cache of the unevaluated R expression 'geterrmessage'
        /// </summary>
        /// <remarks>do_geterrmessage is in Rdll.hide, so we cannot access at the C API level.
        /// We use the 'geterrmessage()' R evaluation, but not using the same mechanism as other REngine evaluation
        /// to avoid recursions issues</remarks>
        private Expression geterrmessage = null;

        /// <summary>
        /// Gets the last error message in the R engine; see R function geterrmessage.
        /// </summary>
        internal string LastErrorMessage
        {
            get
            {
                if (geterrmessage == null)
                {
                    var statement = "geterrmessage()\n";
                    var s = GetFunction<Rf_mkString>()(InternalString.NativeUtf8FromString(statement));
                    ParseStatus status;
                    var vector = new ExpressionVector(this, GetFunction<R_ParseVector>()(s, -1, out status, NilValue.DangerousGetHandle()));
                    if (status != ParseStatus.OK)
                        throw new ParseException(status, statement, "");
                    if (vector.Length == 0)
                        throw new ParseException(status, statement, "Failed to create expression vector!");
                    geterrmessage = vector.First();
                }
                SymbolicExpression result;
                if (geterrmessage.TryEvaluate(GlobalEnvironment, out result))
                {
                    var msgs = SymbolicExpressionExtension.AsCharacter(result).ToArray();
                    if (msgs.Length > 1)
                        throw new Exception("Unexpected multiple error messages returned");
                    if (msgs.Length == 0)
                        throw new Exception("No error messages returned (zero length)");
                    return msgs[0];
                }
                else
                    throw new EvaluationException("Unable to retrieve an R error message. Evaluating 'geterrmessage()' fails. The R engine is not in a working state.");
            }
        }

        /// <summary>
        /// Sets the command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void SetCommandLineArguments(string[] args)
        {
            CheckEngineIsRunning();
            var newArgs = ArrayConverter.Prepend(ID, args);
            GetFunction<R_set_command_line_arguments>()(newArgs.Length, newArgs);
        }

        /// <summary>
        /// Event triggered when disposing of this REngine
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Called on disposing of this REngine
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisposing(EventArgs e)
        {
            if (Disposing != null)
            {
                Disposing(this, e);
            }
        }

        /// <summary>
        /// Gets whether this object has been disposed of already.
        /// </summary>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Dispose of this REngine, including using the native R API to clean up, if the parameter is true
        /// </summary>
        /// <param name="disposing">if true, release native resources, using the native R API to clean up.</param>
        protected override void Dispose(bool disposing)
        {
            this.isRunning = false;
            OnDisposing(EventArgs.Empty);
            if (disposing && !Disposed)
            {
                GetFunction<R_RunExitFinalizers>()();
                GetFunction<Rf_CleanEd>()();
                GetFunction<R_CleanTempDir>()();
                Disposed = true;
            }

            if (disposing && this.adapter != null)
            {
                this.adapter.Dispose();
                this.adapter = null;
            }
            if (Disposed)
                return;
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
            CheckEngineIsRunning();
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

        /// <summary>
        /// Create a SymbolicExpression wrapping an existing native R symbolic expression
        /// </summary>
        /// <param name="sexp">A pointer to the R symbolic expression</param>
        /// <returns></returns>
        public SymbolicExpression CreateFromNativeSexp(IntPtr sexp)
        {
            return new SymbolicExpression(this, sexp);
        }

        #region Nested type: _getDLLVersion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr _getDLLVersion();

        #endregion Nested type: _getDLLVersion

        /// <summary>
        /// Removes variables from the R global environment, and whether garbage collections should be forced
        /// </summary>
        /// <param name="garbageCollectR">if true (default) request an R garbage collection. This happens after the .NET garbage collection if both requested</param>
        /// <param name="garbageCollectDotNet">If true (default), triggers CLR garbage collection and wait for pending finalizers.</param>
        /// <param name="removeHiddenRVars">Should hidden variables (starting with '.', such as '.Random.seed') be removed. Default is false.</param>
        /// <param name="detachPackages">If true, detach some packages and other attached resources. Default is false. See 'detach' function in R</param>
        /// <param name="toDetach">names of resources to dettach, e.g. an array of names such as 'mpg', 'package:lattice'.
        /// If null, entries found in 'search()' between the first item and 'package:base' are detached. See 'search' function documentation in R</param>
        public void ClearGlobalEnvironment(bool garbageCollectR = true, bool garbageCollectDotNet = true, bool removeHiddenRVars = false, bool detachPackages = false, string[] toDetach = null)
        {
            if (detachPackages)
                doDetachPackages(toDetach);
            var rmStatement = removeHiddenRVars ? "rm(list=ls(all.names=TRUE))" : "rm(list=ls())";
            this.Evaluate(rmStatement);
            if (garbageCollectDotNet)
            {
                DoDotNetGarbageCollection();
                DoDotNetGarbageCollection();
            }
            if (garbageCollectR)
                ForceGarbageCollection();
        }

        private void doDetachPackages(string[] toDetach)
        {
            if (toDetach == null)
            {
                toDetach = Evaluate("search()[2:(which(search()=='package:stats')-1)]").AsCharacter().ToArray();
            }
            foreach (var dbName in toDetach)
            {
                Evaluate("detach('" + dbName + "')");
            }
        }

        /// <summary>
        /// Triggers a .NET garbage collection. May be useful in some testing circumstance, but users should avoid using this.
        /// </summary>
        public static void DoDotNetGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private IntPtr stringNAPointer = IntPtr.Zero;

        /// <summary>
        /// Native pointer to the SEXP representing NA for strings (character vectors in R terminology).
        /// </summary>
        public IntPtr NaStringPointer
        {
            get
            {
                if (stringNAPointer == IntPtr.Zero)
                    stringNAPointer = NaString.DangerousGetHandle();
                return stringNAPointer;
            }
        }

        private SymbolicExpression stringNaSexp = null;

        /// <summary>
        /// SEXP representing NA for strings (character vectors in R terminology).
        /// </summary>
        public SymbolicExpression NaString
        {
            get
            {
                if (stringNaSexp == null)
                    stringNaSexp = this.GetPredefinedSymbol("R_NaString");
                return stringNaSexp;
            }
        }
    }
}