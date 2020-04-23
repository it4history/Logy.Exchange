using System;
using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;

namespace Logy.Maps.ReliefMaps.Water
{
    public class WaterModel
    {
        public const double Koef = .25;
        public const double FluidityStable = .4; // for MetricType.RadiusIntersection

        protected const double ThreshholdReliability = 2.2;

        /// <summary>
        /// for deserialization
        /// </summary>
        public WaterModel()
        {
        }

        public WaterModel(HealpixManager man)
        {
            var pow = Math.Pow(2, 4 - man.K);
            Threshhold = (ThreshholdReliability + 7.4) * pow;
            ThreshholdNotReliable = 7.4 * pow;
        }

        //todo seems that in Fluidity range depends on metric: in RadiusIntersection max is 0.5 and in Middle max is 1.0
        //
        // when Fluidity greater then water is more liquid
        // if > 1 then water is autoliquied
        // if == 0 then water is solid
        // .4 good and side faces are stable, .5 water not becoming quiet, .7 is bad
        public double Fluidity { get; set; } = FluidityStable;

        /// <summary>
        /// very reliable
        /// 
        /// for k9: 0.05 is bad, .1-.15 strange interference, .23 is NotReliable
        /// .25 is reliable, .3 very reliable
        /// </summary>
        public double Threshhold { get; set; }
        [IgnoreDataMember]
        public double ThreshholdNotReliable { get; }

        [IgnoreDataMember]
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
        /// <returns>volume moved from basin</returns>
        public double PutV(BasinAbstract basin, BasinAbstract toBasin, double height, int to, int from)
        {
            // must be transaction
            if (!toBasin.Volumes[from] && !basin.Volumes[to])
            {
                if (Math.Abs(height) > Threshhold)
                {
                    var k = IsMeridian ? basin.RingArea / toBasin.RingArea : 1;
                    var v = Fluidity * height;
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

                        //todo water out and water in must be same
                        return volumeFromBasin;
                    }
                }
            }
            return 0;
        }
    }
}