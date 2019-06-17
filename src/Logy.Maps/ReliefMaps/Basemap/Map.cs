using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class Map 
    {
        public readonly HealpixManager HealpixManager;

        protected Map()
        {
            HealpixManager = new HealpixManager(K);
        }

        protected virtual int K => 6;
    }
}