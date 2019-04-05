#if DEBUG

using System;
using NUnit.Framework;

namespace Logy.MwAgent.Sphere.Tests
{
    [TestFixture]
    public class CoorTests
    {
        [Test]
        public void Constructor()
        {
            var coor = new Coor("70.1:52");
            Assert.AreEqual(70.1d, coor.X);
            Assert.AreEqual(52, coor.Y);

             coor = new Coor(new Point2(-90, 90));
            Assert.AreEqual(Math.PI * 1.5, coor.Lambda);
            Assert.AreEqual(0, coor.Beta);
        }

        [Test]
        public void BetaLambda()
        {
            var coor = (Coor)new Coor("-180:-90").Normalize();
            Assert.AreEqual(-180d, coor.X);
            Assert.AreEqual(-90d, coor.Y);
            Assert.AreEqual(2 * Math.PI, coor.Lambda);
            Assert.AreEqual(Math.PI, coor.Beta);

            coor = (Coor)new Coor("180:90").Normalize();
            Assert.AreEqual(180d, coor.X);
            Assert.AreEqual(90d, coor.Y);
            Assert.AreEqual(0, coor.Lambda);
            Assert.AreEqual(0, coor.Beta);
        }
    }
}

#endif
