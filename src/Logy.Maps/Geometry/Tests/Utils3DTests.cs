#if DEBUG
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class Utils3DTests
    {
        [Test]
        public void FromCartesian()
        {
            Assert.AreEqual("-180:90", Utils3D.FromCartesian<Coor>(Basin3.Oz).ToString());
            Assert.AreEqual("-180:-90", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, 0, -1)).ToString());
            Assert.AreEqual("-180:0", Utils3D.FromCartesian<Coor>(Basin3.OxMinus).ToString());
            Assert.AreEqual("0:0", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, 0)).ToString());
            Assert.AreEqual("-90:0", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, 0)).ToString());

            Assert.AreEqual("0:45", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, 1)).ToString());
            Assert.AreEqual("0:-45", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, -1)).ToString());

            Assert.AreEqual("-90:45", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, 1)).ToString());
            Assert.AreEqual("-90:-45", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, -1)).ToString());
        }
    }
}
#endif