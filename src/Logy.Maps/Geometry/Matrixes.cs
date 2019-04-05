using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Geometry
{
    public class Matrixes
    {
        public static UnitVector3D RotationVector = new UnitVector3D(-1, 0, 0);

        public static UnitVector3D Rotate(HealCoor coor)
        {
            var normal = RotationVector.Rotate(new UnitVector3D(0, 1, 0), new Angle(coor.Phi, AngleUnit.Radians));
            return normal.Rotate(new UnitVector3D(0, 0, 1), new Angle(-coor.Lambda.Value, AngleUnit.Radians));
        }
    }
}