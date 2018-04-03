using DirectoryObserver;
using System;

namespace ConsoleApp2
{
    public class Program2
    {
        public static void Main(string[] args)
        {
            ObserverClass2 clsLib = new ObserverClass2();

            string cmd = null;
            string directoryPath = null;
            int frequency = 1000; //ms

            while (true)
            {
                Console.WriteLine(@"Please enter command ('start' or 'stop'): ");
                cmd = Console.ReadLine().Trim();
                Console.WriteLine(@"Please enter directory: ");
                directoryPath = Console.ReadLine().Trim();

                if (cmd == "start")
                {
                    Console.WriteLine("Starting " + directoryPath + " observation...");
                    clsLib.StartObserveDirectory(directoryPath, frequency, (result) => Console.WriteLine(result));
                }
                else if (cmd == "stop")
                {
                    clsLib.StopObserveDirectory (directoryPath);
                    Console.WriteLine("... directory " + directoryPath + " observation is stoped");
                }
                else
                {
                    Console.WriteLine("Unknown command: " + cmd);
                }
            }
        }
    }
}
