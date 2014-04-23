using System.IO;
using System.Windows.Forms;

namespace RDotNet.Graphics
{
   internal class Program
   {
      public const string EngineName = "RDotNetTest";

      private static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         var path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".png");
         Application.ThreadExit += (sender, e) =>
         {
            try
            {
               File.Delete(path);
            }
            finally { }
         };
         var form = new GraphForm()
         {
            Code = @"plot(1:10, pch=1:10, col=1:10, cex=seq(1, 2, length=10))",
            TempImagePath = path,
         };
         REngine.SetEnvironmentVariables();
         var engine = REngine.GetInstance();
         Application.ThreadExit += (sender, e) => engine.Dispose();
         engine.Initialize();
         engine.Install(form.graphPanel);
         Application.Run(form);
      }
   }
}