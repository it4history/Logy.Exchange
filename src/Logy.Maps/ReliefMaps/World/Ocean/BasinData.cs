using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class BasinData : WaterMoving<Basin3>
    {
        public override ReliefType ReliefBedType
        {
            get { return ReliefType.Tbi; }
        }

        public BasinData(HealpixManager man, bool withRelief = false, bool spheric = false,
            double? min = null, double? max = null)
            : base(man, null, min, max)
        {
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

                    basin.InitialHto[(int)to] = basin.Metric(toBasin, to);
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

        internal override void GradientAndHeightCrosses()
        {
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                if (basin.HasWater())
                {
                    foreach (Direction to in Enum.GetValues(typeof(Direction)))
                    {
                        var toBasin = basin.Neibors[to];
                        var hto = basin.Metric(toBasin, to);
                        basin.Hto[(int)to] = hto;
                    }
                }
            }
        }

        public override double? GetAltitude(Basin3 basin)
        {
            //return basin.Visual * 1000;
            if (basin.HasWater())
            {
                if (basin.Type==Direction.Nw)
                { }
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = basin.Neibors[to];

                    var @from = basin.froms[(int)to];
                    var koef 
                        = .25;
                        //= basin.Koef[(int)to] / basin.Koef.Sum();
                    //var koef = basin.Koef2[(int)to] / basin.Koef2.Sum(); koef = koef2;

                    //todo balance deltaH relative to basin.WaterHeight
                    var height = basin.Hto[(int)to] - toBasin.Hto[(int)@from];

                    var moved = Water.PutV(basin, toBasin,
                        height * koef, 
                        (int) to, @from);
                    if (Math.Abs(moved) > 0)
                    {
                    }
                }
            }
            return basin.HasWater() ? basin.hOQ : (double?)null;
        }
    }
}