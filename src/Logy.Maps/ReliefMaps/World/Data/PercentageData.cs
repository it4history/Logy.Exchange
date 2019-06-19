using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class PercentageData : DataForMap2D<HealCoor>
    {
        private double _percentageOfLand = .50;
        private double _landPixelsCount;

        public PercentageData(Map2DBase<HealCoor> map) : base(map)
        {
        }

        public override double? GetAltitude(HealCoor basin)
        {
            var surface = Relief.GetAltitude(basin);
            if (surface > 0)
                _landPixelsCount++;

            var middle = HealpixManager.Npix / 2d;
            return Math.Abs(basin.P - middle) <= middle * _percentageOfLand ? 1 : 0;
        }

        public override void Log()
        {
            Console.WriteLine("Real land occupies {0:P} of Earth", _landPixelsCount / HealpixManager.Npix);
            base.Log();
        }
    }
}