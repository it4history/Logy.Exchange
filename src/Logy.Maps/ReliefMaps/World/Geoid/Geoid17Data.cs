using System.Collections;
using System.Drawing;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;
using Point = System.Drawing.Point;

namespace Logy.Maps.ReliefMaps.World.Geoid
{
    public class Geoid17Data : DataForMap2D
    {
        public Geoid17Data(Map2DBase map) : base(map)
        {
        }

        public override double? GetAltitude(HealCoor basin)
        {
            /*var surface = Relief.GetAltitude(basin);

            if (surface > 0 && surface < 20)
            {
                // lakes in ice are ignored
                return 1;
            }*/

            return 0;
        }

        public override void Draw(
            Bitmap bmp,
            double deltaX = 0,
            IEnumerable basins = null,
            int yResolution = 2,
            int scale = 1,
            Projection projection = Projection.Healpix)
        {
            base.Draw(bmp, deltaX, basins, yResolution, scale, projection);

            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(HealpixManager, yResolution);

            var geo = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(NeManager.Filepath));
            foreach (var feature in geo.Features)
            {
                var multiPolygon = feature.Geometry as MultiPolygon;
                if (multiPolygon != null)
                {
                    for (var i = 0; i < multiPolygon.Coordinates.Count; i++)
                    {
                        var polygon = multiPolygon.Coordinates[i];
                        DrawPolygon(polygon, equirectangular, g);
                    }
                }
                else
                {
                    DrawPolygon((Polygon)feature.Geometry, equirectangular, g);
                }
            }
            g.Flush();
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