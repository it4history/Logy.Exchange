using System.Collections.Generic;
using System.Drawing;
using Logy.Maps.Coloring;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World
{
    /// <summary>
    /// http://logy.gq/lw/Earth2014#issue
    /// </summary>
    public class Earth2014Correction : Map2DBase<HealCoor>
    {
        public Earth2014Correction()
        {
            Scale = 3;
        }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        public override SortedList<int, Color3> ColorsUnder => new SortedList<int, Color3>
        {
            { 0, new Color3(Color.LightGray) }
        };

        protected override DataForMap2D<HealCoor> MapData
            => new Earth2014CorrectionData(this) { Accuracy = 1 };
    }
}