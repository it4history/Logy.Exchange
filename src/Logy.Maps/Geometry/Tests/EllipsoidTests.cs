#if DEBUG
using System;
using Logy.MwAgent.Sphere;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class EllipsoidTests
    {
        [Test]
        public void Radius()
        {
            Assert.AreEqual(Ellipsoid.BigRadius, Ellipsoid.Radius(0));
            Assert.AreEqual(Ellipsoid.LessRadius, Ellipsoid.Radius(Math.PI / 2));
        }

        [Test]
        public void RadiusPaleo()
        {
            Assert.AreEqual(Ellipsoid.LessRadius, Ellipsoid.RadiusPaleo(new Coor(), Datum.Normal));
            Assert.AreEqual(Ellipsoid.BigRadius, Ellipsoid.RadiusPaleo(new Coor(), new Datum { Y = 0 }));
        }
    }
}
#endif