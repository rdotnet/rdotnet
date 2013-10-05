using RDotNet.Internals;
using System;

namespace RDotNet.Devices
{
   /// <summary>
   /// The default IO device.
   /// </summary>
   public class NullCharacterDevice : ICharacterDevice
   {
      #region ICharacterDevice Members

      public string ReadConsole(string prompt, int capacity, bool history)
      {
         return null;
      }

      public void WriteConsole(string output, int length, ConsoleOutputType outputType)
      { }

      public void ShowMessage(string message)
      { }

      public void Busy(BusyType which)
      { }

      public void Callback()
      { }

      public YesNoCancel Ask(string question)
      {
         return default(YesNoCancel);
      }

      public void Suicide(string message)
      {
         CleanUp(StartupSaveAction.Suicide, 2, false);
      }

      public void ResetConsole()
      { }

      public void FlushConsole()
      { }

      public void ClearErrorConsole()
      { }

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
      { }

      public SymbolicExpression LoadHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
      {
         return environment.Engine.NilValue;
      }

      public SymbolicExpression SaveHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
      {
         return environment.Engine.NilValue;
      }

      public SymbolicExpression AddHistory(Language call, SymbolicExpression operation, Pairlist args, REnvironment environment)
      {
         return environment.Engine.NilValue;
      }

      #endregion ICharacterDevice Members
   }
}