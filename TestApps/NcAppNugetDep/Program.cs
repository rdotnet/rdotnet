using RDotNet;

namespace NcAppNugetDep
{
    class Program
    {
        static void Main(string[] args)
        {
            REngine.SetEnvironmentVariables();
            using (var engine = REngine.GetInstance())
            {
                engine.AutoPrint = true;
                var vector = engine.Evaluate("x <- character(100)").AsCharacter();
                vector[1] = "foo";
                vector[2] = "bar";
                var second = engine.Evaluate("x[2]").AsCharacter().ToArray();
                var third = engine.Evaluate("x[3]").AsCharacter().ToArray();
            }
        }
    }
}
