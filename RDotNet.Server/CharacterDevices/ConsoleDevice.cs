using System;
using System.IO;
using System.Text;
using RDotNet.R.Adapter;

namespace RDotNet.Server.CharacterDevices
{
   public class ConsoleDevice : ICharacterDevice
   {
      public bool ReadConsole(string prompt, StringBuilder buffer, int length, bool history)
      {
         Console.Write(prompt);
         return !string.IsNullOrEmpty(Console.ReadLine());
      }

      public void WriteConsole(string output, int length)
      {
          WriteConsoleEx(output, length, ConsoleOutputType.Output);
      }

      public void WriteConsoleEx(string output, int length, ConsoleOutputType outputType)
      {
         Console.Write(output);
      }

      public void ShowMessage(string message)
      {
         Console.Write(message);
      }

      public void Busy(BusyType which)
      { }

      public void Callback()
      { }

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

      public void Suicide(string message)
      {
         Console.Error.WriteLine(message);
         CleanUp(StartupSaveAction.Suicide, 2, false);
      }

      public void Clear()
      {
         Console.Clear();
      }

      public void Flush()
      {
         Console.Write(string.Empty);
      }

      public void ClearErrorConsole()
      {
         Console.Clear();
      }

      public void CleanUp(StartupSaveAction saveAction, int status, bool runLast)
      {
         Environment.Exit(status);
      }

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
}
