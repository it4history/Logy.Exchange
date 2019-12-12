using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class Map 
    {
        public readonly HealpixManager HealpixManager;

        protected Map(int k, LegendType legendType)
        {
            K = k;
            HealpixManager = new HealpixManager(k);
            LegendType = legendType;
        }

        public virtual bool IsGrey => false;

        /// <summary>
        /// set in constructor before Bitmap creation
        /// </summary>
        public readonly LegendType LegendType;

        protected int K { get; }
    }
}