using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class CoastLinesData : DataForMap2D
    {
        public CoastLinesData(Map2DBase map) : base(map)
        {
        }

        public override double? GetAltitude(HealCoor basin)
        {
            var surface = Relief.GetAltitude(basin);
            if (surface > 0)
            {
                // lakes in ice are ignored
                return 1;
            }
            return 0;
        }
    }
}