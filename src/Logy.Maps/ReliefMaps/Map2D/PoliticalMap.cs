using System.Drawing;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public static class PoliticalMap
    {
        public static void Draw(Bitmap bmp, HealpixManager man, int yResolution, int scale)
        {
            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(man, yResolution);

            var geo = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(NeManager.Filepath));
            foreach (var feature in geo.Features)
            {
                var multiPolygon = feature.Geometry as MultiPolygon;
                if (multiPolygon != null)
                {
                    foreach (var polygon in multiPolygon.Coordinates)
                    {
                        DrawPolygon(polygon, equirectangular, scale, g);
                    }
                }
                else
                {
                    DrawPolygon((Polygon)feature.Geometry, equirectangular, scale, g);
                }
            }
            g.Flush();
        }

        private static void DrawPolygon(Polygon polygon, Equirectangular equirectangular, int scale, Graphics g)
        {
            var positions = polygon.Coordinates[0].Coordinates;
            var points = new System.Drawing.Point[positions.Count];
            for (var i = 0; i < positions.Count; i++)
            {
                var position = (GeographicPosition)positions[i];
                var coor = new Coor(position.Longitude, position.Latitude);
                var point = equirectangular.OffsetDouble(coor, scale);
                points[i] = new System.Drawing.Point((int)point.X, (int)point.Y);
            }
            g.DrawLines(new Pen(Color.Gray), points);
        }
    }
}