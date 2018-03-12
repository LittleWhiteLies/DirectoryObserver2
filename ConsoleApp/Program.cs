using DirectoryObserver;
using System;

namespace ConsoleApp
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            ObserverClass clsLib = new ObserverClass();
            
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
                    clsLib.Observe(directoryPath, frequency, (result) => Console.WriteLine(result));
                }
                else if (cmd == "stop")
                {
                    clsLib.StopObservation(directoryPath);
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
