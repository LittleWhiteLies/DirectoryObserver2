using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryObserver
{
    public class ObserverClass2
    {
        List<Directory> observableDirectories = new List<Directory>();
        private bool _isObserverRunning = false;

        public void StartObserveDirectory(string directoryPath, int checkFreq, Action<string> callback)
        {
            observableDirectories.Add(new Directory() {
                                                        directoryPath = directoryPath,
                                                        lastCheckTime = DateTime.Now,
                                                        checkFrequesy = checkFreq,
                                                        callback = callback });

            if (_isObserverRunning != true)
            {
                Task.Run(() => {
                    StartObservation();
                });
            }
        }

        public void StopObserveDirectory(string directoryPath)
        {
            observableDirectories.Remove(observableDirectories.Find(q => q.directoryPath == directoryPath));
        }

        public async Task StartObservation()
        {
            _isObserverRunning = true;

            while (observableDirectories.Count > 0)
            {
                List<Directory> directories = new List<Directory>();
                directories.AddRange(observableDirectories);

                //get all directories to observe
                foreach ( var dir in directories.FindAll(q => q.lastCheckTime.AddMilliseconds(q.checkFrequesy) <= DateTime.Now))
                {
                    //start tasks to observe directory
                    CheckDirectory(dir.directoryPath, dir.lastCheckTime, dir.callback);
                }
            }

            _isObserverRunning = false;
        }

        private async Task CheckDirectory(string directoryPath, DateTime lastCheckTime, Action<string> callback)
        {
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            FileSystemInfo[] files = di.GetFileSystemInfos();

            var tmp = files.ToList<FileSystemInfo>();

            var newFiles = files.Where(f => f.CreationTime > lastCheckTime).ToList<FileSystemInfo>();

            if (newFiles.Any())
            {
                foreach (var file in newFiles)
                {
                    callback("directory: " + directoryPath + ", fileName: " + file.Name + ", created");
                }
            }

            var modifiedFiles = files.Where(f => f.LastWriteTime > lastCheckTime && f.CreationTime < f.LastWriteTime);

            if (modifiedFiles.Any())
            {
                foreach (var file in modifiedFiles)
                {
                    callback("directory: " + directoryPath + ", fileName: " + file.Name + ", changed");
                }
            }

            //increment directory last check datetime
            observableDirectories.Find(q => q.directoryPath == directoryPath).lastCheckTime = DateTime.Now;
        }

        public class Directory
        {
            public string directoryPath { get; set; }

            public DateTime lastCheckTime { get; set; }

            public int checkFrequesy { get; set; }

            public Action<string> callback { get; set; }
        }
    }
}
