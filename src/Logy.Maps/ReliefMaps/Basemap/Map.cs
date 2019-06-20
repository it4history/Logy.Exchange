using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class Map 
    {
        public readonly HealpixManager HealpixManager;

        protected Map(int k)
        {
            K = k;
            HealpixManager = new HealpixManager(k);
        }

        protected int K { get; }
    }
}