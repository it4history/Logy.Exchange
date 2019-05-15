using System;
using System.Runtime.Serialization;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinDataBase<T> : WaterMoving<T> where T : Basin3
    {
        public BasinDataBase(
            HealpixManager man, 
            bool withRelief = false, 
            bool spheric = false,
            double? min = null, 
            double? max = null, 
            bool readAllAtStart = false)
            : base(man, null, min, max, readAllAtStart)
        {
            Visual = basin => basin.HeightOQ;
            /// Visual = basin.Visual * 1000;

            ColorsMiddle = 0;

            foreach (var basin in PixMan.Pixels)
            {
                if (withRelief)
                {
                    int waterHeight;
                    var hOQ = GetHeights(basin, (int)basin.RadiusOfEllipse, out waterHeight);
                    basin.HeightOQ = hOQ;
                    if (waterHeight > 0)
                    {
                        basin.Depth = waterHeight - hOQ;
                    }
                    else
                    {
                        basin.Depth = -hOQ;
                    }
                }
                if (spheric)
                {
                    basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
                    if (withRelief)
                    {
                        var diff = Earth2014Manager.Radius2Add - basin.RadiusOfEllipse;
                        basin.Depth += diff;
                        basin.HeightOQ -= diff;
                    }
                    basin.InitROfEllipse(HealpixManager, Earth2014Manager.Radius2Add);
                }

                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = PixMan.Pixels[man.Neibors.Get(to, basin)];
                    basin.Neibors[to] = toBasin;

                    basin.Froms[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);

                    basin.MeanEdges[(int)to] = man.Neibors.MeanBoundary(basin, to);
                }
                /// basin.CorrectionSurface();
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];
                    basin.InitialHto[to] = basin.Metric(toBasin, to, true);
                }
            }

            if (withRelief)
            {
                CheckOcean();
            }
        }

        [IgnoreDataMember]
        public override ReliefType ReliefBedType => ReliefType.Tbi;

        [IgnoreDataMember]
        public Func<T, double> Visual { get; set; }

        public override double? GetAltitude(T basin)
        {
            if (basin.HasWater())
            {
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];
                    var @from = basin.Froms[to];
                    var koef
                        = .25;
                    /// = basin.Koef[(int)to] / basin.Koef.Sum();

                    // todo balance deltaH relative to basin.WaterHeight
                    var height = basin.Hto[to] - toBasin.Hto[@from];

                    var movedFromBasin = Water.PutV(
                        basin, 
                        toBasin,
                        height * koef,
                        to, 
                        @from);
                    if (Math.Abs(movedFromBasin) > 0)
                    {
                    }
                }
            }
            return basin.HasWater() ? Visual(basin) : (double?)null; // basin.hOQ;
        }

        internal override void GradientAndHeightCrosses()
        {
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                if (basin.HasWater())
                {
                    for (int to = 0; to < 4; to++)
                    {
                        var toBasin = basin.Neibors[to];
                        var hto = basin.Metric(toBasin, to);
                        basin.Hto[to] = hto;
                    }
                }
            }
        }
    }
}