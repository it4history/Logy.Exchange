using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.Isolines
{
    public class IsolinesTest : Map2DBase<PointWithLinearField>
    {
        private readonly int _pointsCount;

        public IsolinesTest() : this(9, 10)
        {
        }

        public IsolinesTest(int k, int pointsCount) : base(k)
        {
            _pointsCount = pointsCount;
            YResolution = 2;
            Frames = 1;
        }

        public override Projection Projection => Projection.NoHealpix;
        protected override string FilenameSuffix => $"{_pointsCount}point{(_pointsCount > 1 ? "s" : null)}_{BitmapHeight - LegendHeight}lines";

        protected override DataForMap2D<PointWithLinearField> MapData
            => new IsolinesTestData(this, _pointsCount);

        protected override void DrawLegend(DataEarth data, Bitmap bmp)
        {
            var points = ((IsolinesTestData)data).Points;
            if (points.Length > 2)
            {
                var resultPoints = new List<Point>();
                var equirectangular = new Equirectangular(HealpixManager, YResolution);
                foreach (var position in points)
                {
                    var point = equirectangular.OffsetDouble(position, Scale);
                    resultPoints.Add(new Point((int)point.X, (int)point.Y));
                }

                var convex = new List<Point>(new Grehem().CalcGrehem(resultPoints.ToArray())
                        .Where(p => p.X > 0 || p.Y > 0));
                convex.Add(convex[0]);
                var g = Graphics.FromImage(bmp);
                g.DrawLines(new Pen(Color.Gray), convex.ToArray());
                g.Flush();
            }

            base.DrawLegend(data, bmp);
        }
    }
}