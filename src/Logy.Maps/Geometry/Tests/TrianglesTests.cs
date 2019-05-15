#if DEBUG
using System;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class TrianglesTests
    {
        [Test]
        public void Intersect()
        {
            var pointQnorth = new Point2D(2, 2);
            var pointQ = new Point2D(4, 0);

            var pointQnorthGeodes = new Point2D(0, 0);
            var pointQGeodes = new Point2D(0, 4);

            var m = new Line2D(pointQnorth, pointQnorthGeodes).IntersectWith(new Line2D(pointQ, pointQGeodes));

            Assert.AreEqual(2, m.Value.X);
            Assert.AreEqual(2, m.Value.Y);
        }

        [Test]
        public void SinusesTheorem()
        {
            Assert.AreEqual(Math.Sqrt(2), Triangles.SinusesTheorem(Math.PI / 4, 2, Math.PI / 4), .0000000001);
        }
    }
}
#endif