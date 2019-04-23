using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
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

        public RotationStopMap()
        {
            if (K < 7)
            {
                YResolution = 3;
                Scale = (7 - K) * 3;
                /*if (K < 5)
                {
                    Scale += 7 - 5;
                }*/
            }
            if (K == 7)
            {
                Scale = 2;
            }
        }
        
        public WaterMoving<T> Data { get; set; }

        protected override int K
        {
            get { return 7; }
        }

        public override Projection Projection
        {
            get { return Projection.Healpix; }
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Jpeg; }
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
            var diff = Data.RecheckOcean();
            File.AppendAllText(Path.Combine(Dir, "stats.txt"),
                string.Format("ocean volume diff: {0}", diff));
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
                        if (line < Bmp.Width)
                            for (var y = -10; y < 0; y++)
                                Bmp.SetPixel(line, YResolution * Scale * HealpixManager.Nside + y, Color.Black);
                    }
                }
                return SaveBitmap(Bmp, Data.Colors, Data.Accuracy, step);
            }
            return null;
        }

        public void ChangeAxis(
            double angle = 17,
            int framesCountBy2 = 60,
            Func<int, int> timeKoefByStep = null,
            bool slow = false,
            Func<int> slowSteps = null)
        {
            //// fill Basin.Visual to see centrifugal components

            T pole = null;
            var accur = 1.5;

            var slowCyclesCount = 10;
            var slowCycle = slow ? 1 : slowCyclesCount;

            Data.Cycle(delegate(int step)
            {
                if (step == 0 || slow && step % slowSteps() == 0 && slowCycle++ < slowCyclesCount)
                {
                    var angleLong = angle * slowCycle / slowCyclesCount;
                    var angleLat = -40d * slowCycle / slowCyclesCount;
                    EllipsoidAcceleration.AxisOfRotation =
                        Basin3.Oz
                            .Rotate(
                                new UnitVector3D(0, 1, 0),
                                new Angle(angleLong, AngleUnit.Degrees))
                            .Rotate(
                                new UnitVector3D(0, 0, 1),
                                new Angle(angleLat, AngleUnit.Degrees))
                        ;

                    //InitiialHtoRecalc();
                    ChangeRotation(step - HealpixManager.Nside, 0);

                    foreach (var b in Data.PixMan.Pixels)
                    {
                        if (Math.Abs(b.X - angleLat) < accur && Math.Abs(b.Y - (90 - angleLong)) < accur)
                        {
                            pole = b;
                            break;
                        }
                    }
                }

                Data.Draw(Bmp, 0, null, YResolution, Scale);
                Circle(pole, .03);
                SaveBitmap(step);
                return timeKoefByStep == null ? 15 : timeKoefByStep(step); // 15 for k4, 80 for k5 of Meridian
            }, framesCountBy2);
        }

        protected void Circle(T basin, double r = .2)
        {
            if (basin != null)
            {
                var width = .03;//.05 for k4
                foreach (var pixel in Data.PixMan.Pixels)
                {
                    var healCoor = (HealCoor)pixel;
                    var dist = basin.DistanceTo(healCoor);
                    if (Data.Colors != null
                        && dist >= r - width && dist <= r + width)
                    {
                        var eqProjection = new Equirectangular(HealpixManager, YResolution);
                        var point = eqProjection.Offset(healCoor);
                        Data.Colors.SetPixelOnBmp(Color.FromArgb(255, 174, 201), Bmp,
                            (int)(point.X), (int)point.Y, Scale);
                    }
                }
            }
        }

        protected void ChangeRotation(int step, double koef = 10000)
        {
            if (koef > 0 && EllipsoidAcceleration.SiderealDayInSeconds < double.MaxValue / 2
                || -koef < EllipsoidAcceleration.SiderealDayInSeconds)
            {
                EllipsoidAcceleration.SiderealDayInSeconds += koef;
                foreach (var basin in Data.PixMan.Pixels)
                {
                    if (Data.SamePolesAndEquatorGravitation)
                        basin.gHpure = 0;
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