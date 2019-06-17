using System;
using Logy.Maps.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class GeoidUndulationsData : EllipseWithShapeData
    {
        public GeoidUndulationsData(Map2DBase map) : base(map)
        {
            ColorsMiddle = -22;
        }

        public override int Accuracy => 1;

        public override double? GetAltitude(HealCoor basin)
        {
            var altitude = Relief.GetAltitude(basin);
            var altitudeShape = ReliefBed.GetAltitude(basin) + Earth2014Manager.Radius2Add;

            var thetaTan = Math.Tan(basin.Beta.Value); /* may be out of range */
                var undulation = altitudeShape - altitude
                                 - Ellipsoid.Radius(Ellipsoid.CalcVarPhi(thetaTan));
            return undulation;
        }

        public override void Log()
        {
            Assert.LessOrEqual(Colors.Max, 90);
            Assert.GreaterOrEqual(Colors.Min, -110);
            base.Log();
        }
    }
}