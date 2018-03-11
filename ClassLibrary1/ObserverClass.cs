using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryObserver
{
    public class ObserverClass
    {
        public Dictionary<string, CancellationTokenSource> cancelationTokens = new Dictionary<string, CancellationTokenSource>();

        public void Observe(string directory, Action<string> callback)
        {
            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            cancelationTokens.Add(directory, cancelationToken);
            //DoWorkAsyncInfiniteLoop(directory, cancelationToken, (result) => System.Diagnostics.Trace.WriteLine(result));

            DoWorkAsyncInfiniteLoop(directory, cancelationToken, callback);
            
            return;
        }

        public async Task DoWorkAsyncInfiniteLoop(string directoryPath, CancellationTokenSource cancelationToken, Action<string> callback)
        {
            DateTime timestamp = System.DateTime.Now;

            while (!cancelationToken.Token.IsCancellationRequested)
            {
                DateTime nextTimestamp = System.DateTime.Now;
                DirectoryInfo di = new DirectoryInfo(directoryPath);
                FileSystemInfo[] files = di.GetFileSystemInfos();

                var newFiles = files.Where(f => f.CreationTime > timestamp).ToList<FileSystemInfo>();

                if (newFiles.Any())
                {
                    foreach (var file in newFiles)
                    {
                        callback("directory: " + directoryPath + ", fileName: " + file.Name + ", created");
                    }
                }

                var modifiedFiles = files.Where(f => f.LastWriteTime > timestamp && f.CreationTime < f.LastWriteTime);

                if (modifiedFiles.Any())
                {
                    foreach (var file in modifiedFiles)
                    {
                        callback("directory: " + directoryPath + ", fileName: " + file.Name + ", changed");
                    }
                }

                await Task.Delay(1);
                
                timestamp = nextTimestamp;
            };
        }

        public void StopObservation(string directory)
        {
            CancellationTokenSource cancelationToken = cancelationTokens.Where(q => q.Key == directory).Select(e => e.Value).FirstOrDefault();
            cancelationToken.Cancel();
        }
    }
}
