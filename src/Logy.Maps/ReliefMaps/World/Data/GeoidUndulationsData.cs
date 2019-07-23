using System;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class GeoidUndulationsData : DataForMap2D<HealCoor>
    {
        public GeoidUndulationsData(Map2DBase<HealCoor> map) : base(map)
        {
            ColorsMiddle = -22;
        }

        public override ReliefType ReliefType => ReliefType.Bed;

        protected override bool IsReliefBedShape => true;

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