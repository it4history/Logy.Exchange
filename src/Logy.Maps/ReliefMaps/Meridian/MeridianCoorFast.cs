using System;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// trying to make faster http://hist.tk/hw/Gradient_and_radius_crosses#lQn
    /// </summary>
    public class MeridianCoorFast : MeridianCoor
    {
        public double lQn2;

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            var northRingPixel = man.GetCenter(Math.Max(1, P - PixelInRing));
            lQn2 = (Beta.Value - northRingPixel.Beta.Value) * rOfEllipse / 2;
        }

        public override double Intersect(MeridianBase otherBasin)
        {
            return Math.Tan(Delta_g_meridian) * lQn2;
        }
    }
}