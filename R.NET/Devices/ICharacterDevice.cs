#if WINDOWS
using System.Text;
using RDotNet.Internals;

namespace RDotNet.Devices
{
	/// <summary>
	/// A console class handles user's inputs and outputs.
	/// </summary>
	public interface ICharacterDevice
	{
		/// <summary>
		/// Read input from console.
		/// </summary>
		/// <param name="prompt">The prompt message.</param>
		/// <param name="buffer">The reading buffer.</param>
		/// <param name="capacity">The buffer's capacity in byte.</param>
		/// <param name="history">Whether the input should be added to any command history.</param>
		/// <returns><c>False</c> when no input available; otherwise <c>True</c>.</returns>
		bool ReadConsole(string prompt, StringBuilder buffer, int capacity, bool history);

		/// <summary>
		/// Write output on console.
		/// </summary>
		/// <param name="output">The output message</param>
		/// <param name="length">The output's length in byte.</param>
		/// <param name="outputType">The output type.</param>
		void WriteConsole(string output, int length, ConsoleOutputType outputType);

		/// <summary>
		/// Callback function.
		/// </summary>
		void Callback();

		/// <summary>
		/// Displays the message.
		/// </summary>
		/// <remarks>
		/// It should be brought to the user's attention immediately. 
		/// </remarks>
		/// <param name="message">The message.</param>
		void ShowMessage(string message);

		/// <summary>
		/// Asks user's decision.
		/// </summary>
		/// <param name="question">The question.</param>
		/// <returns>User's decision.</returns>
		YesNoCancel Ask(string question);

		/// <summary>
		/// Invokes actions.
		/// </summary>
		/// <param name="which">The state.</param>
		void Busy(BusyType which);
	}
}
#endif
