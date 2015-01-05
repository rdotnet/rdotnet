using RDotNet.NativeLibrary;
using System;

namespace RDotNet.SetEnv.Tests
{
    public class TestSetEnvironment
    {
        public static void ConsoleTestFindRPath(string expectContains)
        {
            TestFindRPath(expectContains);
            Console.WriteLine("Enter any key to finish");
            Console.ReadKey();
        }

        public static void TestFindRPath(string expectContains)
        {
            var rPath = NativeUtility.FindRPath();

            var match = rPath.Contains(expectContains);
            Console.WriteLine("Detected RPath is {0}", rPath);
            if (match)
                Console.WriteLine("Contains string '{0}' as expected", expectContains);
            else
                Console.WriteLine("Does NOT contain string '{0}' as expected", expectContains);
        }
    }
}