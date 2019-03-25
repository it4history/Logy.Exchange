#if DEBUG
using Logy.Maps.Healpix;
using NUnit.Framework;

namespace Logy.Maps.Coloring.Tests
{
    [TestFixture]
    public class ColorsManagerTests
    {
        [Test]
        public void Get()
        {
            var man = new ColorsManager(-100, 100);
            Assert.AreEqual(ColorsManager.Red, man.Get(1000));

            Assert.AreEqual(ColorsManager.DarkBlue, man.Get(-100));
            Assert.AreEqual(new Color3(0, 238.1, 254.15), man.Get(-1));
            Assert.AreEqual(ColorsManager.Blue, man.Get(0));
            Assert.AreEqual(ColorsManager.Red, man.Get(100));

            man = new ColorsManager(0, 100, 50);
            Assert.AreEqual(ColorsManager.Blue, man.Get(50));
            Assert.AreEqual(new Color3(0, 145, 212.5), man.Get(25));
            Assert.AreEqual(ColorsManager.DarkBlue, man.Get(0));
            Assert.AreEqual(ColorsManager.Red, man.Get(100));

            man = new ColorsManager(-200, 100, 50);
            Assert.AreEqual(ColorsManager.Blue, man.Get(50));
            Assert.AreEqual(ColorsManager.DarkBlue, man.Get(-200));
            Assert.AreEqual(ColorsManager.Red, man.Get(100));
        }
    }
}
#endif
