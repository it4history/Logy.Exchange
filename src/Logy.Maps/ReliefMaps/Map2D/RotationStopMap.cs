using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public class RotationStopMap<T> : Map2DBase where T : BasinBase
    {
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

        public Bitmap Bmp { get; set; }

        public Bundle<T> Bundle { get; private set; }

        public override Projection Projection => Projection.Healpix;

        public WaterMoving<T> Data
        {
            get
            {
                return Bundle.Algorithm.Data;
            }
            set
            {
                if (Bundle == null)
                {
                    if (Directory.Exists(Dir))
                        Directory.Delete(Dir, true);
                    Bundle = new Bundle<T>(value);
                }
                else
                    throw new ApplicationException();
            }
        }

        protected override int K => 6;

        protected override ImageFormat ImageFormat => ImageFormat.Jpeg;

        private string FileName => Path.Combine(Dir, "stats.json");

        public void SetData(Algorythm<T> algorythm)
        {
            if (File.Exists(FileName))
            {
                Bundle = JsonConvert.DeserializeObject<Bundle<T>>(File.ReadAllText(FileName));
            }
            else
                Bundle = new Bundle<T>(algorythm);
        }

        [SetUp]
        public virtual void SetUp()
        {
            Bmp = CreateBitmap();
        }

        [TearDown]
        public void TearDown()
        {
            Data.IsRunning = false;
            Data.RunningTask.Wait();
            Process.Start(SaveBitmap(Data.Frame));
            Bundle.Algorithm.Diff = Data.RecheckOcean();
            using (var file = File.CreateText(FileName))
            {
                new JsonSerializer().Serialize(file, Bundle);
            }
        }

        public void ShiftAxis(
            int framesCount = 200, /* for k6 */
            Func<int, int> timeKoefByFrame = null,
            Func<int> slowFrames = null)
        {
            var algorythm = Bundle.Algorithm as ShiftAxis;

            T pole = null;
            var poleAccur = 1.5;

            var slowFramesCount = 10;
            var slowFrame = algorythm.Slow ? 1 : slowFramesCount;

            Data.DoFrames(
                delegate(int frame)
                {
                    if (frame == 0
                        || (algorythm.Slow && frame % slowFrames() == 0 && slowFrame++ < slowFramesCount))
                    {
                        var newPole = new PoleNorth
                        {
                            X = algorythm.DesiredPoleNorth.X, //// * slowFrame / slowFramesCount
                            Y = 90 - ((90 - algorythm.DesiredPoleNorth.Y) * slowFrame / slowFramesCount)
                        };
                        algorythm.Poles.Add(frame, newPole);

                        EllipsoidAcceleration.AxisOfRotation =
                            Basin3.Oz
                                .Rotate(
                                    new UnitVector3D(0, 1, 0),
                                    new Angle(90 - newPole.Y, AngleUnit.Degrees))
                                .Rotate(
                                    new UnitVector3D(0, 0, 1),
                                    new Angle(newPole.X, AngleUnit.Degrees));

                        // InitiialHtoRecalc();
                        ChangeRotation(frame - HealpixManager.Nside, 0);

                        foreach (var b in Data.PixMan.Pixels)
                        {
                            if (Math.Abs(b.X - newPole.X) < poleAccur && Math.Abs(b.Y - newPole.Y) < poleAccur)
                            {
                                pole = b;
                                break;
                            }
                        }
                    }

                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    Circle(pole, .03);
                    SaveBitmap(frame);
                    return timeKoefByFrame?.Invoke(frame) ?? 15; // 15 for k4, 80 for k5 of Meridian
                },
                framesCount);
        }

        protected void Circle(T basin, double r = .2)
        {
            if (basin != null)
            {
                var width = .03; /// .05 for k4
                foreach (var pixel in Data.PixMan.Pixels)
                {
                    var healCoor = (HealCoor)pixel;
                    var dist = basin.DistanceTo(healCoor);
                    if (Data.Colors != null
                        && dist >= r - width && dist <= r + width)
                    {
                        var projection = new Equirectangular(HealpixManager, YResolution);
                        var point = projection.Offset(healCoor);
                        Data.Colors.SetPixelOnBmp(
                            Color.FromArgb(255, 174, 201), 
                            Bmp,
                            (int)point.X, 
                            (int)point.Y, 
                            Scale);
                    }
                }
            }
        }

        protected string SaveBitmap(int frame)
        {
            if (Data.Colors != null)
            {
                if (K > 3)
                {
                    DrawLegend(Data, Bmp);

                    var projection = new Equirectangular(HealpixManager, YResolution);
                    var point = projection.Offset(Data.PixMan.Pixels[HealpixManager.RingsCount / 2]);
                    var algorythm = Bundle.Algorithm as ShiftAxis;
                    foreach (var axisShiftFrame in algorythm.Poles.Keys)
                    {
                        var line = (int)Math.Max(0, point.X + axisShiftFrame);

                        if (line < Bmp.Width)
                            for (var y = -10; y < 0; y++)
                                Bmp.SetPixel(line, (YResolution * Scale * HealpixManager.Nside) + y, Color.Black);
                    }
                }
                return SaveBitmap(Bmp, Data.Colors, Data.Accuracy, $"{frame:00000}_");
            }
            return null;
        }

        protected void ChangeRotation(int frame, double koef = 10000)
        {
            if ((koef > 0 && EllipsoidAcceleration.SiderealDayInSeconds < double.MaxValue / 2)
                || -koef < EllipsoidAcceleration.SiderealDayInSeconds)
            {
                EllipsoidAcceleration.SiderealDayInSeconds += koef;
                foreach (var basin in Data.PixMan.Pixels)
                {
                    if (Data.SamePolesAndEquatorGravitation)
                        basin.GHpure = 0;
                    basin.RecalculateDelta_g();
                }
            }
        }
    }
}