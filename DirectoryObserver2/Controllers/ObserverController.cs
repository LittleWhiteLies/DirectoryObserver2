using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryObserver2.Controllers
{
    [Route("api/Observer")]
    public class ObserverController : Controller
    {
        public ObserverController()
        {
        }

        [HttpGet]
        public void Observe(string directory)
        {
            //string directory = @"c:/temp";

            //DoWorkAsyncInfiniteLoop(directory, (result) => System.Diagnostics.Trace.WriteLine(result));
            DoWorkAsyncInfiniteLoop(directory, (result) => Console.WriteLine(result));

            return;
        }

        public async Task DoWorkAsyncInfiniteLoop(string directoryPath, Action<string> callback)
        {
            DateTime timestamp = System.DateTime.Now;

            while (true)
            {
                DateTime nextTimestamp = System.DateTime.Now;
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                FileSystemInfo[] files = di.GetFileSystemInfos();

                var newFiles = files.Where(f => f.CreationTime > timestamp);

                if (newFiles.Any())
                {
                    foreach (var file in newFiles)
                    {
                        callback("fileName: " + file.Name + ", created");
                    }
                }

                var modifiedFiles = files.Where(f => f.LastWriteTime > timestamp && f.CreationTime < f.LastWriteTime);

                if (modifiedFiles.Any())
                {
                    foreach (var file in modifiedFiles)
                    {
                        callback("fileName: " + file.Name + ", changed" );
                    }
                }

                await Task.Delay(5000);


                timestamp = nextTimestamp;

            };            
        }
    }
}