#if DEBUG
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class MathNetPoint3DTests
    {
        [Test]
        public void GetMeanAltitude()
        {
            Assert.AreEqual(new Point3D(0, 0, 2), new Point3D(0, 0, 1) + new Vector3D(0, 0, 1));
        }
    }
}
#endif
