using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class PercentageData : DataForMap2D
    {
        private int _percentageOfLand = 50;
        private double _landPixelsCount;

        public PercentageData(Map2DBase map) : base(map)
        {
        }

        public override double? GetAltitude(HealCoor basin)
        {
            var surface = Relief.GetAltitude(basin);
            if (surface > 0)
                _landPixelsCount++;

            var middle = HealpixManager.Npix / 2d;
            return Math.Abs(basin.P - middle) <= middle * _percentageOfLand / 100d ? 1 : 0;
        }

        public override void Log()
        {
            Console.WriteLine("Real land occupies {0:P} of Earth", _landPixelsCount / HealpixManager.Npix);
            base.Log();
        }
    }
}