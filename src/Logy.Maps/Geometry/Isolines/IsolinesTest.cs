#if DEBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.Isolines
{
    public class IsolinesTest : Map2DBase<HealCoor>
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

        protected override DataForMap2D<HealCoor> MapData => new IsolinesTestData(this, _pointsCount);

        protected override void DrawLegend(DataEarth data, Bitmap bmp)
        {
            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(HealpixManager, YResolution);
            var positions = ((IsolinesTestData)data).Points;
            var points = new List<Point>();
            foreach (var position in positions)
            {
                var point = equirectangular.OffsetDouble(position, Scale);
                points.Add(new Point((int)point.X, (int)point.Y));
            }

            var convex = new List<Point>(new Grehem().CalcGrehem(points.ToArray()));
            foreach (var position in convex)
            {
                Console.WriteLine(position.ToString());
            }

            convex.Add(convex[0]);
            g.DrawLines(new Pen(Color.Gray), convex.Where(p => p.X > 0 || p.Y > 0).ToArray());

            g.Flush();

            base.DrawLegend(data, bmp);
        }
    }
}
#endif