using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public class RotationStopMap<T> : Map2DBase where T : BasinBase
    {
        private bool _jsonNeeded;

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

        private string StatsFileName => Path.Combine(Dir, "stats.json");

        public void SetData(Algorithm<T> algorithm, bool jsonNeeded = false)
        {
            _jsonNeeded = jsonNeeded;
            algorithm.DataAbstract.Init();
            if (File.Exists(StatsFileName))
            {
                Bundle = Bundle<T>.Deserialize(File.ReadAllText(StatsFileName));
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

        /// <summary>
        /// TearDown() is not called during debug
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Data.IsRunning = false;
            Data.RunningTask.Wait();

            if (_jsonNeeded)
            {
                Bundle.Algorithm.Diff = Data.RecheckOcean();
                using (var file = File.CreateText(StatsFileName))
                {
                    new JsonSerializer().Serialize(file, Bundle);
                }
            }
            else
                SaveBitmap(Data.Frame - 1);
            Process.Start(GetFileName(Data.Colors, FrameToString(Data.Frame - 1)));
        }

        public void ShiftAxis(
            int framesCount = 200, /* for k6 */
            Func<int> slowFrames = null,
            Func<int, int> timeStepByFrame = null)
        {
            var algorithm = Bundle.Algorithm as ShiftAxis;
            algorithm.Shift(
                framesCount,
                slowFrames,
                delegate (int frame)
                {
                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    Circle(algorithm.CurrentPoleBasin, .03);
                    SaveBitmap(frame);
                },
                timeStepByFrame);
        }

        protected void Circle(HealCoor basin, double r = .2)
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

        protected void SaveBitmap(int frame)
        {
            if (K > 3)
            {
                DrawLegend(Data, Bmp);

                var projection = new Equirectangular(HealpixManager, YResolution);
                var point = projection.Offset(Data.PixMan.Pixels[HealpixManager.RingsCount / 2]);
                var algorithm = Bundle.Algorithm as ShiftAxisGeneric<T>;
                if (algorithm != null)
                    foreach (var axisShiftFrame in algorithm.Poles.Keys)
                    {
                        var line = (int)Math.Max(0, point.X + axisShiftFrame);

                        if (line < Bmp.Width)
                            for (var y = -10; y < 0; y++)
                                Bmp.SetPixel(line, (YResolution * Scale * HealpixManager.Nside) + y, Color.Black);
                    }
            }
            SaveBitmap(Bmp, Data.Colors, FrameToString(frame));
        }

        private static string FrameToString(int frame)
        {
            return $"{frame:00000}";
        }
    }
}