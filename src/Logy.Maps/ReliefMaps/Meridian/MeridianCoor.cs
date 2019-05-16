using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Meridian
{
    public class MeridianCoor : MeridianBase
    {
        /// <summary>
        /// key - NeighborVert, relative to otherBasin.Q
        /// </summary>
        public override double[] Hto { get; set; }

        /// <summary>
        /// key - NeighborVert
        /// </summary>
        public override bool[] Volumes { get; set; }

        public override void OnInit(HealpixManager man)
        {
            base.OnInit(man);
            Hto = new double[2];
            Volumes = new bool[2];
        }

        public override void WaterReset()
        {
            Volumes[0] = Volumes[1] = false;
        }
    }
}