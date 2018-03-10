using ClassLibrary1;
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
            string directory = @"C:/temp4";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedResult = null;

            if ((File.Exists(directory + @"/" + fileName)))
            {
                Directory.Delete(directory, true);
            }
            
            Directory.CreateDirectory(directory);

            Class1 clsLib = new Class1();

            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            clsLib.DoWorkAsyncInfiniteLoop(directory, cancelationToken, (clbkResult) => result = clbkResult);

            Thread.Sleep(5000);

            File.Create(directory + @"/" + fileName);
                        
            expectedResult = @"directory: " + directory + ", fileName: " + fileName + ", created";

            while (result == null)
            {   
                Thread.Sleep(1000);
            }
            
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void ShouldDetectChangedFile()
        {
            string directory = @"C:/temp3";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedResult = null;

            if (!(File.Exists(directory + @"/" + fileName)))
            { 
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.Create(directory + @"/" + fileName);
            }

            Class1 clsLib = new Class1();

            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            clsLib.DoWorkAsyncInfiniteLoop(directory, cancelationToken, (clbkResult) => result = clbkResult);

            Thread.Sleep(5000);

            //File.WriteAllText(directory + @"/" + fileName, "This is test");
            File.CreateText(directory + @"/" + fileName);

            expectedResult = @"directory: " + directory + ", fileName: " + fileName + ", changed";

            while (result == null)
            {
                Thread.Sleep(1000);
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
                if (!Directory.Exists(directory1))
                    Directory.CreateDirectory(directory1);

                File.Create(directory1 + @"/" + fileName).Close();
            }

            if (!(File.Exists(directory2 + @"/" + fileName)))
            {
                if (!Directory.Exists(directory2))
                    Directory.CreateDirectory(directory2);
                
                File.Create(directory2 + @"/" + fileName).Close();
            }

            Class1 clsLib = new Class1();
            var result = "";
            
            CancellationTokenSource cancelationToken = new CancellationTokenSource();

            clsLib.cancelationTokens.Add(directory1, cancelationToken);

            var tsk1 = clsLib.DoWorkAsyncInfiniteLoop(directory1, cancelationToken, (clbkResult) => result = clbkResult);

            Thread.Sleep(1000);
            
            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);

            CancellationTokenSource cancelationToken2 = new CancellationTokenSource();

            clsLib.cancelationTokens.Add(directory2, cancelationToken2);

            var tsk2 = clsLib.DoWorkAsyncInfiniteLoop(directory2, cancelationToken2, (clbkResult) => result = clbkResult);

            Thread.Sleep(1000);

            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            Assert.AreNotEqual(tsk2.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            
            File.CreateText(directory2 + @"/" + fileName);

            File.CreateText(directory1 + @"/" + fileName);

            clsLib.StopObservation(directory2);

            Thread.Sleep(5000);

            Assert.AreNotEqual(tsk1.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
            Assert.AreEqual(tsk2.Status, System.Threading.Tasks.TaskStatus.RanToCompletion);
        }
    }
}
