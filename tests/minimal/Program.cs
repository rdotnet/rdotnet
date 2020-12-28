using System;

namespace minimal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Minimal program with System.Environment.Version => " + System.Environment.Version.ToString());
            Console.WriteLine("System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription => " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.ToString());
        }
    }
}
