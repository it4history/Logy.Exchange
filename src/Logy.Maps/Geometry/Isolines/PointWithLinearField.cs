using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.Geometry.Isolines
{
    public class PointWithLinearField : HealCoor
    {
        public PointWithLinearField() { }
        public PointWithLinearField(double x, double y) : base(x, y) { }

        // 0..1 range
        public double Intensity { get; set; }
    }
}