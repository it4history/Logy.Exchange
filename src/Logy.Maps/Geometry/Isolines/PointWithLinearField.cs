using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Geometry.Isolines
{
    public class PointWithLinearField : HealCoor
    {
        private static readonly double MaxDistance = Math.Sqrt(Map.BitmapScaleX * Map.BitmapScaleX + Map.BitmapScaleY * Map.BitmapScaleY) / 2;

        public PointWithLinearField(Coor coor) : base(coor) { }
        public PointWithLinearField(double x, double y) : base(x, y) { }

        // 0..1 range
        public double Intensity { get; set; }

        public double Linear(double distance)
        {
            return Intensity * Math.Max(1 - distance / MaxDistance, 0);
        }
    }
}