using DirectoryObserver2.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldDetectCreatedFile()
        {
            string directory = @"C:/temp";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedResult = null;

            if ((File.Exists(directory + @"/" + fileName)))
            {
                Directory.Delete(directory, true);

                Directory.CreateDirectory(directory);
            }            

            var controller = new ObserverController();
            
            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            controller.DoWorkAsyncInfiniteLoop(directory, cancelationToken, (clbkResult) => result = clbkResult);

            System.Threading.Thread.Sleep(5000);

            File.Create(directory + @"/" + fileName);
                        
            expectedResult = @"fileName: " + fileName + ", created";

            while (result == null)
            {   
                System.Threading.Thread.Sleep(1000);
            }
            
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void ShouldDetectChangedFile()
        {
            string directory = @"C:/temp";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedResult = null;

            if (!(File.Exists(directory + @"/" + fileName)))
            {
                File.Create(directory + @"/" + fileName);
            }

            var controller = new ObserverController();

            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            controller.DoWorkAsyncInfiniteLoop(directory, cancelationToken, (clbkResult) => result = clbkResult);

            System.Threading.Thread.Sleep(5000);

            //File.WriteAllText(directory + @"/" + fileName, "This is test");
            File.CreateText(directory + @"/" + fileName);

            expectedResult = @"fileName: " + fileName + ", changed";

            while (result == null)
            {
                System.Threading.Thread.Sleep(1000);
            }

            Assert.AreEqual(result, expectedResult);
        }



        [TestMethod]
        public void ShouldTerminateOneOfTheRunningThreads()
        {
            string directory1 = @"C:/temp";
            string fileName = @"unitTestFile.txt";
            string directory2 = @"C:/temp2";
            
            if (!(File.Exists(directory1 + @"/" + fileName)))
            {
                File.Create(directory1 + @"/" + fileName);
            }

            if (!(File.Exists(directory2 + @"/" + fileName)))
            {
                File.Create(directory2 + @"/" + fileName);
            }

            var controller = new ObserverController();
            var result = "";

            int maxThreads;
            int completionPortThreads;
            ThreadPool.GetMaxThreads(out maxThreads, out completionPortThreads);

            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            var tsk1 = controller.DoWorkAsyncInfiniteLoop(directory1, cancelationToken, (clbkResult) => result = clbkResult);

            System.Threading.Thread.Sleep(1000);
            
            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);

            CancellationTokenSource cancelationToken2 = new CancellationTokenSource();

            var tsk2 = controller.DoWorkAsyncInfiniteLoop(directory2, cancelationToken2, (clbkResult) => result = clbkResult);

            System.Threading.Thread.Sleep(1000);

            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            Assert.AreNotEqual(tsk2.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            
            File.CreateText(directory2 + @"/" + fileName);

            File.CreateText(directory1 + @"/" + fileName);

            controller.StopObservation(cancelationToken2);
            
            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            Assert.AreEqual(tsk2.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
        }
    }
}
