using System;
using System.IO;
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
#if !MAC && !LINUX
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
#endif
		}

#if !MAC && !LINUX
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
			return Device.ReadConsole(prompt, buffer, count, history);
		}

		private void WriteConsole(string buffer, int length)
		{
			WriteConsoleEx(buffer, length, ConsoleOutputType.None);
		}

		private void WriteConsoleEx(string buffer, int length, ConsoleOutputType outputType)
		{
			Device.WriteConsole(buffer, length, outputType);
		}

		private void Callback()
		{
			Device.Callback();
		}

		private void ShowMessage(string message)
		{
			Device.ShowMessage(message);
		}

		private YesNoCancel Ask(string question)
		{
			return Device.Ask(question);
		}

		private void Busy(BusyType which)
		{
			Device.Busy(which);
		}
	}
}
