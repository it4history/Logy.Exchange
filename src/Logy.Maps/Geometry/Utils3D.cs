using System;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Geometry
{
    public class Utils3D
    {
        public static Plane Equator { get; } = new Plane(BasinAbstract.Oz, BasinAbstract.O3);

        public static UnitVector3D Cartesian(Coor coor)
        {
            return Matrixes.ToCartesian(coor);

            /* analogy
            return coor.Y == 90
                ? Basin3.Oz
                : Basin3.Oz
                    .ToCartesian(
                        new UnitVector3D(0, 1, 0),
                        new Angle(90 - coor.Y, AngleUnit.Degrees))
                    .ToCartesian(
                        new UnitVector3D(0, 0, 1),
                        new Angle(coor.X, AngleUnit.Degrees)); */
        }

        public static T FromCartesian<T>(UnitVector3D ray) where T : Coor
        {
            var coor = Activator.CreateInstance<T>();
            if (ray == -BasinAbstract.Oz)
            {
                coor.Y = -90;
            }
            else if (ray != BasinAbstract.Oz)
            {
                coor.X = -180 - ray.ProjectOn(Equator).Direction.SignedAngleTo(BasinAbstract.OxMinus, BasinAbstract.Oz).Degrees;
                coor.Y = 90 - ray.AngleTo(BasinAbstract.Oz).Degrees;
            }
            return coor.Normalize<T>();
        }
    }
}