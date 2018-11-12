using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Models;
using LevelEditor.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelEditorTest {
    [TestClass]
    public class FileServiceTest {

        [TestMethod]
        public void SaveFile()
        {
            if(File.Exists("test"))File.Delete("test");
            FileService.SaveFile(new {Message = "test"}, "test");
            Assert.IsTrue(File.Exists("test"));
        }
    }
}
