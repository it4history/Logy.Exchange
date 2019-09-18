using System.Drawing;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Exchange;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(5)
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
        }

        [Test]
        public void AxisChange()
        {
            // Subdir = "from 423";
            var data = new OceanData(HealpixManager)
            {
                WithRelief = true,
            };

            SetData(new ShiftAxis(data) { Slow = true, Geoisostasy = true }, true);

            ShiftAxisBalanced(4000);
        }

        [Test]
        public void WithContours()
        {
            Subdir = "checkEddies";

            // var map = new ReliefAxis17Geoisostasy(K);
            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(StatsFileName(3674)));
            /*
            Data.CalcAltitudes(); 
            Data.SetColorLists();
            Draw();
            DrawPoliticalMap(Bmp); */

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                }, 
                Data.Frame + 3);
            Draw();
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

        private void DrawPoliticalMap(Bitmap bmp)
        {
            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(HealpixManager, YResolution);

            var geo = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(NeManager.Filepath));
            foreach (var feature in geo.Features)
            {
                var multiPolygon = feature.Geometry as MultiPolygon;
                if (multiPolygon != null)
                {
                    foreach (var polygon in multiPolygon.Coordinates)
                    {
                        DrawPolygon(polygon, equirectangular, Scale, g);
                    }
                }
                else
                {
                    DrawPolygon((Polygon)feature.Geometry, equirectangular, Scale, g);
                }
            }
            g.Flush();
        }
    }
}