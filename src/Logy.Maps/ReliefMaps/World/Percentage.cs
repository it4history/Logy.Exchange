using System.Collections.Generic;
using Logy.Maps.Coloring;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World
{
    public class Percentage : Map2DBase<HealCoor>
    {
        public Percentage() : base(6, LegendType.None)
        {
        }

        public override Projection Projection => Projection.Healpix2Equirectangular;

        public override SortedList<int, Color3> ColorsAbove => new SortedList<int, Color3>
        {
            { 0, ColorsManager.WaterBorder },
            { 100, new Color3(ColorsManager.Land) },
        };

        protected override DataForMap2D<HealCoor> MapData => new PercentageData(this);
    }
}