#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Coloring;
using Logy.Maps.Healpix;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.DotNetWikiBot.Sphere;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using Logy.MwAgent.Sphere;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    [TestFixture]
    public abstract class Map2DBase : Map
    {
        private const int Frames = 1;

        protected virtual DataForMap2D ApproximateData { get { return null; } }

        #region colors
        public virtual SortedList<int, Color3> ColorsAbove
        {
            get { return ColorsManager.Gyr1; }
        }

        public virtual SortedList<int, Color3> ColorsUnder
        {
            get { return ColorsManager.Water; }
        }

        protected virtual bool IsGrey
        {
            get { return false; }
        }
        #endregion

        protected virtual bool LegendNeeded
        {
            get { return true; }
        }

        private bool LegendToDraw
        {
            get { return !IsGrey && LegendNeeded; }
        }

        private int LegendHeight
        {
            get { return K > 7 ? (K - 6) * 20 : 20; }
        }

        protected virtual ImageFormat ImageFormat
        {
            get { return ImageFormat.Jpeg; }
        }

        public virtual Projection Projection
        {
            get { return Projection.Healpix; }
        }

        public int YResolution = 2;
        public int Scale = 1;

        [Test]
        public virtual void Draw()
        {
            var data = ApproximateData;
            var pixels = data.Basins();

            data.InitPoints(pixels, IsGrey);

            DrawFrames(pixels, data);

            data.Log();
        }

        internal void DrawFrame(Point2[] pixels, DataForMap2D data, Bitmap bmp, int step = 0)
        {
            switch (Projection)
            {
                case Projection.Healpix:
                    data.Draw(bmp, 0, pixels, YResolution);
                    break;
                case Projection.Healpix2Equirectangular:
                case Projection.Equirectangular:
                    foreach (var pixel in pixels)
                    {
                        var coor = Equirectangular.CoorFromXY(pixel, YResolution, HealpixManager, step);
                        double? altitude;
                        if (Projection == Projection.Healpix2Equirectangular)
                        {
                            var deltas = data.ApproxMan.GetDeltas(coor);
                            altitude = data.ApproxMan.GetMeanAltitude(deltas);
                            data.CheckMaxMin(altitude);
                        }
                        else
                            altitude = data.GetAltitude(new HealCoor(coor));

                        if (altitude.HasValue)
                            data.Colors.SetPixelOnBmp(altitude.Value, bmp, pixel, Scale);
                    }
                    break;
            }
        }

        private void DrawFrames(Point2[] pixels, DataForMap2D data)
        {
            for (var step = 0; step < Frames; step++)
            {
                var bmp = CreateBitmap();

                DrawFrame(pixels, data, bmp, step);

                if (LegendToDraw)
                {
                    DrawLegend(data, bmp);
                }

                Process.Start(SaveBitmap(bmp, data.Colors, data.Accuracy));
            }
        }

        internal void DrawLegend(DataEarth data, Bitmap bmp)
        {
            var top = YResolution * HealpixManager.Nside * Scale;
            var left = HealpixManager.Nside * Scale;
            const int upbottomedge = 3;
            int? left0 = null;
            int? left0end = null;
            for (var y = 0; y < LegendHeight; y++)
            {
                for (var x = 0; x < 4 * HealpixManager.Nside * Scale; x++)
                {
                    var c = ImageFormat == ImageFormat.Png ? Color.Transparent : Color.White;
                    if (x >= left && x < 3 * HealpixManager.Nside * Scale && y > upbottomedge &&
                        y < LegendHeight - upbottomedge)
                    {
                        var value = data.Colors.Min +
                                    (x - left) / (2d * HealpixManager.Nside * Scale) * (data.Colors.Max - data.Colors.Min);
                        var upbottomedge0 = upbottomedge + (K - 7) * 3;
                        if (y > upbottomedge0 + 2 && y < LegendHeight - upbottomedge0 - 1 &&
                            Math.Abs(value - data.Colors.Middle) < (data.Colors.Max - data.Colors.Min) / 50d)
                        {
                            if (left0.HasValue)
                                left0end = x;
                            else
                                left0 = x;
                        }
                        else
                            c = (Color) data.Colors.Get(value);
                    }
                    bmp.SetPixel(x, top + y, c);
                }
            }

            var g = GetFont(bmp);
            var font = new Font("Tahoma", 8 + (K > 7 ? (K - 7) * 8 : 0));
            var s = data.Colors.Min.ToString("0.#");
            var measure = g.MeasureString(s, font);
            var sTop = top + 3 + (LegendHeight - 3 - measure.Height) / 2;
            g.DrawString(s, font, Brushes.Black,
                new RectangleF(
                    left - 9 - measure.Width,
                    sTop,
                    left,
                    top + LegendHeight));
            s = string.Format("{0:0.#}{1}", data.Colors.Max, data.Dimension);
            g.DrawString(s, font, Brushes.Black,
                new RectangleF(
                    left + 2 * HealpixManager.Nside * Scale + 10,
                    sTop,
                    left + 3 * HealpixManager.Nside * Scale,
                    top + LegendHeight));

            if (left0.HasValue)
            {
                var middle = data.Colors.Middle.ToString("0.#");
                var measure0 = g.MeasureString(middle, font);
                var rectangle = new Rectangle(
                    (int) Math.Round(left0.Value + 2 + ((left0end.Value - left0.Value) - measure0.Width) / 2),
                    (int) sTop,
                    (int) measure0.Width + 1,
                    (int) measure0.Height - 2);
                g.FillRectangle(Brushes.White, rectangle);
                rectangle.X += 3;
                g.DrawString(middle, font, Brushes.Black,
                    rectangle);
            }
            g.Flush();
        }

        protected Graphics GetFont(Bitmap bmp)
        {
            var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            return g;
        }

        protected string SaveBitmap(Bitmap bmp, ColorsManager colors, int accuracy, int step = 0)
        {
            if (!Directory.Exists(Dir))
                Directory.CreateDirectory(Dir);
            var filename = Path.Combine(Dir, string.Format("{1}{0}{5}min{4}{3}.{2}",
                step == 0 ? "" : step.ToString("000") + "_",
                colors.IsGrey ? "grey" : "",
                ImageFormat.ToString().ToLower(),
                LegendToDraw ? "" : "_nolegend",
                Projection == Projection.Equirectangular ? "_noHEALPix" : "",
                accuracy));
            bmp.Save(filename,
                ImageFormat);
            return filename;
        }

        private string _dir;

        protected string Dir
        {
            get
            {
                return _dir ?? (_dir = string.Format(
                           "{2}\\maps\\{1}_lines{0}",
                           YResolution * HealpixManager.Nside,
                           GetType().Name,
                           Directory.GetCurrentDirectory()));
            }
        }

        internal Bitmap CreateBitmap()
        {
            return new Bitmap(
                4 * HealpixManager.Nside * Scale,
                YResolution * HealpixManager.Nside * Scale + (LegendToDraw ? LegendHeight : 0));
        }
    }
}
#endif