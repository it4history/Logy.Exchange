using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid.Tests
{
    [TestFixture]
    public class GeoidTests
    {
        [Test]
        public void AnyModificationofGetAltitude()
        {
            var empty = new Dictionary<int, BasinOfGeoid>();
            var d = empty.FirstOrDefault(f => f.Value.Polygon.SurfaceType == SurfaceType.Water);
            Assert.AreEqual(d.Key, 0);
            Assert.IsNull(d.Value);
        }
    }
}