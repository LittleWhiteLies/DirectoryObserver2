using DirectoryObserver2.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
            
            controller.DoWorkAsyncInfiniteLoop(directory, (clbkResult) => result = clbkResult);

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

            controller.DoWorkAsyncInfiniteLoop(directory, (clbkResult) => result = clbkResult);

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
    }
}
