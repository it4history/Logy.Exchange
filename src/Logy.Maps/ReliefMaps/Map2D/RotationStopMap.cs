using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        private const double Pole2BasinAccuranceDegrees = 1.5;
        private T _currentPoleNorth;

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
                return Bundle.Algorithm.DataAbstract;
            }
            set
            {
                if (Bundle == null)
                {
                    if (Directory.Exists(Dir))
                        Directory.Delete(Dir, true);
                    value.Init();
                    Bundle = new Bundle<T>(value);
                }
                else
                    throw new ApplicationException("Bundle already set");
            }
        }

        protected override int K => 6;

        protected override ImageFormat ImageFormat => ImageFormat.Jpeg;

        private string FileName => Path.Combine(Dir, "stats.json");

        public void SetData(Algorithm<T> algorithm)
        {
            if (File.Exists(FileName))
            {
                Bundle = Bundle<T>.Deserialize(File.ReadAllText(FileName));
                if (K != Bundle.Algorithm.DataAbstract.K)
                    throw new ApplicationException($"map needs K {K}");
            }
            else
                Bundle = new Bundle<T>(algorithm);
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
            var algorithm = Bundle.Algorithm as ShiftAxis;

            SetPole(algorithm.Poles.Values.Last());

            var poleShiftsCount = 10;
            var poleShift = algorithm.Slow ? algorithm.Poles.Count : poleShiftsCount;

            Data.DoFrames(
                delegate(int frame)
                {
                    if (frame == 0
                        || (algorithm.Slow && frame % slowFrames() == 0 && poleShift++ < poleShiftsCount))
                    {
                        var newPole = new PoleNorth
                        {
                            X = algorithm.DesiredPoleNorth.X, //// * slowFrame / slowFramesCount
                            Y = 90 - ((90 - algorithm.DesiredPoleNorth.Y) * poleShift / poleShiftsCount)
                        };
                        SetPole(newPole, frame);
                    }

                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    Circle(_currentPoleNorth, .03);
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
                    var algorithm = Bundle.Algorithm as ShiftAxis;
                    if (algorithm != null)
                        foreach (var axisShiftFrame in algorithm.Poles.Keys)
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

        protected void ChangeRotation(double koef = 10000, int? frame = null)
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

                var algorithm = Bundle.Algorithm as ShiftAxis;
                if (frame.HasValue && algorithm != null)
                {
                    var lastPole = algorithm.Poles.Values.Last();
                    algorithm?.Poles.Add(
                        frame.Value,
                        new PoleNorth
                        {
                            X = lastPole.X,
                            Y = lastPole.X,
                            SiderealDayInSeconds = EllipsoidAcceleration.SiderealDayInSeconds
                        });
                }
            }
        }

        private void SetPole(PoleNorth newPole, int? frame = null)
        {
            EllipsoidAcceleration.AxisOfRotation =
                Basin3.Oz
                    .Rotate(
                        new UnitVector3D(0, 1, 0),
                        new Angle(90 - newPole.Y, AngleUnit.Degrees))
                    .Rotate(
                        new UnitVector3D(0, 0, 1),
                        new Angle(newPole.X, AngleUnit.Degrees));

            foreach (var b in Data.PixMan.Pixels)
            {
                if (Math.Abs(b.X - newPole.X) < Pole2BasinAccuranceDegrees && Math.Abs(b.Y - newPole.Y) < Pole2BasinAccuranceDegrees)
                {
                    _currentPoleNorth = b;
                    break;
                }
            }

            // InitiialHtoRecalc();
            ChangeRotation(0);

            if (frame.HasValue)
            {
                var algorithm = Bundle.Algorithm as ShiftAxis;
                algorithm?.Poles.Add(frame.Value, newPole);
            }
        }
    }
}