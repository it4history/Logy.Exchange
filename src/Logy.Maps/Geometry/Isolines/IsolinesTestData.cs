using System;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Geometry.Isolines
{
    public class IsolinesTestData : DataForMap2D<PointWithLinearField>
    {
        internal const int IsolinesCount = 10;
        private readonly int _pointsCount;

        public readonly PointWithLinearField[] Points;
        public IsolinesTestData(Map2DBase<PointWithLinearField> map, int pointsCount)
            : base(map, new PointWithLinearField[0])
        {
            _pointsCount = pointsCount;

            Dimension = "";

            Points = new PointWithLinearField[_pointsCount];
            var rand = new Random();
            for (var i = 0; i < _pointsCount; i++)
            {
                Points[i] = new PointWithLinearField((rand.NextDouble() * 2 - 1) * ReliefMaps.Basemap.Map.BitmapScaleX, (rand.NextDouble() * 2 - 1) * ReliefMaps.Basemap.Map.BitmapScaleY)
                {
                    Intensity = rand.NextDouble()
                };
            }
        }

        private double? LinearField(Point2 basin, bool last = false)
        {
            var sumOfIntensity = 0d;
            foreach (var point in Points)
            {
                var distance = (point - basin).Distance;
                if (last)
                {
                    var dotRadius = point.Intensity * 2 + 1;
                    if (distance < dotRadius)
                        return null;
                }

                sumOfIntensity += point.Linear(distance);
            }
            return sumOfIntensity / _pointsCount;
        }

        public bool DrawIsoline(PointWithLinearField basin, double potential, double lowLevel, double highLevel)
        {
            var lineWidth = 0.1 * Math.Pow(2, 10 - K);
            var count = IsolinesCount + 1;
            double range = (highLevel - lowLevel) / count;
            for (var i = 0; i < count; i++)
            {
                var lowLevelStart = lowLevel + (i + 1) * range;
                if (potential > lowLevelStart && potential < lowLevelStart + range * (highLevel - lowLevel)
                    * lineWidth
                    * ((i + 1d) / (i + 2d)) // making first lines thiner
                    )
                    return true;
            }
            return false;
        }

        public override double? GetAltitude(PointWithLinearField basin)
        {
            if (Colors == null)
                return LinearField(basin); // calculating Colors min and max

            var potential = LinearField(basin, true);
            if (potential == null) // red point
                return Colors.Max;

            if (DrawIsoline(basin, potential.Value, Colors.Min, Colors.Max))
                return potential;

            return null;
        }
    }
}