using System;
using System.Text;

namespace RDotNet.R.Adapter.Graphics
{
    public class NullCharacterDevice : ICharacterDevice
    {
        public bool ReadConsole(string prompt, StringBuilder buffer, int length, bool history)
        {
            return false;
        }

        public void WriteConsole(string output, int length)
        {
        }

        public void WriteConsoleEx(string output, int length, ConsoleOutputType outputType)
        {
        }

        public void ShowMessage(string message)
        {
        }

        public void Busy(BusyType which)
        {
        }

        public void Callback()
        {
        }

        public YesNoCancel Ask(string question)
        {
            return default(YesNoCancel);
        }

        public void Suicide(string message)
        {
            CleanUp(StartupSaveAction.Suicide, 2, false);
        }

        public void Clear()
        {
        }

        public void Flush()
        {
        }

        public void ClearErrorConsole()
        {
        }

        public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
        {
            Environment.Exit(status);
        }

        public bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager)
        {
            return false;
        }

        public string ChooseFile(bool create)
        {
            return null;
        }

        public void EditFile(string file)
        {
        }

        public SEXPREC LoadHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment)
        {
            return new SEXPREC();
        }

        public SEXPREC SaveHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment)
        {
            return new SEXPREC();
        }

        public SEXPREC AddHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment)
        {
            return new SEXPREC();
        }
    }
}
