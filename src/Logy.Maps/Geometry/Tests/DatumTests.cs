﻿#if DEBUG
using Logy.Maps.ReliefMaps.Basemap;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class DatumTests
    {
        [Test]
        public void Radius()
        {
            Assert.AreEqual(BasinAbstract.Oz, Datum.Normal.Axis);
            Assert.AreEqual(-180, new Datum().X);
            Assert.AreEqual(90, new Datum().Y);
        }
    }
}
#endif