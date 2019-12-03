#if DEBUG
using System;
using Logy.Maps.Exchange.Earth2014.Tests;
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

            var oldPole = new HealCoor { X = -40, Y = 73 };
            Assert.AreEqual(43, (oldPole.DistanceTo(Earth2014ManagerTests.Sevastopol) / Math.PI) * 180, .5);
        }

        [Test]
        public void Symmetric()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<HealCoor>(0);
            Assert.AreEqual(9, basin.Symmetric(man).P);
            Assert.AreEqual(7, man.GetCenter<HealCoor>(5).Symmetric(man).P);
            Assert.AreEqual(4, man.GetCenter<HealCoor>(4).Symmetric(man).P);
            Assert.AreEqual(5, man.GetCenter<HealCoor>(7).Symmetric(man).P);
            Assert.AreEqual(6, man.GetCenter<HealCoor>(6).Symmetric(man).P);

            TestAll(man);

            TestAll(new HealpixManager(1));
            TestAll(new HealpixManager(2));
            TestAll(new HealpixManager(3));
            TestAll(new HealpixManager(4));
        }

        private static void TestAll(HealpixManager man)
        {
            for (int p = 0; p < man.Npix; p++)
            {
                Assert.AreEqual(
                    p,
                    man.GetCenter<HealCoor>(man.GetCenter<HealCoor>(p).Symmetric(man).P).Symmetric(man).P);
            }
        }
    }
}

#endif