using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Geometry.Isolines
{
    public class PointWithLinearField : HealCoor
    {
        public PointWithLinearField(Coor coor) : base(coor) { }
        public PointWithLinearField(double x, double y) : base(x, y) { }

        // 0..1 range
        public double Intensity { get; set; }
    }
}