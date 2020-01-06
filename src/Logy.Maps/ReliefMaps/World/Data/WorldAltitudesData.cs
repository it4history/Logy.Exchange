using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Data
{
    public class WorldAltitudesData : DataForMap2D<HealCoor>
    {
        public WorldAltitudesData(Map2DBase<HealCoor> map) : base(map)
        {
            ColorsMiddle = 0;
        }

        public int? AraratP { get; set; }

        public override double? GetAltitude(HealCoor basin)
        {
            var altitude = Relief.GetAltitude(basin);
            if (altitude == 0)
            {
                altitude = ReliefBed.GetAltitude(basin);
            }

            if (AraratP.HasValue && basin.P == AraratP)
                return 10000;
            return altitude;
        }
    }
}