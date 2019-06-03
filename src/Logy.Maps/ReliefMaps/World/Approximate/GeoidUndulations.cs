using System.Collections.Generic;
using System.Drawing;
using Logy.Maps.Coloring;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Data;

namespace Logy.Maps.ReliefMaps.World.Approximate
{
    public class GeoidUndulations : Map2DBase
    {
        public override Projection Projection => Projection.Healpix; // Healpix2Equirectangular // Equirectangular

        public override SortedList<int, Color3> ColorsAbove => new SortedList<int, Color3>
        {
            { 0, new Color3(80, 173, 173) },
            { 10, new Color3(Color.Green) },
            { 70, new Color3(Color.Yellow) },
            { 80, new Color3(Color.SandyBrown) },
            { 100, new Color3(Color.Red) },
        };

        /*protected override SortedList<int, Color3> ColorsUnder
        {
            get
            {
                return null; //ColorsManager.Water; 
            }
        }*/

        protected override int K => 9;

        protected override DataForMap2D ApproximateData => new GeoidUndulationsData(this);
    }
}
