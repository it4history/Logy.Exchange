#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Coloring;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    [TestFixture]
    public abstract class Map2DBase<T> : Map where T : HealCoor
    {
        public const string Slow = "Slow maps saved into files";

        private string _dir;

        protected Map2DBase()
        {
            LegendNeeded = true;
        }

        public virtual Projection Projection => Projection.Healpix;

        /// <summary>
        /// practical values 2, 3, 4
        /// </summary>
        public int YResolution { get; set; } = 2;
        public int Scale { get; set; } = 1;

        #region colors
        public virtual SortedList<int, Color3> ColorsAbove => ColorsManager.Gyr1;

        public virtual SortedList<int, Color3> ColorsUnder => ColorsManager.Water;

        protected virtual bool IsGrey => false;

        /// <summary>
        /// not for png
        /// </summary>
        protected Brush Background { get; set; } = Brushes.White;
        #endregion

        protected int Frames { get; set; } = 1;

        protected virtual DataForMap2D<T> MapData => null;

        protected string Dir => _dir ?? (_dir = string.Format(
                                    "{2}\\maps\\{1}_lines{0}",
                                    YResolution * HealpixManager.Nside,
                                    GetType().Name,
                                    Directory.GetCurrentDirectory()));

        protected virtual ImageFormat ImageFormat => ImageFormat.Jpeg;

        protected bool LegendNeeded { get; set; }

        private bool LegendToDraw => !IsGrey && LegendNeeded;

        private int LegendHeight => K > 7 ? (K - 6) * 20 : 20;

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
            Process.Start(fileName);

            data.Log();
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

            var top = YResolution * HealpixManager.Nside * Scale;
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
                    bmp.SetPixel(x, top + y, c);
                }
            }

            var g = GetFont(bmp);
            var font = new Font("Tahoma", 8 + (K > 7 ? (K - 7) * 8 : 0));
            var s = data.Colors.Min.ToString("0.#");
            var measure = g.MeasureString(s, font);
            var stringTop = top + 3 + ((LegendHeight - 3 - measure.Height) / 2);
            var layoutRectangle = new RectangleF(
                left - 9 - measure.Width,
                stringTop,
                left,
                top + LegendHeight);
            g.DrawString(s, font, Brushes.Black, layoutRectangle);

            s = $"{data.Colors.Max:0.#}{data.Dimension}";
            layoutRectangle = new RectangleF(
                left + (2 * HealpixManager.Nside * Scale) + 10,
                stringTop,
                left + (3 * HealpixManager.Nside * Scale),
                top + LegendHeight);
            g.DrawString(s, font, Brushes.Black, layoutRectangle);

            if (left0.HasValue)
            {
                var middle = data.Colors.Middle.ToString("0.#");
                var measure0 = g.MeasureString(middle, font);
                var rectangle = new Rectangle(
                    (int)Math.Round(left0.Value + 2 + ((left0end.Value - left0.Value - measure0.Width) / 2)),
                    (int)stringTop,
                    (int)measure0.Width + 1,
                    (int)measure0.Height - 2);
                g.FillRectangle(Brushes.White, rectangle);
                rectangle.X += K > 6 ? 3 : 0;
                g.DrawString(middle, font, Brushes.Black, rectangle);
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
    }
}
#endif