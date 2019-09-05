using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;
using Point = System.Drawing.Point;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class ComplexData : DataForMap2D<Basin3>
    {
        /// <summary>
        /// Key - abs. value of in/out volume
        /// </summary>
        private readonly SortedList<double, List<Current>> _arrows = new SortedList<double, List<Current>>();

        private WaterMoving<Basin3> _data;

        public ComplexData(Map2DBase<Basin3> map, WaterMoving<Basin3> data) : base(map, data.PixMan.Pixels)
        {
            _data = data;
        }

        public Rectangle<Basin3> Rectangle { get; set; }

        public bool ToAnalize(Basin3 basin)
        {
            return basin.HasWater(); // && Rectangle.Contains(basin)
        }

        public void CalcArrows()
        {
            foreach (var basin in PixMan.Pixels)
            {
                if (ToAnalize(basin))
                {
                    double? heights = 0;
                    var maxHeight = 0d;
                    var maxHeightSigned = 0d;
                    var maxDirection = -1;
                    for (var to = 0; to < 4; to++)
                    {
                        var toBasin = basin.Neighbors[to];
                        if (!toBasin.HasWater())
                        {
                            heights = null;
                            break;
                        }
                        var diff 
                            /// = basin.Hoq - toBasin.Hoq;
                         = _data.GetBasinHeight(basin, to) * basin.Koefs[to] * WaterModel.FluidityStable;
                        var height = Math.Abs(diff);
                        if (height > maxHeight)
                        {
                            maxHeight = height;
                            maxHeightSigned = diff;
                            maxDirection = to;
                        }
                        heights += diff;
                        // check of currents heights = maxHeight;
                    }
                    var count = 500;
                    if (heights.HasValue)
                    {
                        var heightsAbs = //heights.Value; 
                         Math.Abs(heights.Value);
                        if (_arrows.Count < count || _arrows.Keys[0] < heightsAbs)
                        {
                            List<Current> list;
                            if (!_arrows.TryGetValue(heightsAbs, out list))
                            {
                                list = new List<Current>();
                                _arrows.Add(heightsAbs, list);
                            }
                            list.Add(new Current
                            {
                                Basin = basin,
                                VolumeSigned = heights.Value,
                                Direction = maxDirection,
                                DirectionVolume = maxHeight,
                                DirectionVolumeSigned = maxHeightSigned
                            });
                            if (_arrows.Count >= count)
                                _arrows.RemoveAt(0);
                        }
                    }
                }
            }
        }

        public override double? GetAltitude(Basin3 basin)
        {
            if (ToAnalize(basin))
            {
                /*foreach (var arrows in _arrows)
                {
                    foreach (var current in arrows.Value)
                    {
                        if (current.Basin.P == basin.P)
                        {
                            var sign = arrows.Key * current.DirectionVolumeSigned;
                            return arrows.Key;
                        }
                    }
                }*/
                return basin.Hoq;
            }

            return null;

            // var jsonBasin = _jsonPixels[basin.P];
            // return jsonBasin.Polygon.SurfaceType == SurfaceType.Lake ? 1 : 0;
            /*(jsonBasin.RadiusGeoid - jsonBasin.RadiusOfEllipse);
            //return jsonBasin.HasWater() ? jsonBasin.Radius - basin.RadiusOfEllipse : (double?)null;*/
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

            DrawEddies(bmp, basins, yResolution, scale);
        }

        internal void DrawEddies(Bitmap bmp, IEnumerable basins, int yResolution, int scale)
        {
            var g = Graphics.FromImage(bmp);
            var equirectangular = new Equirectangular(HealpixManager, yResolution);

            var min = _arrows.Keys.First();
            var max = _arrows.Keys.Last();
            foreach (var arrows in _arrows)
            {
                foreach (var current in arrows.Value)
                {
                    var basinTo = current.Basin.Neighbors[current.Direction];
                    var point = equirectangular.Offset(current.Basin);
                    var pointTo = equirectangular.Offset(basinTo);

                    if (Math.Abs(pointTo.X - point.X) > HealpixManager.Nside)
                    {
                        Console.WriteLine(
                            $"line: {current.Basin}-{basinTo} should be drawn through Earth left and right edges");
                    }
                    else
                    {
                        var koefTill100 = (int)((arrows.Key - min) / (max - min) * 100d);
                        var color = Color.FromArgb(koefTill100, koefTill100, koefTill100);
                        var koefEnd = Math.Min(255, koefTill100 + 150);
                        var colorEnd = Color.FromArgb(koefEnd, koefEnd, koefEnd);
                        var pen = new Pen(color);

                        if (current.DirectionVolumeSigned < 0)
                        {
                            var t = pointTo;
                            pointTo = point;
                            point = t;
                        }

                        // ColorsManager.SetPixelOnBmp(color, bmp, point, scale);
                        var ar = pointTo + ((pointTo - point) * (current.DirectionVolume / Math.Abs(current.VolumeSigned)));
                        g.DrawLine(pen, (int)(point.X * scale), (int)(point.Y * scale), (int)(pointTo.X * scale), (int)(pointTo.Y * scale));
                        g.DrawLine(new Pen(colorEnd), (int)(pointTo.X * scale), (int)(pointTo.Y * scale), (int)(ar.X * scale), (int)(ar.Y * scale));
                    }
                }
            }

            g.Flush();
            Console.WriteLine($"arrows {min:#} .. {max:#}m");
        }

        private static void DrawPolygon(Polygon polygon, Equirectangular equirectangular, Graphics g)
        {
            var positions = polygon.Coordinates[0].Coordinates;
            var points = new Point[positions.Count];
            for (var i = 0; i < positions.Count; i++)
            {
                var position = (GeographicPosition)positions[i];
                var coor = new Coor(position.Longitude, position.Latitude);
                var point = equirectangular.Offset(coor);
                points[i] = new Point((int)point.X, (int)point.Y);
            }
            g.DrawLines(new Pen(Color.Blue), points);
        }

        private void DrawPoliticalMap(Bitmap bmp, int yResolution)
        {
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