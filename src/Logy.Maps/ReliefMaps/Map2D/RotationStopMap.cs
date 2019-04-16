using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Logy.Maps.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public class RotationStopMap<T> : Map2DBase where T : BasinBase
    {
        protected Bitmap Bmp;
        protected readonly List<int> ChangeLines = new List<int>();
        public WaterMoving<T> Data;

        public RotationStopMap()
        {
            if (K < 7)
            {
                YResolution = 4;
                Scale = (7 - K) * 3;
            }
        }

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
                if (K > 3)
                {
                    DrawLegend(Data, Bmp);
                    foreach (var line in ChangeLines)
                    {
                        for (var y = -10; y < 15; y++)
                            Bmp.SetPixel(line, YResolution * Scale * HealpixManager.Nside + y, Color.Black);
                    }
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

        public virtual void Water_ChangeRotation()
        {
            //// fill Basin.Visual and uncomment BasinData.GetAltitude to see centrifugal components

            EllipsoidAcceleration.AxisOfRotation =
                //new UnitVector3D(1, 0, 0);
                //                Basin3.Oz.Rotate(new UnitVector3D(1, 0, 0), new Angle(15.0, AngleUnit.Degrees))
                Basin3.Oz
                    .Rotate(new UnitVector3D(0, 1, 0), new Angle(17, AngleUnit.Degrees))
                    .Rotate(new UnitVector3D(0, 0, 1), new Angle(-40, AngleUnit.Degrees))
                ;

            //InitiialHtoRecalc();

            ChangeRotation(-HealpixManager.Nside, 0);
            var framesCountBy2 = 50;
            Data.Cycle(15, delegate (int step)
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
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
                var line = (int)(point.X + step);
                ChangeLines.Add(Math.Max(0, line));
            }
        }
    }
}