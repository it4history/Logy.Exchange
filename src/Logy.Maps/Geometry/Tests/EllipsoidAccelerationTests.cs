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
        private Basin3 _basin0;
        private Basin3 _basin7;
        private Basin3 _basin31;
        private Basin3 _basin77;
        private Basin3 _basin95;

        [SetUp]
        public void SetUp()
        {
            var man = new HealpixManager(2);
            _basin0 = man.GetCenter<Basin3>(0);
            _basin7 = man.GetCenter<Basin3>(7);
            _basin31 = man.GetCenter<Basin3>(31);
            _basin77 = man.GetCenter<Basin3>(77);
            _basin95 = man.GetCenter<Basin3>(95);
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

            var datum = Datum.Normal;
            double a, aTraverse, aVertical;
            Assert.AreEqual(0, datum.Centrifugal(_basin0), .01);
            Assert.AreEqual(.024, datum.Centrifugal(_basin31), .01);
            Assert.AreEqual(0, datum.Centrifugal(_basin95, out a, out aTraverse, out aVertical), .000001);
            Assert.AreEqual(.0339, a, .0001);
            Assert.AreEqual(0, aTraverse);
            Assert.AreEqual(.0339, aVertical, .0001);

            datum = new Datum { X = 0, Y = 0 }; /// Axis = new UnitVector3D(1, 0, 0);
            Assert.AreEqual(-.003, datum.Centrifugal(_basin0, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.034, a, .01);
            Assert.AreEqual(-.003, aTraverse, .001);
            Assert.AreEqual(.0331, aVertical, .0001);
            Assert.AreEqual(-.017, datum.Centrifugal(_basin31, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(.005, aTraverse, .001);
            Assert.AreEqual(.0157, aVertical, .0001);
            Assert.AreEqual(0, datum.Centrifugal(_basin95), .01);
        }

        [Test]
        public void Centrifugal_aMeridian_BugOfCone()
        {
            double a, aTraverse, aVertical;
            var datum = new Datum { X = 0, Y = 0 }; /// Axis = new UnitVector3D(1, 0, 0);
            Assert.AreEqual(-.003, datum.Centrifugal(_basin77, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.017, a, .01);
            Assert.AreEqual(.017, aTraverse, .001);
            Assert.AreEqual(.0174, aVertical, .0001);

            datum = new Datum { X = 90, Y = 0 }; /// Axis = new UnitVector3D(0, 1, 0);
            Assert.AreEqual(-.003, datum.Centrifugal(_basin77, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.017, a, .01);
            Assert.AreEqual(-.017, aTraverse, .001);
            Assert.AreEqual(.0174, aVertical, .0001);
            Assert.AreEqual(-.001, datum.Centrifugal(_basin31, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(-.005, aTraverse, .001);
            Assert.AreEqual(.0331, aVertical, .0001);
        }

        [Test]
        public void Centrifugal_Y()
        {
            double a, aTraverse, aVertical;
            var datum = new Datum { X = 90, Y = 0 }; /// Axis = new UnitVector3D(0, 1, 0);
            Assert.AreEqual(-.003, datum.Centrifugal(_basin0, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.034, a, .01);
            Assert.AreEqual(.003, aTraverse, .001);
            Assert.AreEqual(.0331, aVertical, .0001);
            Assert.AreEqual(-.002, datum.Centrifugal(_basin7, out a, out aTraverse, out aVertical), .001);
            Assert.AreEqual(.024, a, .01);
            Assert.AreEqual(-.005, aTraverse, .001);
            Assert.AreEqual(.0331, aVertical, .0001);
        }
    }
}
#endif