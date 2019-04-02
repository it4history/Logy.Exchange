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
        public void Geometry_Rotate_OrderHasSense()
        {
            var normal = Basin3D.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            Assert.AreNotEqual(Basin3D.Oz, normal);

            normal = Basin3D.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            Assert.AreEqual(Basin3D.Oz.X, normal.X, .0000001);
        }

        [Test]
        public void Geometry_Rotate_Matrix()
        {
            var phi = 10;
            var lambda = 90;

            var normal = new UnitVector3D(-1, 0, 0);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), new Angle(phi, AngleUnit.Degrees));
            var normalCalm = normal.Rotate(new UnitVector3D(0, 0, 1), new Angle(-lambda, AngleUnit.Degrees));

            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-phi, AngleUnit.Degrees))
                           * Matrix3D.RotationAroundZAxis(new Angle(lambda, AngleUnit.Degrees));
            var normalCalmByMatrix = new UnitVector3D(new UnitVector3D(-1, 0, 0) * rotation);
            Assert.AreEqual(normalCalm.X, normalCalmByMatrix.X, .00000001);
            Assert.AreEqual(normalCalm.Y, normalCalmByMatrix.Y, .00000001);
            Assert.AreEqual(normalCalm.Z, normalCalmByMatrix.Z, .00000001);
        }

        [Test]
        public void Intersect()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin3D>(0);
            Assert.AreEqual(0, basin.Intersect(basin));
            Assert.AreEqual(0, basin.Surface.AbsoluteDistanceTo(basin.Q3D));
            var toBasin = man.GetCenter<Basin3D>(1);
            var from0to1 = basin.Surface.AbsoluteDistanceTo(toBasin.Q3D);
            var from1to0 = toBasin.Surface.AbsoluteDistanceTo(basin.Q3D);
            Assert.AreEqual(from1to0, from0to1, 1);
            basin = man.GetCenter<Basin3D>(4);
            toBasin = man.GetCenter<Basin3D>(7);
            Assert.AreEqual(
                basin.Surface.AbsoluteDistanceTo(toBasin.Q3D),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3D), 
                .0001);

            man = new HealpixManager(1);
            basin = man.GetCenter<Basin3D>(0);
            toBasin = man.GetCenter<Basin3D>(4);

            // basin.SetKQQaxis(0, man);
            // toBasin.SetKQQaxis(0, man);
            Assert.AreEqual(
                basin.Surface.AbsoluteDistanceTo(toBasin.Q3D),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3D), 
                .0001);
        }

        [Test]
        public void Symmetric()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin3D>(0);
            Assert.AreEqual(9, basin.Symmetric(man).P);
            Assert.AreEqual(7, man.GetCenter<Basin3D>(5).Symmetric(man).P);
            Assert.AreEqual(4, man.GetCenter<Basin3D>(4).Symmetric(man).P);
            Assert.AreEqual(5, man.GetCenter<Basin3D>(7).Symmetric(man).P);
            Assert.AreEqual(6, man.GetCenter<Basin3D>(6).Symmetric(man).P);

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
                    man.GetCenter<Basin3D>(man.GetCenter<Basin3D>(p).Symmetric(man).P).Symmetric(man).P);
            }
        }
    }
}