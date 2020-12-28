using RDotNet.Internals;
using RDotNet.Internals.Unix;
using RDotNet.NativeLibrary;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RDotNet.Devices
{
    internal class CharacterDeviceAdapter : IDisposable
    {
        /// <summary>
        /// When R calls the character device (unamanged R calling managed code),
        /// it sometimes calls the method with 'this == null' when writing/reading
        /// from console (this seems to happen on Mono and may be a bug).
        ///
        /// The (somewhat incorrect) workaround is to keep the last device in a static
        /// field and use it when 'this == null' (the check is done in 'this.Device').
        /// This workarounds: http://rdotnet.codeplex.com/workitem/154
        /// </summary>
        private static ICharacterDevice lastDevice;

        private readonly ICharacterDevice device;
        private REngine engine;

        private ptr_R_Suicide suicideDelegate;
        private ptr_R_ShowMessage showMessageDelegate;
        private ptr_R_ReadConsole readConsoleDelegate;
        private ptr_R_WriteConsole writeConsoleDelegate;
        private ptr_R_WriteConsoleEx writeConsoleExDelegate;
        private ptr_R_ResetConsole resetConsoleDelegate;
        private ptr_R_FlushConsole flushConsoleDelegate;
        private ptr_R_ClearerrConsole clearerrConsoleDelegate;
        private ptr_R_Busy busyDelegate;
        private ptr_R_CleanUp cleanUpDelegate;
        private ptr_R_ShowFiles showFilesDelegate;
        private ptr_R_ChooseFile chooseFileDelegate;
        private ptr_R_EditFile editFileDelegate;
        private ptr_R_loadhistory loadHistoryDelegate;
        private ptr_R_savehistory saveHistoryDelegate;
        private ptr_R_addhistory addHistoryDelegate;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="device">The implementation.</param>
        public CharacterDeviceAdapter(ICharacterDevice device)
        {
            if (device == null)
            {
                throw new ArgumentNullException("device");
            }
            lastDevice = device;
            this.device = device;
        }

        /// <summary>
        /// Gets the implementation of <see cref="ICharacterDevice"/> interface.
        /// </summary>
        public ICharacterDevice Device
        {
            get
            {
                if (this == null) return lastDevice;
                else return this.device;
            }
        }

        private REngine Engine
        {
            get { return this.engine; }
        }

        protected TDelegate GetFunction<TDelegate>() where TDelegate : class
        {
            return Engine.GetFunction<TDelegate>();
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.KeepAlive(this);
        }

        #endregion IDisposable Members

        internal void Install(REngine engine, StartupParameter parameter)
        {
            this.engine = engine;
            switch (NativeUtility.GetPlatform())
            {
                case PlatformID.Win32NT:
                    SetupWindowsDevice(parameter);
                    break;

                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    SetupUnixDevice();
                    break;
            }
        }

        private void SetupWindowsDevice(StartupParameter parameter)
        {
            if (parameter.RHome == null)
            {
                parameter.start.rhome = ToNativeUnixPath(NativeUtility.GetRHomeEnvironmentVariable());
            }
            if (parameter.Home == null)
            {
                string home = Marshal.PtrToStringAnsi(Engine.GetFunction<getValue>("getRUser")());
                parameter.start.home = ToNativeUnixPath(home);
            }
            parameter.start.ReadConsole = ReadConsole;
            parameter.start.WriteConsole = WriteConsole;
            parameter.start.WriteConsoleEx = WriteConsoleEx;
            parameter.start.CallBack = Callback;
            parameter.start.ShowMessage = ShowMessage;
            parameter.start.YesNoCancel = Ask;
            parameter.start.Busy = Busy;
        }

        private static IntPtr ToNativeUnixPath(string path)
        {
            return Marshal.StringToHGlobalAnsi(ConvertSeparatorToUnixStylePath(path));
        }

        private void SetupUnixDevice()
        {

            suicideDelegate = (ptr_R_Suicide)Suicide;
            IntPtr newSuicide = Marshal.GetFunctionPointerForDelegate(suicideDelegate);
            this.engine.WriteIntPtr("ptr_R_Suicide", newSuicide);

            showMessageDelegate = (ptr_R_ShowMessage)ShowMessage;
            IntPtr newShowMessage = Marshal.GetFunctionPointerForDelegate(showMessageDelegate);
            this.engine.WriteIntPtr("ptr_R_ShowMessage", newShowMessage);

            readConsoleDelegate = (ptr_R_ReadConsole)ReadConsole;
            IntPtr newReadConsole = Marshal.GetFunctionPointerForDelegate(readConsoleDelegate);
            this.engine.WriteIntPtr("ptr_R_ReadConsole", newReadConsole);

            // Candidate fix for https://github.com/rdotnet/rdotnet/issues/131
            // Not sure when behavior changed in R character handling, but it seems that at some point 
            // having R_outputfile set to not NULL e.g.:
            // in src/unix/system.c 
            // R_Outputfile = stdout;
            // R_Consolefile = stderr;
            // took precedence over setting ptr_R_WriteConsole with a callback. 
            // We need to reset these two values to a nullptr: 
            this.engine.WriteIntPtr("R_Outputfile", IntPtr.Zero);
            this.engine.WriteIntPtr("R_Consolefile", IntPtr.Zero);


            writeConsoleDelegate = (ptr_R_WriteConsole)WriteConsole;
            IntPtr newWriteConsole = Marshal.GetFunctionPointerForDelegate(writeConsoleDelegate);
            this.engine.WriteIntPtr("ptr_R_WriteConsole", newWriteConsole);

            writeConsoleExDelegate = (ptr_R_WriteConsoleEx)WriteConsoleEx;
            IntPtr newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate(writeConsoleExDelegate);
            this.engine.WriteIntPtr("ptr_R_WriteConsoleEx", newWriteConsoleEx);

            resetConsoleDelegate = (ptr_R_ResetConsole)ResetConsole;
            IntPtr newResetConsole = Marshal.GetFunctionPointerForDelegate(resetConsoleDelegate);
            this.engine.WriteIntPtr("ptr_R_ResetConsole", newResetConsole);

            flushConsoleDelegate = (ptr_R_FlushConsole)FlushConsole;
            IntPtr newFlushConsole = Marshal.GetFunctionPointerForDelegate(flushConsoleDelegate);
            this.engine.WriteIntPtr("ptr_R_FlushConsole", newFlushConsole);

            clearerrConsoleDelegate = (ptr_R_ClearerrConsole)ClearErrorConsole;
            IntPtr newClearerrConsole = Marshal.GetFunctionPointerForDelegate(clearerrConsoleDelegate);
            this.engine.WriteIntPtr("ptr_R_ClearerrConsole", newClearerrConsole);

            busyDelegate = (ptr_R_Busy)Busy;
            IntPtr newBusy = Marshal.GetFunctionPointerForDelegate(busyDelegate);
            this.engine.WriteIntPtr("ptr_R_Busy", newBusy);

            cleanUpDelegate = (ptr_R_CleanUp)CleanUp;
            IntPtr newCleanUp = Marshal.GetFunctionPointerForDelegate(cleanUpDelegate);
            this.engine.WriteIntPtr("ptr_R_CleanUp", newCleanUp);

            showFilesDelegate = (ptr_R_ShowFiles)ShowFiles;
            IntPtr newShowFiles = Marshal.GetFunctionPointerForDelegate(showFilesDelegate);
            this.engine.WriteIntPtr("ptr_R_ShowFiles", newShowFiles);

            chooseFileDelegate = (ptr_R_ChooseFile)ChooseFile;
            IntPtr newChooseFile = Marshal.GetFunctionPointerForDelegate(chooseFileDelegate);
            this.engine.WriteIntPtr("ptr_R_ChooseFile", newChooseFile);

            editFileDelegate = (ptr_R_EditFile)EditFile;
            IntPtr newEditFile = Marshal.GetFunctionPointerForDelegate(editFileDelegate);
            this.engine.WriteIntPtr("ptr_R_EditFile", newEditFile);

            loadHistoryDelegate = (ptr_R_loadhistory)LoadHistory;
            IntPtr newLoadHistory = Marshal.GetFunctionPointerForDelegate(loadHistoryDelegate);
            this.engine.WriteIntPtr("ptr_R_loadhistory", newLoadHistory);

            saveHistoryDelegate = (ptr_R_savehistory)SaveHistory;
            IntPtr newSaveHistory = Marshal.GetFunctionPointerForDelegate(saveHistoryDelegate);
            this.engine.WriteIntPtr("ptr_R_savehistory", newSaveHistory);

            addHistoryDelegate = (ptr_R_addhistory)AddHistory;
            IntPtr newAddHistory = Marshal.GetFunctionPointerForDelegate(addHistoryDelegate);
            this.engine.WriteIntPtr("ptr_R_addhistory", newAddHistory);
        }

        private static string ConvertSeparatorToUnixStylePath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        private bool ReadConsole(string prompt, StringBuilder buffer, int count, bool history)
        {
            buffer.Clear();
            string input = Device.ReadConsole(prompt, count, history);
            buffer.Append(input).Append("\n"); // input must end with '\n\0' ('\0' is appended during marshalling).
            return input != null;
        }

        private void WriteConsole(string buffer, int length)
        {
            WriteConsoleEx(buffer, length, ConsoleOutputType.None);
        }

        private void WriteConsoleEx(string buffer, int length, ConsoleOutputType outputType)
        {
            Device.WriteConsole(buffer, length, outputType);
        }

        private void ShowMessage(string message)
        {
            Device.ShowMessage(message);
        }

        private void Busy(BusyType which)
        {
            Device.Busy(which);
        }

        private void Callback()
        {
            Device.Callback();
        }

        private YesNoCancel Ask(string question)
        {
            return Device.Ask(question);
        }

        private void Suicide(string message)
        {
            Device.Suicide(message);
        }

        private void ResetConsole()
        {
            Device.ResetConsole();
        }

        private void FlushConsole()
        {
            Device.FlushConsole();
        }

        private void ClearErrorConsole()
        {
            Device.ClearErrorConsole();
        }

        private void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Device.CleanUp(saveAction, status, runLast);
        }

        private bool ShowFiles(int count, string[] files, string[] headers, string title, bool delete, string pager)
        {
            return Device.ShowFiles(files, headers, title, delete, pager);
        }

        private int ChooseFile(bool create, StringBuilder buffer, int length)
        {
            string path = Device.ChooseFile(create);
            return path == null ? 0 : Encoding.ASCII.GetByteCount(path);
        }

        private void EditFile(string file)
        {
            Device.EditFile(file);
        }

        private IntPtr CallDeviceFunction(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment, Func<Language, SymbolicExpression, Pairlist, REnvironment, SymbolicExpression> func)
        {
            var c = new Language(Engine, call);
            var op = new SymbolicExpression(Engine, operation);
            var arglist = new Pairlist(Engine, args);
            var env = new REnvironment(Engine, environment);
            SymbolicExpression result = func(c, op, arglist, env);
            return result.DangerousGetHandle();
        }

        private IntPtr LoadHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.LoadHistory);
        }

        private IntPtr SaveHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.SaveHistory);
        }

        private IntPtr AddHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
        {
            return CallDeviceFunction(call, operation, args, environment, Device.AddHistory);
        }

        #region Nested type: getValue

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr getValue();

        #endregion Nested type: getValue
    }
}
