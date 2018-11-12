using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelEditorTest {
    [TestClass]
    public class BitmapServiceTest {

        [TestMethod]
        public void TestImageLoading()
        {
            var source = BitmapService.Instance.GetBitmapSource("Images/DefaultTile.png", null);
            Assert.IsNotNull(source);
            Assert.IsTrue(source.PixelHeight == 128 && source.PixelWidth == 128);
        }
    }
}
