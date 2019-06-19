using System.Collections;
using System.Drawing;
using GeoJSON.Net.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using Point = System.Drawing.Point;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid17Data : DataForMap2D<Basin3>
    {
        private readonly Basin3[] _jsonPixels;

        public Geoid17Data(Map2DBase<Basin3> map, Basin3[] jsonPixels) : base(map)
        {
            _jsonPixels = jsonPixels;
        }

        public override double? GetAltitude(Basin3 basin)
        {
            var jsonBasin = _jsonPixels[basin.P];
            return jsonBasin.HasWater() ? jsonBasin.Radius - basin.RadiusOfEllipse : (double?)null;
        }

        public override void Draw(
            Bitmap bmp,
            double deltaX = 0,
            IEnumerable basins = null,
            int yResolution = 2,
            int scale = 1,
            Projection projection = Projection.Healpix)
        {
            SetColorLists();
            base.Draw(bmp, deltaX, basins, yResolution, scale, projection);

            /*
            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(HealpixManager, yResolution);

            var geo = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(NeManager.Filepath));
            foreach (var feature in geo.Features)
            {
                var multiPolygon = feature.Geometry as MultiPolygon;
                if (multiPolygon != null)
                {
                    foreach (var polygon in multiPolygon.Coordinates)
                    {
                        DrawPolygon(polygon, equirectangular, g);
                    }
                }
                else
                {
                    DrawPolygon((Polygon)feature.Geometry, equirectangular, g);
                }
            }
            g.Flush();
            */
        }

        private static void DrawPolygon(Polygon polygon, Equirectangular equirectangular, Graphics g)
        {
            var positions = polygon.Coordinates[0].Coordinates;
            var points = new Point[positions.Count];
            for (var i = 0; i < positions.Count; i++)
            {
                var position = (GeographicPosition)positions[i];
                var coor = new Coor { X = position.Longitude, Y = position.Latitude };
                var point = equirectangular.Offset(coor);
                points[i] = new Point((int)point.X, (int)point.Y);
            }
            g.DrawLines(new Pen(Color.Blue), points);
        }

        /*      private void DrawPolygon(Graphics g, Topology topo, TopoJSONPolygon polygon, List<int> arcs, Equirectangular equirectangular, int scale)
                {
                    //var points = new Point[polygon.Coordinates.Count-1];
                    //polygon.ArcIdx
                    var points = new List<Point>();
                    var arcIndex = -1;
                    var indexToCheckArc = 0;
                    for (var i = 0; i < polygon.Coordinates.Count - 1; i++)
                    {
                        var position = polygon.Coordinates[i];
                        if (i == indexToCheckArc)
                        {
                            if (i > 0 &&
                                position.Latitude != polygon.Coordinates[i - 1].Latitude &&
                                position.Longitude != polygon.Coordinates[i - 1].Longitude)
                            {
                            }

                            arcIndex++;
                            if (arcIndex == arcs.Count)
                            {
                                // polygon has more positions than in its arcs
                                break;
                            }

                            var arcIndexAtPolygon = arcs[arcIndex];
                            if (arcIndexAtPolygon < 0)
                            {
                                i += topo.Arcs[-arcIndexAtPolygon].Positions.Count;
                                indexToCheckArc = i;
                                i--; // i++ compensation
                                continue;
                            }
                            indexToCheckArc = i + topo.Arcs[arcIndexAtPolygon].Positions.Count;
                        }

                        var coor = new Coor { X = position.Longitude, Y = position.Latitude };
                        var point = equirectangular.Offset(coor);
                        points.Add(new Point((int)point.X, (int)point.Y));
                    }
                    if (points.Count>1)
                        g.DrawLines(new Pen(Color.Blue), points.ToArray());                    */
    }
}