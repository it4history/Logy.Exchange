using System;
using System.Runtime.Serialization;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinDataAbstract<T> : WaterMoving<T> where T : Basin3
    {
        public BasinDataAbstract() { }
        public BasinDataAbstract(
            HealpixManager man,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false)
            : base(man, null, min, max, readAllAtStart)
        {
        }

        [IgnoreDataMember]
        public override ReliefType ReliefBedType => ReliefType.Tbi;

        [IgnoreDataMember]
        public Func<T, double> Visual { get; set; } = basin => basin.Hoq; //// basin.Altitude * 1000;

        public override void Init(bool full = true)
        {
            base.Init(full);

            ColorsMiddle = 0;

            if (full)
            {
                foreach (var basin in PixMan.Pixels)
                {
                    if (WithRelief)
                    {
                        int waterHeight;
                        var hOQ = GetHeights(basin, (int)basin.RadiusOfEllipse, out waterHeight);
                        basin.Hoq = hOQ;
                        if (waterHeight > 0)
                        {
                            basin.Depth = waterHeight - hOQ;
                        }
                        else
                        {
                            basin.Depth = -hOQ;
                        }
                    }
                    if (Spheric)
                    {
                        basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
                        if (WithRelief)
                        {
                            var diff = Earth2014Manager.Radius2Add - basin.RadiusOfEllipse;
                            basin.Depth += diff;
                            basin.Hoq -= diff;
                        }
                        basin.InitROfEllipse(HealpixManager);
                    }

                    foreach (Direction to in Enum.GetValues(typeof(Direction)))
                    {
                        var toBasin = PixMan.Pixels[HealpixManager.Neibors.Get(to, basin)];
                        basin.Neibors[to] = toBasin;

                        basin.Froms[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);

                        basin.MeanEdges[(int)to] = HealpixManager.Neibors.MeanBoundary(basin, to);
                    }
                    /// basin.CorrectionSurface();
                    for (int to = 0; to < 4; to++)
                    {
                        var toBasin = basin.Neibors[to];
                        basin.HtoBase[to] = basin.Metric(toBasin, to, true);
                    }
                }

                if (WithRelief)
                {
                    CheckOcean();
                }
            }
        }

        public override void GradientAndHeightCrosses()
        {
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                /// should be commented if (basin.HasWater())

                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];
                    var hto = basin.Metric(toBasin, to);
                    basin.Hto[to] = hto;
                }
            }
        }

        public override double? GetAltitude(T basin)
        {
            if (basin.HasWater())
            {
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];

                    // null for maps that only visualize
                    if (toBasin != null)
                    {
                        var @from = basin.Froms[to];
                        var koef
                            = .25;
                        /// = basin.Koef[(int)to] / basin.Koef.Sum();

                        // todo balance deltaH relative to basin.WaterHeight
                        var height = basin.Hto[to] - toBasin.Hto[@from];

                        Water.PutV(
                            basin,
                            toBasin,
                            height * koef,
                            to,
                            @from);
                    }
                }
                return Visual(basin); // basin.hOQ;
            }
            return null;
        }
    }
}