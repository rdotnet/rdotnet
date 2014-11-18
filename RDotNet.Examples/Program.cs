using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using RDotNet.Client;

namespace RDotNet.Examples
{
    internal class Program
    {
        private static void Main()
        {
            WebConfigurationManager.AppSettings["binPath"] = @"..\..\..\RDotNet.ProcessHost\bin\debug\";
            const int singleInstanceId = 16;
            EngineExamples(singleInstanceId);

            var ids = new[] { 1, 2, 3, 4 };
            Task.WaitAll(ids.Select(EngineExamplesAsync).ToArray());
        }

        private async static Task EngineExamplesAsync(int instanceId)
        {
           await Task.Run(() => EngineExamples(instanceId));
        }

        private static void EngineExamples(int instanceId)
        {
            var factory = new REngineFactory();
            var engine = factory.CreateInstance(instanceId);
            engine.ResetConsole();
            engine.ResetPlots();

            var statement = "1 + 1 ";
            Console.Write("Evaluating {0}", statement);
            var result = engine.Evaluate("1 + 1 ");
            var vector = result.ToIntegerVector();
            Console.WriteLine(" returned '{0}' as a result", vector[0]);

            engine.ResetConsole();
            engine.ResetPlots();

            var statements = new[]
            {
                "x <- c(1,2,3,4,5)",
                "y <- c(5,2,3,5,8)",
                "df <- data.frame(x,y)",
                "model1 <- lm(x ~ y, data=df)",
                "par(mfrow=c(2,2))",
                "plot(model1)"
            };
            statement = string.Join("\n", statements);
            Console.WriteLine("Evaluating {0}", statement);
            engine.Evaluate(statement);
            Console.WriteLine("Generated {0} bytes of svg plot data", engine.GetPlots()[0].Length);
        }
    }
}
