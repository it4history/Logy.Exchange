using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Meridian.Data
{
    public class Gravity : CentrifugalAcceleration
    {
        public Gravity(HealpixManager man) : base(man)
        {
        }

        public override double? GetAltitude(MeridianCoor basin)
        {
            return basin.GHpure * 1000;
        }
    }
}