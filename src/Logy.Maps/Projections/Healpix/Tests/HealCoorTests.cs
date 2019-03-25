using System;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Tests
{
    [TestFixture]
    public class HealCoorTests
    {
        [Test]
        public void DistanceTo()
        {
            var coor = new HealCoor { X = 0, Y = 0 };
            Assert.AreEqual(0, coor.DistanceTo(new HealCoor { X = 0, Y = 0 }));
            Assert.AreEqual(Math.PI / 2, coor.DistanceTo(new HealCoor { X = 90, Y = 0 }));
        }
    }
}