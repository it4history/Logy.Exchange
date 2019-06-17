using System.Drawing.Imaging;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class CoastLines : Map2DBase
    {
        public override Projection Projection => Projection.Equirectangular;

        protected override bool IsGrey => true;

        protected override ImageFormat ImageFormat => ImageFormat.Gif;

        protected override DataForMap2D MapData => new CoastLinesData(this);
    }
}