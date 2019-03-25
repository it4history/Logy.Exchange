#if DEBUG
using System;
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
    }
}
#endif