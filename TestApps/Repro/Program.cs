using RDotNet;
using RDotNet.NativeLibrary;
using System;
using System.Collections;

namespace SimpleTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            github_issue_90();

            //string rHome = null;
            //string rPath = null;
            //if (args.Length > 0)
            //    rPath = args[0];
            //if (args.Length > 1)
            //    rHome = args[1];

            //NativeUtility util = new NativeUtility();
            //Console.WriteLine(util.FindRPaths(ref rPath, ref rHome));

            //rHome = null;
            //rPath = null;

            //REngine.SetEnvironmentVariables(rPath: rPath, rHome: rHome);
            //REngine e = REngine.GetInstance();

            //Console.WriteLine(NativeUtility.SetEnvironmentVariablesLog);

            //counter = 0;
            //for (int i = 0; i < 6; i++)
            //    TestDataFrameInMemoryCreation(e);

            //for (int i = 0; i < 6; i++)
            //    TestCallStop(e);

            //e.Dispose();
        }

        private static void github_issue_90()
        {

            string rHome = @"c:\Program Files\R\R-3.4.4\";
            string rPath = @"C:\Program Files\R\R-3.4.4\bin\x64";

            REngine.SetEnvironmentVariables(rPath: rPath, rHome: rHome);

            //p.RHome = @"c:\Program Files\Microsoft\R Open\R-3.4.3\";
            REngine engine = REngine.GetInstance();

            engine.Evaluate("library(dplyr)");
            engine.Evaluate("library(keras)");
            engine.Evaluate("model <- keras_model_sequential() %>% layer_dense(units = 1000, input_shape = c(1000)) %>% compile(loss = 'mse',optimizer = 'adam')");

            int counter = 0;

            while (true)
            {

                Console.WriteLine(counter++);

                IntegerVector vec = engine.CreateIntegerVector(1000);
                vec.SetVector(new int[1000]);
                engine.SetSymbol("vec1000", vec);

                var execution = "predict(model, t(vec1000))";
                using (var output = engine.Evaluate(execution))
                {
                    using (var a = output.AsList())
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            using (var b = a[i].AsNumeric())
                            {

                            }
                        }
                    }
                };
            }
        }
    }
}