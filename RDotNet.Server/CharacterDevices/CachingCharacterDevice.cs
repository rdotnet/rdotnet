using System.Collections.Generic;
using System.Text;
using RDotNet.R.Adapter;

namespace RDotNet.Server.CharacterDevices
{
    public class CachingCharacterDevice : ICharacterDevice, ICachedOutput
    {
        private readonly List<string> _output = new List<string>();
        private string _pending = string.Empty;

        public IEnumerable<string> GetOutput()
        {
            return _output;
        }

        public bool ReadConsole(string prompt, StringBuilder buffer, int length, bool history)
        {
            _output.Add(prompt);
            return false;
        }

        public void WriteConsole(string output, int length)
        {
            WriteConsoleEx(output, length, ConsoleOutputType.Output);
        }

        public void WriteConsoleEx(string output, int length, ConsoleOutputType outputType)
        {
            _pending += output;
            if (output.IndexOfAny(new[] {'\r', '\n'}) == 0)
            {
                _output.Add(_pending);
                _pending = string.Empty;
            }
        }

        public void ShowMessage(string message)
        {
            _output.Add(message);
        }

        public void Busy(BusyType which)
        { }

        public void Callback()
        { }

        public YesNoCancel Ask(string question)
        {
            return YesNoCancel.Yes;
        }

        public void Suicide(string message)
        {
            CleanUp(StartupSaveAction.Suicide, 2, false);
        }

        public void Clear()
        {
            _pending = string.Empty;
            _output.Clear();
        }

        public void Flush()
        {
            _pending = string.Empty;
        }

        public void ClearErrorConsole()
        {
            Clear();
        }

        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        { }

        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            return true;
        }

        public string ChooseFile(bool create)
        {
            return string.Empty;
        }

        public void EditFile(string file)
        { }

        public SEXPREC LoadHistory(SEXPREC call,
                                                    SEXPREC operation,
                                                    SEXPREC args,
                                                    SEXPREC environment)
        {
            return new SEXPREC();
        }

        public SEXPREC SaveHistory(SEXPREC call,
                                                    SEXPREC operation,
                                                    SEXPREC args,
                                                    SEXPREC environment)
        {
            return new SEXPREC();
        }

        public SEXPREC AddHistory(SEXPREC call,
                                                    SEXPREC operation,
                                                    SEXPREC args,
                                                    SEXPREC environment)
        {
            return new SEXPREC();
        }
    }

    public interface ICachedOutput
    {
        IEnumerable<string> GetOutput();
    }
}
