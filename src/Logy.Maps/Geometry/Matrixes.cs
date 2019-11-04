using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Geometry
{
    public class Matrixes
    {
        public static UnitVector3D ToCartesian(Coor coor)
        {
            if (coor.Y == 90)
                return BasinAbstract.Oz;

            var normal = BasinAbstract.OxMinus.Rotate(new UnitVector3D(0, 1, 0), new Angle(coor.Phi, AngleUnit.Radians));
            return normal.Rotate(new UnitVector3D(0, 0, 1), new Angle(-coor.Lambda.Value, AngleUnit.Radians));
        }
    }
}