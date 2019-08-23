using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace Logy.Maps.Geometry.Tests
{
    [TestFixture]
    public class MatrixesTests
    {
        [Test]
        public void Rotate_OrderHasSense()
        {
            var normal = Basin3.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            Assert.AreNotEqual(Basin3.Oz, normal);

            normal = Basin3.Oz;
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), 10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(1, 0, 0), -10, AngleUnit.Degrees);
            normal = normal.Rotate(new UnitVector3D(0, 1, 0), -10, AngleUnit.Degrees);
            Assert.AreEqual(Basin3.Oz.X, normal.X, .0000001);
        }

        [Test]
        public void Rotate_Matrix()
        {
            Assert.AreEqual(Basin3.Oz, Matrixes.ToCartesian(new Coor()));

            var coor = new HealCoor(-90, 90);
            var normalCalm = Matrixes.ToCartesian(coor);

            // todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-coor.Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(coor.Lambda.Value, AngleUnit.Radians));
            var normalCalmByMatrix = new UnitVector3D(Basin3.OxMinus * rotation);
            Assert.AreEqual(normalCalm.X, normalCalmByMatrix.X, .00000001);
            Assert.AreEqual(normalCalm.Y, normalCalmByMatrix.Y, .00000001);
            Assert.AreEqual(normalCalm.Z, normalCalmByMatrix.Z, .00000001);
        }
    }
}