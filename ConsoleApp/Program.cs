using ClassLibrary1;
using System.Threading;

namespace ConsoleApp
{
    public class Program
    {
        static Class1 clsLib = new Class1();
        
        public static void Main(string[] args)
        {
            ObserveDirectory(@"c:/temp");

            Thread.Sleep(10000);

            ObserveDirectory(@"c:/temp2");

            Thread.Sleep(20000);

            StopObservingDirectory("c:/temp2");

            Thread.Sleep(20000);

            StopObservingDirectory("c:/temp");

            Thread.Sleep(20000);
        }

        private static void ObserveDirectory(string directory)
        {
            clsLib.Observe(directory);
        }

        private static void StopObservingDirectory(string directory)
        {
            clsLib.StopObservation(directory);            
        }
    }
}
