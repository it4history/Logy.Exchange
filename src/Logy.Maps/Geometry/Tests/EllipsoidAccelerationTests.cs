#if DEBUG
using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class EllipsoidAccelerationTests
    {
        private Basin basin0;
        private Basin basin7;
        private Basin basin31;
        private Basin basin77;
        private Basin basin95;

        [SetUp]
        public void SetUp()
        {
            var man = new HealpixManager(2);
            basin0 = man.GetCenter<Basin>(0);
            basin7 = man.GetCenter<Basin>(7);
            basin31 = man.GetCenter<Basin>(31);
            basin77 = man.GetCenter<Basin>(77);
            basin95 = man.GetCenter<Basin>(95);
        }

        [TearDown]
        public void TearDown()
        {
            EllipsoidAcceleration.AxisOfRotation = Basin.Oz;
        }

        [Test]
        public void GravitationalSomigliana()
        {
            Assert.AreEqual(EllipsoidAcceleration.GWithAOnEquator, EllipsoidAcceleration.GravitationalSomigliana(0));
            Assert.AreEqual(EllipsoidAcceleration.GWithAOnEquator, EllipsoidAcceleration.GravitationalSomigliana(Math.PI * 2));
            Assert.AreEqual(EllipsoidAcceleration.GOnPoles, EllipsoidAcceleration.GravitationalSomigliana(Math.PI / 2), .00000000000001);
            Assert.AreEqual(EllipsoidAcceleration.GOnPoles, EllipsoidAcceleration.GravitationalSomigliana(-Math.PI / 2), .00000000000001);
            Assert.AreEqual(EllipsoidAcceleration.GOnPoles, EllipsoidAcceleration.GravitationalSomigliana(3 * Math.PI / 2), .00000000000001);
        }

        [Test]
        public void Centrifugal()
        {
            Assert.AreEqual(90, new UnitVector3D(1, 0, 0).AngleTo(new UnitVector3D(0, 1, 0)).Degrees);
            Assert.AreEqual(0, new UnitVector3D(1, 0, 0).DotProduct(new UnitVector3D(0, 1, 0)));
            Assert.AreEqual(1, new UnitVector3D(1, 0, 0).DotProduct(new UnitVector3D(1, 0, 0)));

            double a, aTraverse;
            Assert.AreEqual(0, EllipsoidAcceleration.Centrifugal(basin0), .01);
            Assert.AreEqual(.024, EllipsoidAcceleration.Centrifugal(basin31), .01);
            Assert.AreEqual(0, EllipsoidAcceleration.Centrifugal(basin95, out a, out aTraverse), .000001);
            Assert.AreEqual(.0339, a, .0001);
            Assert.AreEqual(0, aTraverse);

            EllipsoidAcceleration.AxisOfRotation = new UnitVector3D(1, 0, 0);
            Assert.AreEqual(-.003, EllipsoidAcceleration.Centrifugal(basin0, out a, out aTraverse), .001);
            Assert.AreEqual(.034, a, .01);
            Assert.AreEqual(-.003, aTraverse, .001);
            Assert.AreEqual(-.017, EllipsoidAcceleration.Centrifugal(basin31, out a, out aTraverse), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(.005, aTraverse, .001);
            Assert.AreEqual(0, EllipsoidAcceleration.Centrifugal(basin95), .01);
        }

        [Test]
        public void Centrifugal_aMeridian_BugOfCone()
        {
            double a, aTraverse;
            EllipsoidAcceleration.AxisOfRotation = new UnitVector3D(1, 0, 0);
            Assert.AreEqual(-.003, EllipsoidAcceleration.Centrifugal(basin77, out a, out aTraverse), .001);
            Assert.AreEqual(.017, a, .01);
            Assert.AreEqual(.017, aTraverse, .001);

            EllipsoidAcceleration.AxisOfRotation = new UnitVector3D(0, 1, 0);
            Assert.AreEqual(-.003, EllipsoidAcceleration.Centrifugal(basin77, out a, out aTraverse), .001);
            Assert.AreEqual(.017, a, .01);
            Assert.AreEqual(-.017, aTraverse, .001);
            Assert.AreEqual(-.001, EllipsoidAcceleration.Centrifugal(basin31, out a, out aTraverse), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(-.005, aTraverse, .001);
        }

        [Test]
        public void Centrifugal_Y()
        {
            double a, aTraverse;
            EllipsoidAcceleration.AxisOfRotation = new UnitVector3D(0, 1, 0);
            Assert.AreEqual(-.003, EllipsoidAcceleration.Centrifugal(basin0, out a, out aTraverse), .001);
            Assert.AreEqual(.034, a, .01);
            Assert.AreEqual(.003, aTraverse, .001);
            Assert.AreEqual(-.002, EllipsoidAcceleration.Centrifugal(basin7, out a, out aTraverse), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(-.005, aTraverse, .001);
        }
    }
}
#endif