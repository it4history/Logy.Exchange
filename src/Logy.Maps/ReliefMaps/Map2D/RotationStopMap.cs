using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Logy.Maps.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public class RotationStopMap<T> : Map2DBase where T : BasinBase
    {
        protected Bitmap Bmp;
        protected readonly List<int> ChangeLines = new List<int>();
        public WaterMoving<T> Data;

        [SetUp]
        public virtual void SetUp()
        {
            Bmp = CreateBitmap();
        }

        [TearDown]
        public void TearDown()
        {
            Process.Start(SaveBitmap());
        }

        protected string SaveBitmap(int step = 0)
        {
            if (Data.Colors != null)
            {
                DrawLegend(Data, Bmp);
                foreach (var line in ChangeLines)
                {
                    for (var y = -10; y < 15; y++)
                        Bmp.SetPixel(line, YResolution * Scale * HealpixManager.Nside + y, Color.Black);
                }
                return SaveBitmap(Bmp, Data.Colors, Data.Accuracy, step);
            }
            return null;
        }

        public override Projection Projection
        {
            get { return Projection.Healpix; }
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Png; }
        }


        protected void ChangeRotation(int step, double koef = 10000)
        {
            if (koef > 0 || -koef < EllipsoidAcceleration.SiderealDayInSeconds)
            {
                EllipsoidAcceleration.SiderealDayInSeconds += koef;
                foreach (var basin in Data.PixMan.Pixels)
                {
                    basin.RecalculateDelta_g();
                }

                var eqProjection = new Equirectangular(HealpixManager, YResolution);
                var point = eqProjection.Offset(Data.PixMan.Pixels[HealpixManager.RingsCount / 2]);
                var line = (int) (point.X + step);
                ChangeLines.Add(Math.Max(0, line));
                //Console.WriteLine(Ellipsoid.SiderealDayInSeconds);
            }
        }
    }
}