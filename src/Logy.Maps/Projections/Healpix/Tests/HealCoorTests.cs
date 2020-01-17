#if DEBUG
using System;
using Logy.Maps.Exchange.Earth2014.Tests;
using Logy.Maps.Geometry;
using Logy.MwAgent.Sphere;
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

            var oldPole = (Coor)Datum.Greenland17;
            Assert.AreEqual(43, (oldPole.DistanceTo(Earth2014ManagerTests.Sevastopol) / Math.PI) * 180, .5);

            oldPole = new Coor(-52, 66);
            Assert.AreEqual(24, (oldPole.DistanceTo(new Coor()) / Math.PI) * 180);
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

        [Test]
        public void GetKids()
        {
            var man = new HealpixManager(0);
            var kidsMan = new HealpixManager(1);
            Assert.AreEqual(new[] { 0, 4, 5, 12 }, man.GetCenter(0).GetKids(kidsMan));
            Assert.AreEqual(new[] { 1, 6, 7, 14 }, man.GetCenter(1).GetKids(kidsMan));
            Assert.AreEqual(new[] { 2, 8, 9, 16 }, man.GetCenter(2).GetKids(kidsMan));
            Assert.AreEqual(new[] { 3, 10, 11, 18 }, man.GetCenter(3).GetKids(kidsMan));
            Assert.AreEqual(new[] { 13, 21, 22, 29 }, man.GetCenter(4).GetKids(kidsMan));
            Assert.AreEqual(new[] { 15, 23, 24, 31 }, man.GetCenter(5).GetKids(kidsMan));
            Assert.AreEqual(new[] { 17, 25, 26, 33 }, man.GetCenter(6).GetKids(kidsMan));
            Assert.AreEqual(new[] { 28, 36, 37, 44 }, man.GetCenter(8).GetKids(kidsMan));
            Assert.AreEqual(new[] { 34, 42, 43, 47 }, man.GetCenter(11).GetKids(kidsMan));

            man = new HealpixManager(1);
            kidsMan = new HealpixManager(2);
            Assert.AreEqual(new[] { 0, 4, 5, 13 }, man.GetCenter(0).GetKids(kidsMan));
            Assert.AreEqual(new[] { 1, 6, 7, 16 }, man.GetCenter(1).GetKids(kidsMan));
            Assert.AreEqual(new[] { 2, 8, 9, 19 }, man.GetCenter(2).GetKids(kidsMan));
            Assert.AreEqual(new[] { 3, 10, 11, 22 }, man.GetCenter(3).GetKids(kidsMan));
            Assert.AreEqual(new[] { 12, 24, 25, 40 }, man.GetCenter(4).GetKids(kidsMan));
            Assert.AreEqual(new[] { 14, 26, 27, 42 }, man.GetCenter(5).GetKids(kidsMan));
            Assert.AreEqual(new[] { 15, 28, 29, 44 }, man.GetCenter(6).GetKids(kidsMan));
            Assert.AreEqual(new[] { 18, 32, 33, 48 }, man.GetCenter(8).GetKids(kidsMan));

            Assert.AreEqual(new[] { 43, 59, 60, 75 }, man.GetCenter(13).GetKids(kidsMan));
            Assert.AreEqual(new[] { 45, 61, 62, 77 }, man.GetCenter(14).GetKids(kidsMan));
            Assert.AreEqual(new[] { 47, 63, 64, 79 }, man.GetCenter(15).GetKids(kidsMan));
            Assert.AreEqual(new[] { 78, 94, 95, 110 }, man.GetCenter(23).GetKids(kidsMan));
            Assert.AreEqual(new[] { 80, 96, 97, 112 }, man.GetCenter(24).GetKids(kidsMan));
            Assert.AreEqual(new[] { 109, 125, 126, 141 }, man.GetCenter(30).GetKids(kidsMan));
            Assert.AreEqual(new[] { 111, 127, 128, 143 }, man.GetCenter(31).GetKids(kidsMan));

            Assert.AreEqual(new[] { 169, 180, 181, 188 }, man.GetCenter(44).GetKids(kidsMan));

            man = new HealpixManager(5);
            kidsMan = new HealpixManager(6);
            var kids = man.GetCenter(2159).GetKids(kidsMan);
            foreach (var kid in kids)
            {
                Console.WriteLine(kidsMan.GetCenter(kid));
            }
            Assert.AreEqual(new[] { 8415, 8671, 8672, 8927 }, kids);

            kids = man.GetCenter(2288).GetKids(kidsMan);
            Assert.AreEqual(new[] { 8928, 9184, 9185, 9440 }, kids);

            kids = man.GetCenter(2287).GetKids(kidsMan);
            Assert.AreEqual(new[] { 8926, 9182, 9183, 9438 }, kids);
        }

        [Test]
        public void GetKids_all()
        {
            for (var k = 0; k <= 6; k++)
            {
                var man = new HealpixManager(k);
                var kidsMan = new HealpixManager(man.K + 1);
                for (var p = 0; p < man.Npix; p++)
                {
                    var parent = man.GetCenter(p);
                    foreach (var kidP in parent.GetKids(kidsMan))
                    {
                        Assert.Greater(man.ThetaPix, kidsMan.GetCenter(kidP).DistanceTo(parent));
                    }
                }
            }
        }

        public void GetParent()
        {
            var man = new HealpixManager(1);
            var parentMan = new HealpixManager(0);
            Assert.AreEqual(0, man.GetCenter(0).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(1).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(2).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(3).GetParent(parentMan));
            Assert.AreEqual(0, man.GetCenter(4).GetParent(parentMan));
            Assert.AreEqual(0, man.GetCenter(5).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(6).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(7).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(8).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(9).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(10).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(11).GetParent(parentMan));

            Assert.AreEqual(0, man.GetCenter(12).GetParent(parentMan));
            Assert.AreEqual(4, man.GetCenter(13).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(14).GetParent(parentMan));
            Assert.AreEqual(5, man.GetCenter(15).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(16).GetParent(parentMan));
            Assert.AreEqual(6, man.GetCenter(17).GetParent(parentMan));

            man = new HealpixManager(2);
            parentMan = new HealpixManager(1);
            Assert.AreEqual(0, man.GetCenter(0).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(1).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(2).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(3).GetParent(parentMan));
            Assert.AreEqual(0, man.GetCenter(4).GetParent(parentMan));
            Assert.AreEqual(0, man.GetCenter(5).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(6).GetParent(parentMan));
            Assert.AreEqual(1, man.GetCenter(7).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(8).GetParent(parentMan));
            Assert.AreEqual(2, man.GetCenter(9).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(10).GetParent(parentMan));
            Assert.AreEqual(3, man.GetCenter(11).GetParent(parentMan));

            Assert.AreEqual(7, man.GetCenter(17).GetParent(parentMan));
            Assert.AreEqual(7, man.GetCenter(31).GetParent(parentMan));
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