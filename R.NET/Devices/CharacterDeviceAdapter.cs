using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RDotNet.Internals;

namespace RDotNet.Devices
{
	internal class CharacterDeviceAdapter : IDisposable
	{
		private readonly ICharacterDevice device;
		/// <summary>
		/// Gets the implementation of <see cref="ICharacterDevice"/> interface.
		/// </summary>
		public ICharacterDevice Device
		{
			get { return this.device; }
		}
		
		private REngine engine;
		private REngine Engine
		{
			get { return this.engine; }
		}

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

		internal void Install(REngine engine, ref RStart start)
		{
			this.engine = engine;
#if WINDOWS
			string rhome = Marshal.PtrToStringAnsi(engine.GetFunction<getValue>("get_R_HOME")());
			rhome = ConvertSeparator(rhome);
			start.rhome = Marshal.StringToHGlobalAnsi(rhome);
			string home = Marshal.PtrToStringAnsi(engine.GetFunction<getValue>("getRUser")());
			home = ConvertSeparator(home);
			start.home = Marshal.StringToHGlobalAnsi(home);
			start.CharacterMode = UiMode.LinkDll;
			start.ReadConsole = this.ReadConsole;
			start.WriteConsole = this.WriteConsole;
			start.WriteConsoleEx = this.WriteConsoleEx;
			start.CallBack = this.Callback;
			start.ShowMessage = this.ShowMessage;
			start.YesNoCancel = this.Ask;
			start.Busy = this.Busy;
#elif MAC || LINUX
			IntPtr suicidePointer = engine.DangerousGetHandle("ptr_R_Suicide");
			IntPtr newSuicide = Marshal.GetFunctionPointerForDelegate((ptr_R_Suicide)this.Suicide);
			Marshal.WriteIntPtr(suicidePointer, newSuicide);
			IntPtr showMessagePointer = engine.DangerousGetHandle("ptr_R_ShowMessage");
			IntPtr newShowMessage = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowMessage)this.ShowMessage);
			Marshal.WriteIntPtr(showMessagePointer, newShowMessage);
			IntPtr readConsolePointer = engine.DangerousGetHandle("ptr_R_ReadConsole");
			IntPtr newReadConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ReadConsole)this.ReadConsole);
			Marshal.WriteIntPtr(readConsolePointer, newReadConsole);
			IntPtr writeConsolePointer = engine.DangerousGetHandle("ptr_R_WriteConsole");
			IntPtr newWriteConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsole)this.WriteConsole);
			Marshal.WriteIntPtr(writeConsolePointer, newWriteConsole);
			IntPtr writeConsoleExPointer = engine.DangerousGetHandle("ptr_R_WriteConsoleEx");
			IntPtr newWriteConsoleEx = Marshal.GetFunctionPointerForDelegate((ptr_R_WriteConsoleEx)this.WriteConsoleEx);
			Marshal.WriteIntPtr(writeConsoleExPointer, newWriteConsoleEx);
			IntPtr resetConsolePointer = engine.DangerousGetHandle("ptr_R_ResetConsole");
			IntPtr newResetConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ResetConsole)this.ResetConsole);
			Marshal.WriteIntPtr(resetConsolePointer, newResetConsole);
			IntPtr flushConsolePointer = engine.DangerousGetHandle("ptr_R_FlushConsole");
			IntPtr newFlushConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_FlushConsole)this.FlushConsole);
			Marshal.WriteIntPtr(flushConsolePointer, newFlushConsole);
			IntPtr clearerrConsolePointer = engine.DangerousGetHandle("ptr_R_ClearerrConsole");
			IntPtr newClearerrConsole = Marshal.GetFunctionPointerForDelegate((ptr_R_ClearerrConsole)this.ClearErrorConsole);
			Marshal.WriteIntPtr(clearerrConsolePointer, newClearerrConsole);
			IntPtr busyPointer = engine.DangerousGetHandle("ptr_R_Busy");
			IntPtr newBusy = Marshal.GetFunctionPointerForDelegate((ptr_R_Busy)this.Busy);
			Marshal.WriteIntPtr(busyPointer, newBusy);
			IntPtr cleanUpPointer = engine.DangerousGetHandle("ptr_R_CleanUp");
			IntPtr newCleanUp = Marshal.GetFunctionPointerForDelegate((ptr_R_CleanUp)this.CleanUp);
			Marshal.WriteIntPtr(cleanUpPointer, newCleanUp);
			IntPtr showFilesPointer = engine.DangerousGetHandle("ptr_R_ShowFiles");
			IntPtr newShowFiles = Marshal.GetFunctionPointerForDelegate((ptr_R_ShowFiles)this.ShowFiles);
			Marshal.WriteIntPtr(showFilesPointer, newShowFiles);
			IntPtr chooseFilePointer = engine.DangerousGetHandle("ptr_R_ChooseFile");
			IntPtr newChooseFile = Marshal.GetFunctionPointerForDelegate((ptr_R_ChooseFile)this.ChooseFile);
			Marshal.WriteIntPtr(chooseFilePointer, newChooseFile);
			IntPtr editFilePointer = engine.DangerousGetHandle("ptr_R_EditFile");
			IntPtr newEditFile = Marshal.GetFunctionPointerForDelegate((ptr_R_EditFile)this.EditFile);
			Marshal.WriteIntPtr(editFilePointer, newEditFile);
			IntPtr loadHistoryPointer = engine.DangerousGetHandle("ptr_R_loadhistory");
			IntPtr newLoadHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_loadhistory)this.LoadHistory);
			Marshal.WriteIntPtr(loadHistoryPointer, newLoadHistory);
			IntPtr saveHistoryPointer = engine.DangerousGetHandle("ptr_R_savehistory");
			IntPtr newSaveHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_savehistory)this.SaveHistory);
			Marshal.WriteIntPtr(saveHistoryPointer, newSaveHistory);
			IntPtr addHistoryPointer = engine.DangerousGetHandle("ptr_R_addhistory");
			IntPtr newAddHistory = Marshal.GetFunctionPointerForDelegate((ptr_R_addhistory)this.AddHistory);
			Marshal.WriteIntPtr(addHistoryPointer, newAddHistory);
