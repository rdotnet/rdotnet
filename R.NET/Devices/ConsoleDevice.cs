using RDotNet.Internals;
using System;
using System.IO;

namespace RDotNet.Devices
{
    /// <summary>
    /// The default IO device, using the System.Console
    /// </summary>
    public class ConsoleDevice : ICharacterDevice
    {
        #region ICharacterDevice Members

        /// <summary>
        /// Read input from console.
        /// </summary>
        /// <param name="prompt">The prompt message.</param>
        /// <param name="capacity">Parameter is ignored</param>
        /// <param name="history">Parameter is ignored</param>
        /// <returns>The input.</returns>
        public string ReadConsole(string prompt, int capacity, bool history)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        /// <summary>
        /// Write output on console.
        /// </summary>
        /// <param name="output">The output message</param>
        /// <param name="length">Parameter is ignored</param>
        /// <param name="outputType">Parameter is ignored</param>
        public void WriteConsole(string output, int length, ConsoleOutputType outputType)
        {
            Console.Write(output);
        }

        /// <summary>
        /// Displays the message to the System.Console.
        /// </summary>
        /// <param name="message">The message.</param>
        public void ShowMessage(string message)
        {
            Console.Write(message);
        }

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
        /// Ask a question to the user with three choices.
        /// </summary>
        /// <param name="question">The question to write to the console</param>
        /// <returns></returns>
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

        /// <summary>
        /// Write the message to standard error output stream.
        /// </summary>
        /// <param name="message"></param>
        public void Suicide(string message)
        {
            Console.Error.WriteLine(message);
            CleanUp(StartupSaveAction.Suicide, 2, false);
        }

        /// <summary>
        /// Clears the System.Console
        /// </summary>
        public void ResetConsole()
        {
            Console.Clear();
        }

        /// <summary>
        /// Flush the System.Console
        /// </summary>
        public void FlushConsole()
        {
            Console.Write(string.Empty);
        }

        /// <summary>
        /// Clears the System.Console
        /// </summary>
        public void ClearErrorConsole()
        {
            Console.Clear();
        }

        /// <summary>
        /// Terminate the process with the given status
        /// </summary>
        /// <param name="saveAction">Parameter is ignored</param>
        /// <param name="status">The status code on exit</param>
        /// <param name="runLast">Parameter is ignored</param>
        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Environment.Exit(status);
        }

        /// <summary>
        /// Displays the contents of files.
        /// </summary>
        /// <remarks>
        /// Only Unix.
        /// </remarks>
        /// <param name="files">The file paths.</param>
        /// <param name="headers">The header before the contents is printed.</param>
        /// <param name="title">Ignored by this implementation</param>
        /// <param name="delete">Whether the file will be deleted.</param>
        /// <param name="pager">Ignored by this implementation</param>
        /// <returns>true on successful completion, false if an IOException was caught</returns>
        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            int count = files.Length;
            for (int index = 0; index < count; index++)
            {
                try
                {
                    Console.WriteLine(headers);
                    Console.WriteLine(File.ReadAllText(files[index]));
                    if (delete)
                    {
                        File.Delete(files[index]);
                    }
                }
                catch (IOException)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Chooses a file.
        /// </summary>
        /// <remarks>
        /// Only Unix.
        /// </remarks>
        /// <param name="create">To be created.</param>
        /// <returns>The length of input.</returns>
        public string ChooseFile(bool create)
        {
            string path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            if (create && !File.Exists(path))
            {
                File.Create(path).Close();
            }
            if (File.Exists(path))
            {
                return path;
            }
            return null;
        }

        /// <summary>
        /// This implementation does nothing
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