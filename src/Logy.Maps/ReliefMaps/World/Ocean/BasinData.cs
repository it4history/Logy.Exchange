using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinData : BasinDataAbstract<Basin3>
    {
        public BasinData() { }
        public BasinData(
            HealpixManager man, 
            double? min = null, 
            double? max = null, 
            bool readAllAtStart = false)
            : base(man, min, max, readAllAtStart)
        {
        }
   }
}