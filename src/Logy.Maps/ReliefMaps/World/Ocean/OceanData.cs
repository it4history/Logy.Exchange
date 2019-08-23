using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Geoid;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class OceanData : BasinDataAbstract<Basin3>
    {
        public OceanData() { }
        public OceanData(
            HealpixManager man, 
            double? min = null, 
            double? max = null, 
            bool readAllAtStart = false)
            : base(man, null, min, max, readAllAtStart)
        {
        }

        public OceanData(HealpixManager man, Rectangle<Basin3> rectangle)
            : base(man, rectangle.Subset(man))
        {
        }
    }
}