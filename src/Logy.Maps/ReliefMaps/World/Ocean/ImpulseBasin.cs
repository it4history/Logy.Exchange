using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Расчет_импульса_в_уравнение_Навье-Стокса
    /// </summary>
    public class ImpulseBasin : Basin3
    {
        public Vector2D Impulse { get; set; }
    }

    public class ImpulseData : BasinDataAbstract<ImpulseBasin>
    {
        public ImpulseData()
        {
        }

        public ImpulseData(
            HealpixManager man,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false)
            : base(man, null, min, max, readAllAtStart)
        {
        }

        public override double? GetAltitude(ImpulseBasin basin)
        {
            var moved = 0d;

            if (basin.HasWater())
            {
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neighbors[to];
                    var from = basin.Opposites[to];
                    var heightWithKoef = GetBasinHeight(basin, toBasin, to, from) * basin.Koefs[to];

                    var movedVolume = Water.PutV(
                        basin,
                        toBasin,
                        heightWithKoef,
                        to,
                        from);
                    if (Math.Abs(movedVolume) > 0)
                    {
                        if (heightWithKoef > 0) // from basin
                        {
                            // let impulse go from basin.Impulse 
                            basin.Impulse -= Vector((Direction) to) * heightWithKoef;
                        }
                        else
                        {
                            
                        }

                        /* reverse
                        toBasin.WaterIn(volumeToBasin.Value, from);

                        // water out source basin
                        basin.WaterIn(-volumeFromBasin, to);
                        */

                        moved += heightWithKoef;
                    }
                }
                basin.Impulse *= .95;
            }
            return Visual(basin, moved * Water.Fluidity); // basin.hOQ;
        }

        private Vector2D Vector(Direction to)
        {
            switch (to)
            {
                case Direction.Ne:
                    return new Vector2D(1, 1);
                case Direction.Nw:
                    return new Vector2D(0, 1);
                case Direction.Se:
                    return new Vector2D(1, 0);
                default:
                    return new Vector2D(0, 0);
            }
        }
    }
}
 