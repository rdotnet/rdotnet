using System;
using System.Text;
using RDotNet.Internals;

namespace RDotNet.Devices
{
	public class ConsoleDevice : ICharacterDevice
	{
		public bool ReadConsole(string prompt, StringBuilder buffer, int capacity, bool history)
		{
			buffer.Clear();
			Console.Write(prompt);
			string input = Console.ReadLine();
			buffer.Append(input).Append("\n");  // input must end with '\n\0' ('\0' is appended during marshalling).
			return input != null;
		}

		public void WriteConsole(string output, int length, ConsoleOutputType outputType)
		{
			Console.Write(output);
		}

		public void Callback()
		{
		}

		public void ShowMessage(string message)
		{
			Console.Write(message);
		}

		public YesNoCancel Ask(string question)
		{
			Console.Write("{0} [y/n/c]: ", question);
			string input = Console.ReadLine();
			if (!string.IsNullOrEmpty(input))
			{
				switch (Char.ToLower(input[0]))
				{
					case 'y':
						return YesNoCancel.Yes;
					case 'n':
						return YesNoCancel.No;
					case 'c':
						return YesNoCancel.Cancel;
				}
			}
			return default(YesNoCancel);
		}

		public void Busy(Internals.BusyType which)
		{
		}
	}
}
