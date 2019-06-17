#if DEBUG
using System;
using Logy.MwAgent.Sphere;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class EllipsoidTests
    {
        [TearDown]
        public void TearDown()
        {
            Ellipsoid.CurrentDatum = Datum.Normal;
        }

        [Test]
        public void Radius()
        {
            Assert.AreEqual(Ellipsoid.BigRadius, Ellipsoid.Radius(0));
            Assert.AreEqual(Ellipsoid.LessRadius, Ellipsoid.Radius(Math.PI / 2));
        }

        [Test]
        public void RadiusPaleo()
        {
            Assert.AreEqual(Ellipsoid.LessRadius, Ellipsoid.RadiusPaleo(new Coor()));
            Ellipsoid.CurrentDatum = new Datum { Y = 0 };
            Assert.AreEqual(Ellipsoid.BigRadius, Ellipsoid.RadiusPaleo(new Coor()));
        }
    }
}
#endif