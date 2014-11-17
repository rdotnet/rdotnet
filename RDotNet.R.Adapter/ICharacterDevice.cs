using System.Text;

namespace RDotNet.R.Adapter
{
    public interface ICharacterDevice
    {
        bool ReadConsole(string prompt, StringBuilder buffer, int length, bool history);
        void WriteConsole(string output, int length);
        void WriteConsoleEx(string output, int length, ConsoleOutputType outputType);
        void ShowMessage(string message);
        void Busy(BusyType which);
        void Callback();
        YesNoCancel Ask(string question);
        void Suicide(string message);
        void Clear();
        void Flush();
        void ClearErrorConsole();
        void CleanUp(StartupSaveAction saveAction, int status, bool runLast);
        bool ShowFiles(string[] files, string[] headers, string title, bool delete, string pager);
        string ChooseFile(bool create);
        void EditFile(string file);
        SEXPREC LoadHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment);
        SEXPREC SaveHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment);
        SEXPREC AddHistory(SEXPREC call, SEXPREC operation, SEXPREC args, SEXPREC environment);
    }
}
