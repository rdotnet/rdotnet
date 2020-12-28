using RDotNet.Devices;
using RDotNet.Internals;
using System;
using System.Text;

namespace RDotNet
{
    public class MockDevice : ICharacterDevice
    {
        private readonly StringBuilder builder;

        public MockDevice()
        {
            this.builder = new StringBuilder();
        }

        public string Input { get; set; }

        public YesNoCancel Answer { get; set; }

        #region ICharacterDevice Members

        public string ReadConsole(string prompt, int capacity, bool history)
        {
            this.builder.Append(prompt);
            return Input;
        }

        public void WriteConsole(string output, int length, ConsoleOutputType outputType)
        {
            this.builder.Append(output);
        }

        public void ShowMessage(string message)
        {
            this.builder.Append(message);
        }

        public void Busy(BusyType which)
        {
            //throw new NotImplementedException();
        }

        public void Callback()
        { }

        public YesNoCancel Ask(string question)
        {
            this.builder.Append(question);
            return Answer;
        }

        public void Suicide(string message)
        {
            throw new Exception(message);
        }

        public void ResetConsole()
        {
            this.builder.Clear();
        }

        public void FlushConsole()
        {
            //throw new NotImplementedException();
        }

        public void ClearErrorConsole()
        {
            //throw new NotImplementedException();
        }

        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Environment.Exit(status);
        }

        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            throw new NotImplementedException();
        }

        public string ChooseFile(bool create)
        {
            throw new NotImplementedException();
        }

        public void EditFile(string file)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpression LoadHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpression SaveHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new NotImplementedException();
        }

        public SymbolicExpression AddHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
        {
            throw new NotImplementedException();
        }

        #endregion ICharacterDevice Members

        public void Initialize()
        {
            this.builder.Clear();
            Input = null;
            Answer = YesNoCancel.Cancel;
        }

        public string GetString()
        {
            return this.builder.ToString();
        }
    }
}