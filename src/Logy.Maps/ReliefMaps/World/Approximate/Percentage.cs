using System.Collections.Generic;
using Logy.Maps.Coloring;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class Percentage : Map2DBase
    {
        public Percentage()
        {
            LegendNeeded = false;
        }

        public override Projection Projection => Projection.Healpix2Equirectangular;

        public override SortedList<int, Color3> ColorsAbove => new SortedList<int, Color3>
        {
            { 0, ColorsManager.WaterBorder },
            { 100, new Color3(ColorsManager.Land) },
        };

        protected override DataForMap2D MapData => new PercentageData(this);
    }
}