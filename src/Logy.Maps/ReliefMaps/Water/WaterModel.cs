using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;

namespace Logy.Maps.ReliefMaps.Water
{
    public class WaterModel
    {
        // when Viscosity greater then water is more liquid
        // if > 1 then water is autoliquied
        // if == 0 then water is solid
        // .4 good and side faces are solid, .5 water not becoming quiet, .7 is bad
        protected const double Viscosity = .4;

        public WaterModel(HealpixManager man)
        {
            Threshhold = (ThreshholdReliability + 7.4) * Math.Pow(2, 4 - man.K);
            ThreshholdNotReliable = 7.4 * Math.Pow(2, 4 - man.K);
        }

        protected const double ThreshholdReliability = 2.2;
        /// <summary>
        /// very reliable
        /// 
        /// for k9: 0.05 is bad, .1-.15 strange interference, .23 is NotReliable
        /// .25 is reliable, .3 very reliable
        /// </summary>
        public double Threshhold { get; private set; }
        public double ThreshholdNotReliable { get; private set; }

        public bool IsMeridian { get; set; }

        /// <summary>
        /// for meridian projection
        /// </summary>
        internal double Move(MeridianCoor basin, MeridianCoor toBasin, NeighborVert to)
        {
            var from = 1 - (int)to;
            return toBasin == null
                ? 0
                : PutV(basin, toBasin,
                    (basin.Hto[(int) to] - toBasin.Hto[from]) / 2,
                    (int) to,
                    from);
        }

        /// <param name="height">>0 when water is moved from basin</param>
        /// <returns>some volume moved from basin</returns>
        public double PutV(BasinBase basin, BasinBase toBasin, double height, int to, int from)
        {
            if (Math.Abs(height) > Threshhold)
            {
                // must be transaction
                if (!toBasin.Volumes[from] && !basin.Volumes[to])
                {
                    var k = IsMeridian ? basin.RingArea / toBasin.RingArea : 1;
                    var v = Viscosity * height;
                    var vFromBasin = v;
                    double? vToBasin = null;
                    if (v > 0)
                    {
                        if (basin.Depth.HasValue)
                        {
                            vFromBasin = Math.Min(basin.WaterHeight, v);
                            vToBasin = vFromBasin * k;
                        }
                    }
                    else
                    {
                        // todo test this
                        if (toBasin.Depth.HasValue)
                        {
                            vToBasin = -Math.Min(toBasin.WaterHeight, -v * k);
                            vFromBasin = vToBasin.Value / k;
                        }
                    }
                    if (!vToBasin.HasValue)
                        vToBasin = v * k;

                    if (Math.Abs(vFromBasin) > 0)
                    {
                        toBasin.WaterIn(vToBasin.Value, from);

                        // water out source basin
                        basin.WaterIn(-vFromBasin, to);
                        return vFromBasin;
                    }
                }
            }
            return 0;
        }
    }
}