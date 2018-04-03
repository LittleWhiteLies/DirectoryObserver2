using DirectoryObserver;
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
            int frequency = 1000;
            string directory = @"C:/temp4";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedResult = null;

            if ((File.Exists(directory + @"/" + fileName)))
            {
                Directory.Delete(directory, true);
            }
            
            Directory.CreateDirectory(directory);

            //ObserverClass clsLib = new ObserverClass();
            ObserverClass2 clsLib = new ObserverClass2();

            //clsLib.Observe(directory, frequency, (clbkResult) => result = clbkResult);
            clsLib.StartObserveDirectory(directory, frequency, (clbkResult) => result = clbkResult);

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
            int frequency = 1000;
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

            //ObserverClass clsLib = new ObserverClass();
            ObserverClass2 clsLib = new ObserverClass2();

            clsLib.StartObserveDirectory(directory, frequency, (clbkResult) => result = clbkResult);
            
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
            int frequency = 1000;
            string directory1 = @"C:/temp";
            string directory2 = @"C:/temp2";
            string fileName = @"unitTestFile.txt";
            string result = null;
            string expectedDir1Result = @"directory: " + directory1 + ", fileName: " + fileName + ", changed"; ;
            string expectedDir2Result = @"directory: " + directory2 + ", fileName: " + fileName + ", changed"; ;

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

            ObserverClass2 clsLib = new ObserverClass2();
            
            clsLib.StartObserveDirectory(directory1, frequency, (clbkResult) => result = clbkResult);
            
            File.CreateText(directory1 + @"/" + fileName).Close();
            
            while (result == null)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(result, expectedDir1Result);

            result = null;
            
            clsLib.StartObserveDirectory(directory2, frequency, (clbkResult) => result = clbkResult);
            
            File.CreateText(directory2 + @"/" + fileName).Close();
            
            while (result == null)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(result, expectedDir2Result);

            result = null;
            
            clsLib.StopObserveDirectory(directory2);

            File.CreateText(directory2 + @"/" + fileName).Close();
            
            File.CreateText(directory1 + @"/" + fileName).Close();

            while (result == null)
            {
                Thread.Sleep(1000);
            }

            Assert.AreEqual(result, expectedDir1Result);
            Assert.AreNotEqual(result, expectedDir2Result);

            clsLib.StopObserveDirectory(directory1);
        }
    }
}
