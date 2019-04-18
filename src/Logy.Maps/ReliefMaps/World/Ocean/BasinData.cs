using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinData : BasinDataBase<Basin3>
    {
        public BasinData(HealpixManager man, bool withRelief = false, bool spheric = false,
            double? min = null, double? max = null)
            : base(man, withRelief, spheric, min, max)
        {
        }
    }

    public class BasinDataBase<T> : WaterMoving<T> where T : Basin3
    {
        public BasinDataBase(HealpixManager man, bool withRelief = false, bool spheric = false,
            double? min = null, double? max = null)
            : base(man, null, min, max)
        {
            Visual = basin => basin.hOQ;
            ColorsMiddle = 0;

            foreach (var basin in PixMan.Pixels)
            {
                if (spheric)
                {
                    basin.InitROfEllipse(HealpixManager, Ellipsoid.MeanRadius);
                    basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
                }

                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = PixMan.Pixels[man.Neibors.Get(to, basin)];
                    basin.Neibors[to] = toBasin;

                    basin.froms[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);

                    basin.MeanEdges[(int)to] = man.Neibors.MeanBoundary(basin, to);
                }
                //basin.CorrectionSurface();
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];
                    basin.InitialHto[to] = basin.Metric(toBasin, to);
                }

                if (withRelief)
                {
                    int waterHeight;
                    var hOQ = GetHeights(basin, (int)basin.rOfEllipse, out waterHeight);
                    basin.hOQ = hOQ;
                    if (waterHeight > 0)
                    {
                        basin.Depth = waterHeight - hOQ;
                    }
                    else
                    {
                        basin.Depth = -hOQ;
                    }
                }
            }
        }

        public override ReliefType ReliefBedType
        {
            get { return ReliefType.Tbi; }
        }

        public Func<T, double> Visual { get; set; }

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

        public override double? GetAltitude(T basin)
        {
            //return basin.Visual * 1000;
            if (basin.HasWater())
            {
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neibors[to];
                    var @from = basin.froms[to];
                    var koef
                        = .25;
                    //= basin.Koef[(int)to] / basin.Koef.Sum();

                    //todo balance deltaH relative to basin.WaterHeight
                    var height = basin.Hto[to] - toBasin.Hto[@from];

                    var movedFromBasin = Water.PutV(basin, toBasin,
                        height * koef,
                        to, @from);
                    if (Math.Abs(movedFromBasin) > 0)
                    {
                    }
                }
            }
            return basin.HasWater() ? Visual(basin) : (double?)null;
        }
    }
}