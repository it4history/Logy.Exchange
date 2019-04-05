using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class Altitudes : Map2DBase
    {
        public Altitudes()
        {
            Frames = 30;
        }

        public override Projection Projection
        {
            get { return Projection.Equirectangular; }
        }

        protected override DataForMap2D ApproximateData
        {
            get { return new WorldAltitudesData(this); }
        }
    }
}