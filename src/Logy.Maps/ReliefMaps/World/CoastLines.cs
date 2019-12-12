using System.Drawing.Imaging;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World
{
    public class CoastLines : Map2DBase<HealCoor>
    {
        public override Projection Projection => Projection.Equirectangular;

        public override bool IsGrey => true;

        protected override ImageFormat ImageFormat => ImageFormat.Gif;

        protected override DataForMap2D<HealCoor> MapData => new CoastLinesData(this);
    }
}