using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class Altitudes : Map2DBase
    {
        public Altitudes()
        {
            Frames = 1;
        }

        public override Projection Projection => Projection.Equirectangular;

        protected override int K => 8;

        protected override DataForMap2D ApproximateData => new WorldAltitudesData(this);
    }
}