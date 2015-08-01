using RDotNet;
using System;
using System.Collections;

namespace SimpleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string rHome = null;
            string rPath = null;
            if (args.Length > 0)
                rPath = args[0];
            if (args.Length > 1)
                rHome = args[1];

            Console.WriteLine(RDotNet.NativeLibrary.NativeUtility.FindRPaths(ref rPath, ref rHome));

            rHome = null;
            rPath = null;

            REngine.SetEnvironmentVariables(rPath: rPath, rHome: rHome);
            REngine e = REngine.GetInstance();

            Console.WriteLine(RDotNet.NativeLibrary.NativeUtility.SetEnvironmentVariablesLog);

            counter = 0;
            for (int i = 0; i < 6; i++)
                TestDataFrameInMemoryCreation(e);

            for (int i = 0; i < 6; i++)
                TestCallStop(e);

            e.Dispose();
        }

        private static int counter = 0;

        private static void TestCallStop(REngine engine)
        {
            try
            {
                engine.Evaluate("stop('Just stop')");
            }
            catch (Exception ex)
            {
                counter++;
                Console.WriteLine(string.Format("Caught an exception ({0})", counter));
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Recovered from evaluation exception?");
        }

        private static void TestDataFrameInMemoryCreation(REngine engine)
        {
            IEnumerable[] columns;
            string[] columnNames;
            DataFrame df;
            createTestDataFrame(engine, out columns, out columnNames, out df);
            df = engine.CreateDataFrame(columns, columnNames: null);
            columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            try
            {
                df = engine.CreateDataFrame(columns, columnNames: null);
            }
            catch (Exception ex)
            {
                counter++;
                Console.WriteLine(string.Format("Caught an exception ({0})", counter));
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("Recovered from evaluation exception?");
        }

        private static void createTestDataFrame(REngine engine, out IEnumerable[] columns, out string[] columnNames, out DataFrame df)
        {
            columns = createTestDfColumns();
            columnNames = new[] { "Category", "No.", "Measure" };
            df = engine.CreateDataFrame(columns, columnNames: columnNames);
        }

        private static IEnumerable[] createTestDfColumns()
        {
            IEnumerable[] columns = new IEnumerable[3];
            columns[0] = new string[] { "a", "a", "a", "a", "a", "b", "b", "b", "b", "b" };
            columns[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            columns[2] = new double[] { 1.1, 2.1, 3.1, 4.1, 5.1, 6.1, 7.1, 8.1, 9.1, 10.1 };
            return columns;
        }

        private static void TestPlot(REngine e)
        {
            e.Evaluate("library(DAAG)");
            for (int i = 0; i < 100; i++)
            {
                e.Evaluate("p <- plot(northing ~ easting, data=frogs, pch=c(1,16)[frogs$pres.abs+1], xlab='Meters east of reference point', ylab='Meters north')");
                e.Evaluate(string.Format("print(paste('plot iteration number', {0}))", i));
            }
        }

        private static void TestPendingFinalizersThreadingIssues(REngine e)
        {
            e.Evaluate("f <- function(a) {if (length(a)!= 1) stop('What goes on?')}");
            var f = e.Evaluate("f").AsFunction();
            try
            {
                e.Evaluate("f(letters[1:3])");
            }
            catch (EvaluationException)
            {
            }
            f.Invoke(e.CreateCharacterVector(new[] { "blah" }));
            try
            {
                f.Invoke(e.CreateCharacterVector(new[] { "blah", "blah" }));
            }
            catch (EvaluationException)
            {
                Console.WriteLine("Caught the expected exception");
            }
            f = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            e.Dispose();
            Console.WriteLine("Just waiting for crash...");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}