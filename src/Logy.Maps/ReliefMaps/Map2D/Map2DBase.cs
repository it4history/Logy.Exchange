using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Logy.Maps.Coloring;
using Logy.Maps.Exchange.Naturalearth;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.DotNetWikiBot;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    [TestFixture]
    public abstract class Map2DBase<T> : Map where T : HealCoor
    {
        public const string Slow = "Slow maps saved into files";

        private string _dir;

        protected Map2DBase(int k = 6) : base(k)
        {
            LegendNeeded = true;

            if (k < 7)
            {
                Scale = (7 - k) * 3;
                /*if (k < 5)
                {
                    Scale += 7 - 5;
                }*/
            }
            else if (k == 7)
            {
                Scale = 2;
            }
        }

        public virtual Projection Projection => Projection.Healpix;

        /// <summary>
        /// practical values 2, 3, 4
        /// </summary>
        public int YResolution { get; set; } = 3;
        public int Scale { get; set; } = 1;

        public int Top => HealpixManager.Nside * YResolution * Scale;

        /// <summary>
        /// used during Data setting, bmp saving, teardown
        /// </summary>
        public string Subdir { get; set; }
        public string Dir => _dir ?? (_dir = string.Format(
                                 "{2}{3}maps{3}{1}_lines{0}{3}{4}",
                                 YResolution * HealpixManager.Nside,
                                 GetType().Name,
                                 Directory.GetCurrentDirectory(),
                                 Path.DirectorySeparatorChar,
                                 Subdir));

        #region colors
        public virtual SortedList<int, Color3> ColorsAbove => ColorsManager.Gyr1;

        public virtual SortedList<int, Color3> ColorsUnder => ColorsManager.Water;

        protected virtual bool IsGrey => false;

        /// <summary>
        /// not for png
        /// </summary>
        protected Brush Background { get; set; } = Brushes.White;
        #endregion

        protected virtual ImageFormat ImageFormat => ImageFormat.Jpeg;

        protected int Frames { get; set; } = 1;

        protected virtual DataForMap2D<T> MapData => null;

        protected bool LegendNeeded { get; set; }

        private bool LegendToDraw => !IsGrey && LegendNeeded;

        private int LegendHeight => K > 7 ? (K - 6) * 20 : 20;

        public static void OpenPicture(string name)
        {
            if (Site.IsRunningOnMono)
                Process.Start(
                new ProcessStartInfo("xdg-open", name) { UseShellExecute = false });
            else
                Process.Start(name);
        }

        public static Graphics GetFont(Bitmap bmp)
        {
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            return g;
        }

        [Test]
        [Category(Slow)]
        public virtual void Draw()
        {
            var data = MapData;
            var pixels = data.Basins();

            data.InitPoints(pixels, IsGrey);

            string fileName = null;
            for (var frame = 0; frame < Frames; frame++)
            {
                var bmp = CreateBitmap();

                DrawFrame(pixels, data, bmp, frame);

                DrawLegend(data, bmp);

                fileName = SaveBitmap(bmp, data.Colors, frame == 0 ? null : $"{frame:000}_{data.Accuracy}min");
            }
            data.Log();
            OpenPicture(fileName);
        }

        protected Bitmap CreateBitmap()
        {
            var bmp = new Bitmap(
                4 * HealpixManager.Nside * Scale,
                (YResolution * HealpixManager.Nside * Scale) + (LegendToDraw ? LegendHeight : 0));
            if (ImageFormat != ImageFormat.Png)
            {
                var g = GetFont(bmp);
                g.FillRectangle(Background, 0, 0, bmp.Width, bmp.Height);
                g.Flush();
            }
            return bmp;
        }

        protected void DrawFrame(Point2[] pixels, DataForMap2D<T> data, Bitmap bmp, int frame = 0)
        {
            switch (Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                    data.Draw(bmp, 0, pixels, YResolution, Scale, Projection);
                    break;
                case Projection.Healpix2Equirectangular:
                case Projection.Equirectangular:
                    foreach (var pixel in pixels)
                    {
                        var coor = Equirectangular.CoorFromXY(pixel, YResolution, HealpixManager, frame);
                        double? altitude;
                        if (Projection == Projection.Healpix2Equirectangular)
                        {
                            var deltas = data.PixMan.GetDeltas(coor);
                            altitude = data.PixMan.GetMeanAltitude(deltas);
                            data.CheckMaxMin(altitude);
                        }
                        else
                            altitude = data.GetAltitude((T)Activator.CreateInstance(typeof(HealCoor), coor));

                        if (altitude.HasValue)
                            data.Colors.SetPixelOnBmp(altitude.Value, bmp, pixel, Scale);
                    }
                    break;
            }
        }

        protected void DrawLegend(DataEarth data, Bitmap bmp)
        {
            if (!LegendToDraw)
                return;

            var left = HealpixManager.Nside * Scale;
            const int Upbottomedge = 3;
            int? left0 = null;
            int? left0end = null;
            for (var y = 0; y < LegendHeight; y++)
            {
                for (var x = 0; x < 4 * HealpixManager.Nside * Scale; x++)
                {
                    var c = ImageFormat == ImageFormat.Png ? Color.Transparent : Color.White;
                    if (x >= left && x < 3 * HealpixManager.Nside * Scale && y > Upbottomedge &&
                        y < LegendHeight - Upbottomedge)
                    {
                        var value = data.Colors.Min +
                                    ((x - left) / (2d * HealpixManager.Nside * Scale) *
                                    (data.Colors.Max - data.Colors.Min));
                        var upbottomedge0 = Upbottomedge + ((K - 7) * 3);
                        if (y > upbottomedge0 + 2 && y < LegendHeight - upbottomedge0 - 1 &&
                            Math.Abs(value - data.Colors.Middle) < (data.Colors.Max - data.Colors.Min) / 50d)
                        {
                            if (left0.HasValue)
                                left0end = x;
                            else
                                left0 = x;
                        }
                        else
                            c = (Color)data.Colors.Get(value);
                    }
                    bmp.SetPixel(x, Top + y, c);
                }
            }

            var g = GetFont(bmp);
            var font = new Font("Tahoma", 8 + (K > 7 ? (K - 7) * 8 : 0));
            var smin = data.Colors.Min.ToString("0.#");
            var measure = g.MeasureString(smin, font);
            var stringTop = Top + 3 + ((LegendHeight - 3 - measure.Height) / 2);
            var layoutRectangle = new RectangleF(
                left - 9 - measure.Width,
                stringTop,
                left,
                Top + LegendHeight);
            g.DrawString(smin, font, Brushes.Black, layoutRectangle);

            var smax = data.Colors.Max.ToString("0.#");
            layoutRectangle = new RectangleF(
                left + (2 * HealpixManager.Nside * Scale) + 10,
                stringTop,
                left + (3 * HealpixManager.Nside * Scale),
                Top + LegendHeight);
            g.DrawString(smax + data.Dimension, font, Brushes.Black, layoutRectangle);

            var middle = data.Colors.Middle.ToString("0.#");
            if (left0.HasValue && middle != smin && middle != smax)
            {
                var measure0 = g.MeasureString(middle, font);
                var rectangle = new Rectangle(
                    (int)Math.Round(left0.Value + 2 + ((left0end.Value - left0.Value - measure0.Width) / 2), MidpointRounding.AwayFromZero),
                    (int)stringTop,
                    (int)measure0.Width + 1,
                    (int)measure0.Height - 2);
                g.FillRectangle(Brushes.White, rectangle);
                rectangle.X += K > 6 ? 3 : 0;
                g.DrawString(middle, font, Brushes.Black, rectangle);
            }
            g.Flush();
        }

        protected string GetFileName(ColorsManager colors, string frame = null)
        {
            var filename = string.Format(
                "{1}{0}{4}{3}.{2}",
                frame,
                colors.IsGrey ? "grey" : null,
                ImageFormat.ToString().ToLower(),
                LegendToDraw ? null : "_nolegend",
                Projection == Projection.Equirectangular ? "_noHEALPix" : null);
            return Path.Combine(Dir, filename);
        }

        protected string SaveBitmap(Bitmap bmp, ColorsManager colors, string frame = null)
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            var fileName = GetFileName(colors, frame);
            bmp.Save(fileName, ImageFormat);
            return fileName;
        }

        public static void DrawPoliticalMap(Bitmap bmp, HealpixManager man, int yResolution, int scale)
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