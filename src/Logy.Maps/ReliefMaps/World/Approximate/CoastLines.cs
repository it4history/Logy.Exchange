using System.Drawing.Imaging;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class CoastLines : Map2DBase
    {
        public override Projection Projection
        {
            get { return Projection.Equirectangular; }
        }

        protected override bool IsGrey
        {
            get { return true; }
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Gif; }
        }

        protected override DataForMap2D ApproximateData
        {
            get { return new CoastLinesData(this); }
        }
    }
}