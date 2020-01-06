using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World
{
    public class Ararat : WorldAltitudes
    {
        public Ararat() : base(5)
        {
            YResolution = 4;
        }

        public override Projection Projection => Projection.Healpix2Equirectangular;

        protected override DataForMap2D<HealCoor> MapData => new WorldAltitudesData(this)
        {
            AraratP = 2159 /* 2159 40.2281847280611,45  
            //2288 38.6821874534894,43.59375 //2287 38.6821874534894,46.40625} //2023 41.8103148957786,43.59375 //2160 40.2281847280611,42.1875 */
        };
    }
}