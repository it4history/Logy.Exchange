using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Meridian.Data
{
    public class CentrifugalAcceleration : MeridianWater<MeridianCoor>
    {
        public CentrifugalAcceleration(HealpixManager man) : base(man)
        {
            Dimension = "mm/s²";
        }

        public override double? GetAltitude(MeridianCoor basin)
        {
            var aH = EllipsoidAcceleration.Centrifugal(basin);

            //var aV = a * Math.Sin(basin.Theta);

            return aH * 1000;
        }
    }
}