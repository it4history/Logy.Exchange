using System;
using Logy.Maps.Approximations;
using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Tests
{
    [TestFixture]
    public class BasinTests
    {
        [Test]
        public void Symmetric()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin>(0);
            Assert.AreEqual(9, basin.Symmetric(man).P);
            Assert.AreEqual(7, man.GetCenter<Basin>(5).Symmetric(man).P);
            Assert.AreEqual(4, man.GetCenter<Basin>(4).Symmetric(man).P);
            Assert.AreEqual(5, man.GetCenter<Basin>(7).Symmetric(man).P);
            Assert.AreEqual(6, man.GetCenter<Basin>(6).Symmetric(man).P);

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
                Assert.AreEqual(p,
                    man.GetCenter<Basin>(man.GetCenter<Basin>(p).Symmetric(man).P).Symmetric(man).P);
            }
        }

        [Test]
        public void Geometry_Rotate()
        {
            var normal = Basin.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            Assert.AreNotEqual(Basin.Oz, normal);

            normal = Basin.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            Assert.AreEqual(Basin.Oz.X, normal.X, .0000001);
        }

        [Test]
        public void Intersect()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin>(0);
            Assert.AreEqual(0, basin.Intersect(basin));
            Assert.AreEqual(0, basin.Surface.AbsoluteDistanceTo(basin.Q3));
            var toBasin = man.GetCenter<Basin>(1);
            var from0to1 = basin.Surface.AbsoluteDistanceTo(toBasin.Q3);
            var from1to0 = toBasin.Surface.AbsoluteDistanceTo(basin.Q3);
            Assert.AreEqual(from1to0, from0to1, 1);
            basin = man.GetCenter<Basin>(4);
            toBasin = man.GetCenter<Basin>(7);
            Assert.AreEqual(basin.Surface.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3), .0001);

            man = new HealpixManager(1);
            basin = man.GetCenter<Basin>(0);
            toBasin = man.GetCenter<Basin>(4);
            //basin.SetKQQaxis(0, man);
            //toBasin.SetKQQaxis(0, man);
            Assert.AreEqual(basin.Surface.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3), .0001);
        }
    }
}