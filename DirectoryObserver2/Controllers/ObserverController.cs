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
        public void Observe()
        {
            string directory = @"c:/temp";
            
            DoWorkAsyncInfiniteLoop(directory, (result) => System.Diagnostics.Trace.WriteLine(result)/*Console.WriteLine(result)*/);

            return;
        }

        public async Task DoWorkAsyncInfiniteLoop(string directoryPath, Action<string> callback)
        {
            DateTime timestamp = System.DateTime.Now;

            while (true)
            {
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                FileSystemInfo[] files = di.GetFileSystemInfos();

                var newFiles = files.Where(f => f.CreationTime > timestamp);

                if (newFiles.Any())
                {
                    foreach (var file in newFiles)
                    {
                        callback("fileName: " + file.FullName + ", changeDate: " + file.CreationTime);
                    }
                }

                var modifiedFiles = files.Where(f => f.LastWriteTime > timestamp);

                if (modifiedFiles.Any())
                {
                    foreach (var file in modifiedFiles)
                    {
                        callback("fileName: " + file.FullName + ", changeDate: " + file.LastWriteTime);
                    }
                }

                timestamp = System.DateTime.Now;

                await Task.Delay(10000);
            };            
        }
    }
}