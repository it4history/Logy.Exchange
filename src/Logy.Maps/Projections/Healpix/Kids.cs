using System.Drawing.Imaging;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Projections.Healpix
{
    public class Kids : Map2DBase<HealCoor>
    {
        public Kids() : base(6, LegendType.None)
        {
            YResolution = 5;
        }

        protected override DataForMap2D<HealCoor> MapData => new KidsData(this);

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;
    }
}