#endif
		}
		
#if WINDOWS
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr getValue();

		private static string ConvertSeparator(string path)
		{
			return path.Replace(Path.DirectorySeparatorChar, '/');
		}
#endif
		
		public void Dispose()
		{
			GC.KeepAlive(this);
		}

		private bool ReadConsole(string prompt, StringBuilder buffer, int count, bool history)
		{
			buffer.Clear();
			string input = Device.ReadConsole(prompt, count, history);
			buffer.Append(input).Append("\n");  // input must end with '\n\0' ('\0' is appended during marshalling).
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
		
#if WINDOWS
		private void Callback()
		{
			Device.Callback();
		}
		
		private YesNoCancel Ask(string question)
		{
			return Device.Ask(question);
		}
#elif MAC || LINUX
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
			Language c = new Language(Engine, call);
			SymbolicExpression op = new SymbolicExpression(Engine, operation);
			Pairlist arglist = new Pairlist(Engine, args);
			RDotNet.Environment env = new RDotNet.Environment(Engine, environment);
			SymbolicExpression result = Device.LoadHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}
		
		private IntPtr SaveHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
		{
			Language c = new Language(Engine, call);
			SymbolicExpression op = new SymbolicExpression(Engine, operation);
			Pairlist arglist = new Pairlist(Engine, args);
			RDotNet.Environment env = new RDotNet.Environment(Engine, environment);
			SymbolicExpression result = Device.SaveHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}
		
		private IntPtr AddHistory(IntPtr call, IntPtr operation, IntPtr args, IntPtr environment)
		{
			Language c = new Language(Engine, call);
			SymbolicExpression op = new SymbolicExpression(Engine, operation);
			Pairlist arglist = new Pairlist(Engine, args);
			RDotNet.Environment env = new RDotNet.Environment(Engine, environment);
			SymbolicExpression result = Device.AddHistory(c, op, arglist, env);
			return result.DangerousGetHandle();
		}
#endif
	}
}
