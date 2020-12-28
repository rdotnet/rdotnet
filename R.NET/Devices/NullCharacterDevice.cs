using RDotNet.Internals;
using System;

namespace RDotNet.Devices
{
    /// <summary>
    /// A sink with (almost) no effect, similar in purpose to /dev/null
    /// </summary>
    public class NullCharacterDevice : ICharacterDevice
    {
        #region ICharacterDevice Members

        /// <summary>
        /// Read input from console.
        /// </summary>
        /// <param name="prompt">The prompt message.</param>
        /// <param name="capacity">The buffer's capacity in byte.</param>
        /// <param name="history">Whether the input should be added to any command history.</param>
        /// <returns>A null reference</returns>
        public string ReadConsole(string prompt, int capacity, bool history)
        {
            return null;
        }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        /// <param name="output">The output message</param>
        /// <param name="length">The output's length in byte.</param>
        /// <param name="outputType">The output type.</param>
        public void WriteConsole(string output, int length, ConsoleOutputType outputType)
        { }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        /// <param name="message">The message.</param>
        public void ShowMessage(string message)
        { }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        /// <param name="which"></param>
        public void Busy(BusyType which)
        { }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        public void Callback()
        { }

        /// <summary>
        /// Always return the default value of the YesNoCancel enum (yes?)
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public YesNoCancel Ask(string question)
        {
            return default(YesNoCancel);
        }

        /// <summary>
        /// Ignores the message, but triggers a CleanUp, a termination with no action.
        /// </summary>
        /// <param name="message"></param>
        public void Suicide(string message)
        {
            CleanUp(StartupSaveAction.Suicide, 2, false);
        }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        public void ResetConsole()
        { }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        public void FlushConsole()
        { }

        /// <summary>
        /// This implementation has no effect
        /// </summary>
        public void ClearErrorConsole()
        { }

        /// <summary>
        /// Clean up action; exit the process with a specified status
        /// </summary>
        /// <param name="saveAction">Ignored</param>
        /// <param name="status"></param>
        /// <param name="runLast">Ignored</param>
        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Environment.Exit(status);
        }

        /// <summary>
        /// Always returns false, no other side effect
        /// </summary>
        /// <param name="files"></param>
        /// <param name="headers"></param>
        /// <param name="title"></param>
        /// <param name="delete"></param>
        /// <param name="pager"></param>
        /// <returns>Returns false</returns>
        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            return false;
        }

        /// <summary>
        /// Always returns null; no other side effect
        /// </summary>
        /// <param name="create">ignored</param>
        /// <returns>null</returns>
        public string ChooseFile(bool create)
        {
            return null;
        }

        /// <summary>
        /// No effect
        /// </summary>
        /// <param name="file"></param>
        public void EditFile(string file)
        { }

        /// <summary>
        /// Return the NULL SEXP; no other effect
        /// </summary>
        /// <param name="call"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public SymbolicExpression LoadHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        /// <summary>
        /// Return the NULL SEXP; no other effect
        /// </summary>
        /// <param name="call"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public SymbolicExpression SaveHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        /// <summary>
        /// Return the NULL SEXP; no other effect
        /// </summary>
        /// <param name="call"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public SymbolicExpression AddHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            return environment.Engine.NilValue;
        }

        #endregion ICharacterDevice Members
    }
}