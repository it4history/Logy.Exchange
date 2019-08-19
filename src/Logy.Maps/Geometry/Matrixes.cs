using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Geometry
{
    public class Matrixes
    {
        public static UnitVector3D Rotate(Coor coor)
        {
            if (coor.Y == 90)
                return Basin3.Oz;

            var normal = Basin3.OxMinus.Rotate(new UnitVector3D(0, 1, 0), new Angle(coor.Phi, AngleUnit.Radians));
            return normal.Rotate(new UnitVector3D(0, 0, 1), new Angle(-coor.Lambda.Value, AngleUnit.Radians));
        }
    }
}