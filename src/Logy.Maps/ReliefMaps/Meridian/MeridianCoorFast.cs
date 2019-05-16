using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// trying to make faster http://hist.tk/hw/Gradient_and_radius_crosses#lQn
    /// </summary>
    public class MeridianCoorFast : MeridianCoor
    {
        private double _lengthQn2;

        public override void OnInit(HealpixManager man)
        {
            base.OnInit(man);
            var northRingPixel = man.GetCenter(Math.Max(1, P - PixelInRing));
            _lengthQn2 = (Beta.Value - northRingPixel.Beta.Value) * RadiusOfEllipse / 2;
        }

        public override double Intersect(BasinBase otherBasin)
        {
            return Math.Tan(Delta_g_meridian) * _lengthQn2;
        }
    }
}