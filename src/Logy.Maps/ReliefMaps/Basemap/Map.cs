using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class Map 
    {
        public const int BitmapScaleX = 180;
        public const int BitmapScaleY = 90;

        public readonly HealpixManager HealpixManager;

        /// <summary>
        /// set in constructor before Bitmap creation
        /// </summary>
        public readonly LegendType LegendType;

        protected Map(int k, LegendType legendType)
        {
            K = k;
            HealpixManager = new HealpixManager(k);
            LegendType = legendType;
        }

        public virtual bool IsGrey => false;

        protected int K { get; }
    }
}