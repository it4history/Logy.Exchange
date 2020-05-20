using System;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.Geometry
{
    public class Utils3D
    {
        public static Plane Equator { get; } = new Plane(BasinAbstract.Oz, BasinAbstract.O3);

        public static UnitVector3D Cartesian(Coor coor)
        {
            if (coor.Y == 90)
                return BasinAbstract.Oz;

            return RotateBySphericCoor(coor.Phi, -coor.Lambda.Value);

            /* analogy:
            new Point3D(
            LambdaMinusPi2Sin* qb.X,
            LambdaSin* qb.X,
            qb.Y); */

            /* analogy:
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

        public static UnitVector3D RotateBySphericCoor(double zenith, double azimuth)
        {
            var normal = BasinAbstract.OxMinus;

            normal = normal.Rotate(
                new UnitVector3D(0, 1, 0),
                new Angle(zenith, AngleUnit.Radians));
            return normal.Rotate(
                new UnitVector3D(0, 0, 1),
                new Angle(azimuth, AngleUnit.Radians));
        }
    }
}