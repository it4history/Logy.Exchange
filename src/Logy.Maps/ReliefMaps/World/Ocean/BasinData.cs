using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinData : BasinDataBase<Basin3>
    {
        public BasinData(HealpixManager man, bool withRelief = false, bool spheric = false,
            double? min = null, double? max = null, bool readAllAtStart = false)
            : base(man, withRelief, spheric, min, max, readAllAtStart)
        {
        }
    }
}