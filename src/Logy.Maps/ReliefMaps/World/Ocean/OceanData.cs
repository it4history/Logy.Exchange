using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;

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
            : base(man, min, max, readAllAtStart)
        {
        }
   }
}