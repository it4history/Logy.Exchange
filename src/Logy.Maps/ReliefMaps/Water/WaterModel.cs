using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;

namespace Logy.Maps.ReliefMaps.Water
{
    public class WaterModel
    {
        public const double Koef = .25;

        // when Viscosity greater then water is more liquid
        // if > 1 then water is autoliquied
        // if == 0 then water is solid
        // .4 good and side faces are solid, .5 water not becoming quiet, .7 is bad
        protected const double Viscosity = .4;

        protected const double ThreshholdReliability = 2.2;

        public WaterModel(HealpixManager man)
        {
            Threshhold = (ThreshholdReliability + 7.4) * Math.Pow(2, 4 - man.K);
            ThreshholdNotReliable = 7.4 * Math.Pow(2, 4 - man.K);
        }

        /// <summary>
        /// very reliable
        /// 
        /// for k9: 0.05 is bad, .1-.15 strange interference, .23 is NotReliable
        /// .25 is reliable, .3 very reliable
        /// </summary>
        public double Threshhold { get; }
        public double ThreshholdNotReliable { get; }

        public bool IsMeridian { get; set; }

        /// <summary>
        /// for meridian projection
        /// </summary>
        public double Move(MeridianCoor basin, MeridianCoor toBasin, NeighborVert to)
        {
            var from = 1 - (int)to;
            return toBasin == null
                ? 0
                : PutV(
                    basin,
                    toBasin,
                    (basin.Hto[(int)to] - toBasin.Hto[from]) / 2,
                    (int)to,
                    from);
        }

        /// <param name="height">>0 when water is moved from basin</param>
        /// <returns>some volume moved from basin</returns>
        public double PutV(BasinAbstract basin, BasinAbstract toBasin, double height, int to, int from)
        {
            // must be transaction
            if (!toBasin.Volumes[from] && !basin.Volumes[to])
            {
                if (Math.Abs(height) > Threshhold)
                {
                    var k = IsMeridian ? basin.RingArea / toBasin.RingArea : 1;
                    var v = Viscosity * height;
                    var volumeFromBasin = v;
                    double? volumeToBasin = null;
                    if (v > 0)
                    {
                        if (basin.Depth.HasValue)
                        {
                            volumeFromBasin = Math.Min(basin.WaterHeight, v);
                            volumeToBasin = volumeFromBasin * k;
                        }
                    }
                    else
                    {
                        // todo test this
                        if (toBasin.Depth.HasValue)
                        {
                            volumeToBasin = -Math.Min(toBasin.WaterHeight, -v * k);
                            volumeFromBasin = volumeToBasin.Value / k;
                        }
                    }
                    if (!volumeToBasin.HasValue)
                        volumeToBasin = v * k;

                    if (Math.Abs(volumeFromBasin) > 0)
                    {
                        toBasin.WaterIn(volumeToBasin.Value, from);

                        // water out source basin
                        basin.WaterIn(-volumeFromBasin, to);
                        return volumeFromBasin;
                    }
                }
            }
            return 0;
        }
    }
}