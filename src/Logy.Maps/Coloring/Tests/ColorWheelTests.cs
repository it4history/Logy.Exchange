#if DEBUG
using System.Diagnostics;
using System.Drawing;
using NUnit.Framework;

namespace Logy.Maps.Coloring.Tests
{
    [TestFixture]
    public class ColorWheelTests
    {
        [Test]
        public void Draw()
        {
            var w = 100;
            var bmp = new Bitmap(w, w);
            ColorWheel.Draw(bmp, w);
            var filename = "ColorWheelTests.jpg";
            bmp.Save(filename);
            Process.Start(filename);
        }

        [Test]
        public void SetAngle()
        {
            Assert.AreEqual(.00027, ColorWheel.SetAngle(0, 0), ColorWheel.ModuleAccuracy);

            Assert.AreEqual(0, ColorWheel.SetAngle(1, 0));
            Assert.AreEqual(90, ColorWheel.SetAngle(0, 1) * 1000000, .0001);
            Assert.AreEqual(180, ColorWheel.SetAngle(-1, 0) * 1000000, .0001);
            Assert.AreEqual(270, ColorWheel.SetAngle(0, -1) * 1000000, .0001);

            Assert.AreEqual(45, ColorWheel.SetAngle(1, 1) * 1000000, .0001);
            Assert.AreEqual(135, ColorWheel.SetAngle(-1, 1) * 1000000, .0001);
            Assert.AreEqual(225, ColorWheel.SetAngle(-1, -1) * 1000000, .0001);
            Assert.AreEqual(315, ColorWheel.SetAngle(1, -1) * 1000000, .0001);

            Assert.AreEqual(32, ColorWheel.GetAngle(ColorWheel.SetAngle(1, 1)), .2);

            Assert.AreEqual(32, ColorWheel.GetAngle(ColorWheel.SetAngle(1, 1, 2.01)), .2);
        }
    }
}
#endif
