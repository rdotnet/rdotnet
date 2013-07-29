using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using RDotNet.Internals;
using RDotNet.Internals.Unix;
using RDotNet.Internals.Windows;

namespace RDotNet.Devices
{
	internal class CharacterDeviceAdapter : IDisposable
	{
		private readonly ICharacterDevice device;

		private REngine engine;

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
			this.device = device;
		}

		/// <summary>
		/// Gets the implementation of <see cref="ICharacterDevice"/> interface.
		/// </summary>
		public ICharacterDevice Device
		{
			get { return this.device; }
		}

		private REngine Engine
		{
			get { return this.engine; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			GC.KeepAlive(this);
		}

		#endregion

		internal void Install(REngine engine, StartupParameter parameter)
		{
			this.engine = engine;
			switch (Environment.OSVersion.Platform)
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
			string rhome = Marshal.PtrToStringAnsi(this.engine.GetFunction<getValue>("get_R_HOME")());
			parameter.start.rhome = Marshal.StringToHGlobalAnsi(ConvertSeparator(rhome));
			string home = Marshal.PtrToStringAnsi(this.engine.GetFunction<getValue>("getRUser")());
			parameter.start.home = Marshal.StringToHGlobalAnsi(ConvertSeparator(home));
			parameter.start.CharacterMode = UiMode.LinkDll;
			parameter.start.ReadConsole = ReadConsole;
			parameter.start.WriteConsole = WriteConsole;
			parameter.start.WriteConsoleEx = WriteConsoleEx;
			parameter.start.CallBack = Callback;
			parameter.start.ShowMessage = ShowMessage;
			parameter.start.YesNoCancel = Ask;
			parameter.start.Busy = Busy;
		}

		private void SetupUnixDevice()
		{
			IntPtr suicidePointer = this.engine.DangerousGetHandle("ptr_R_Suicide");
			IntPtr newSuicide = Marshal.GetFunctionPointerForDelegate((ptr_R_Suicide)Suicide);
			Marshal.WriteIntPtr(suicidePointer, newSuicide);
			IntPtr showMessagePointer = this.engine.DangerousGetHandle("ptr_R_ShowMessage");
			IntPtr newShowMessage = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowMessage)ShowMessage);
			Marshal.WriteIntPtr(showMessagePointer, newShowMessage);
			IntPtr readConsolePointer = this.engine.DangerousGetHandle("ptr_R_ReadConsole");
			IntPtr newReadConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ReadConsole)ReadConsole);
			Marshal.WriteIntPtr(readConsolePointer, newReadConsole);
			IntPtr writeConsolePointer = this.engine.DangerousGetHandle("ptr_R_WriteConsole");
			IntPtr newWriteConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsole)WriteConsole);
			Marshal.WriteIntPtr(writeConsolePointer, newWriteConsole);
			IntPtr writeConsoleExPointer = this.engine.DangerousGetHandle("ptr_R_WriteConsoleEx");
			IntPtr newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsoleEx)WriteConsoleEx);
			Marshal.WriteIntPtr(writeConsoleExPointer, newWriteConsoleEx);
			IntPtr resetConsolePointer = this.engine.DangerousGetHandle("ptr_R_ResetConsole");
			IntPtr newResetConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ResetConsole)ResetConsole);
			Marshal.WriteIntPtr(resetConsolePointer, newResetConsole);
			IntPtr flushConsolePointer = this.engine.DangerousGetHandle("ptr_R_FlushConsole");
			IntPtr newFlushConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_FlushConsole)FlushConsole);
			Marshal.WriteIntPtr(flushConsolePointer, newFlushConsole);
			IntPtr clearerrConsolePointer = this.engine.DangerousGetHandle("ptr_R_ClearerrConsole");
			IntPtr newClearerrConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ClearerrConsole)ClearErrorConsole);
			Marshal.WriteIntPtr(clearerrConsolePointer, newClearerrConsole);
			IntPtr busyPointer = this.engine.DangerousGetHandle("ptr_R_Busy");
			IntPtr newBusy = Marshal.GetFunctionPointerForDelegate((ptr_R_Busy)Busy);
			Marshal.WriteIntPtr(busyPointer, newBusy);
			IntPtr cleanUpPointer = this.engine.DangerousGetHandle("ptr_R_CleanUp");
			IntPtr newCleanUp = Marshal.GetFunctionPointerForDelegate((ptr_R_CleanUp)CleanUp);
			Marshal.WriteIntPtr(cleanUpPointer, newCleanUp);
			IntPtr showFilesPointer = this.engine.DangerousGetHandle("ptr_R_ShowFiles");
			IntPtr newShowFiles = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowFiles)ShowFiles);
			Marshal.WriteIntPtr(showFilesPointer, newShowFiles);
			IntPtr chooseFilePointer = this.engine.DangerousGetHandle("ptr_R_ChooseFile");
			IntPtr newChooseFile = Marshal.GetFunctionPointerForDelegate((ptr_R_ChooseFile)ChooseFile);
			Marshal.WriteIntPtr(chooseFilePointer, newChooseFile);
			IntPtr editFilePointer = this.engine.DangerousGetHandle("ptr_R_EditFile");
			IntPtr newEditFile = Marshal.GetFunctionPointerForDelegate((ptr_R_EditFile)EditFile);
			Marshal.WriteIntPtr(editFilePointer, newEditFile);
			IntPtr loadHistoryPointer = this.engine.DangerousGetHandle("ptr_R_loadhistory");
			IntPtr newLoadHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_loadhistory)LoadHistory);
			Marshal.WriteIntPtr(loadHistoryPointer, newLoadHistory);
			IntPtr saveHistoryPointer = this.engine.DangerousGetHandle("ptr_R_savehistory");
			IntPtr newSaveHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_savehistory)SaveHistory);
			Marshal.WriteIntPtr(saveHistoryPointer, newSaveHistory);
			IntPtr addHistoryPointer = this.engine.DangerousGetHandle("ptr_R_addhistory");
			IntPtr newAddHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_addhistory)AddHistory);
			Marshal.WriteIntPtr(addHistoryPointer, newAddHistory);
		}

		private static string ConvertSeparator(string path)
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

		private IntPtr LoadHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
		{
			var c = new Language(Engine, call);
			var op = new SymbolicExpression(Engine, operation);
			var arglist = new Pairlist(Engine, args);
			var env = new REnvironment(Engine, environment);
			SymbolicExpression result = Device.LoadHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}

		private IntPtr SaveHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
		{
			var c = new Language(Engine, call);
			var op = new SymbolicExpression(Engine, operation);
			var arglist = new Pairlist(Engine, args);
			var env = new REnvironment(Engine, environment);
			SymbolicExpression result = Device.SaveHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}

		private IntPtr AddHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
		{
			var c = new Language(Engine, call);
			var op = new SymbolicExpression(Engine, operation);
			var arglist = new Pairlist(Engine, args);
			var env = new REnvironment(Engine, environment);
			SymbolicExpression result = Device.AddHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}

		#region Nested type: getValue

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr getValue();

		#endregion
	}
}
