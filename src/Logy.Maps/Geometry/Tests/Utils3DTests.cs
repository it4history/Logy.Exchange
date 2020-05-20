#if DEBUG
using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class Utils3DTests
    {
        [Test]
        public void FromCartesian()
        {
            Assert.AreEqual("-180:90", Utils3D.FromCartesian<Coor>(BasinAbstract.Oz).ToString());
            Assert.AreEqual("-180:-90", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, 0, -1)).ToString());
            Assert.AreEqual("-180:0", Utils3D.FromCartesian<Coor>(BasinAbstract.OxMinus).ToString());
            Assert.AreEqual("0:0", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, 0)).ToString());
            Assert.AreEqual("-90:0", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, 0)).ToString());

            Assert.AreEqual("0:45", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, 1)).ToString());
            Assert.AreEqual("0:-45", Utils3D.FromCartesian<Coor>(new UnitVector3D(1, 0, -1)).ToString());

            Assert.AreEqual("-90:45", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, 1)).ToString());
            Assert.AreEqual("-90:-45", Utils3D.FromCartesian<Coor>(new UnitVector3D(0, -1, -1)).ToString());
        }

        [Test]
        public void Rotate_OrderHasSense()
        {
            var normal = BasinAbstract.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            Assert.AreNotEqual(BasinAbstract.Oz, normal);

            normal = BasinAbstract.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            Assert.AreEqual(BasinAbstract.Oz.X, normal.X, .0000001);
        }

        [Test]
        public void Rotate_Matrix()
        {
            Assert.AreEqual(BasinAbstract.Oz, Utils3D.Cartesian(new Coor()));

            var coor = new HealCoor(-90, 90);
            var normalCalm = Utils3D.Cartesian(coor);

            // todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-coor.Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(coor.Lambda.Value, AngleUnit.Radians));
            var normalCalmByMatrix = new UnitVector3D(BasinAbstract.OxMinus * rotation);
            Assert.AreEqual(normalCalm.X, normalCalmByMatrix.X, .00000001);
            Assert.AreEqual(normalCalm.Y, normalCalmByMatrix.Y, .00000001);
            Assert.AreEqual(normalCalm.Z, normalCalmByMatrix.Z, .00000001);
        }

        [Test]
        public void RotateBySphericCoor()
        {
            Assert.AreEqual(new UnitVector3D(-1, 0, 0), Utils3D.RotateBySphericCoor(0, 0));

            var delta = .1;
            // issue was 
            Assert.AreNotEqual(
                Utils3D.Cartesian(new Coor {Beta = delta}),
                Utils3D.Cartesian(new Coor())
                    .Rotate(new UnitVector3D(0, 1, 0), new Angle(delta, AngleUnit.Radians)));
        }
    }
}
#endif