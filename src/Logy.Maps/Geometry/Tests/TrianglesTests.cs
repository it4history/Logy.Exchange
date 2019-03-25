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
            var Qnorth = new Point2D(2, 2);
            var Q = new Point2D(4, 0);

            var QnorthGeodes = new Point2D(0, 0);
            var QGeodes = new Point2D(0, 4);

            var M = new Line2D(Qnorth, QnorthGeodes).IntersectWith(new Line2D(Q, QGeodes));

            Assert.AreEqual(2, M.Value.X);
            Assert.AreEqual(2, M.Value.Y);
        }

        [Test]
        public void SinusesTheorem()
        {
            Assert.AreEqual(Math.Sqrt(2), Triangles.SinusesTheorem(Math.PI/4, 2, Math.PI/4), .0000000001);
        }
    }
}
#endif