using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    [Category(Slow)]
    public class RotationStopMap<T> : Map2DBase<T> where T : BasinAbstract
    {
        private bool _jsonNeeded;

        public RotationStopMap() : this(6)
        {
        }
        public RotationStopMap(int k) : base(k)
        {
            if (k < 7)
            {
                YResolution = 3;
                Scale = (7 - k) * 3;
                /*if (k < 5)
                {
                    Scale += 7 - 5;
                }*/
            }
            if (k == 7)
            {
                Scale = 2;
            }
        }

        public Bitmap Bmp { get; set; }

        public Bundle<T> Bundle { get; protected set; }

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
                        Array.ForEach(Directory.GetFiles(Dir), File.Delete);
                    value.Init();
                    Bundle = new Bundle<T>(value);
                }
                else
                    throw new ApplicationException("Bundle already set");
            }
        }

        protected override ImageFormat ImageFormat => ImageFormat.Jpeg;

        public string StatsFileName(int? frame = null)
        {
            return Path.Combine(Dir, $"stats{FrameToString(frame)}.json");
        }

        /// <summary>
        /// like a constructor
        /// </summary>
        public void SetData(Algorithm<T> algorithm, bool jsonNeeded = false)
        {
            _jsonNeeded = jsonNeeded;
            if (File.Exists(StatsFileName()))
            {
                Bundle = Bundle<T>.Deserialize(File.ReadAllText(StatsFileName()));
                var dataInJson = Bundle.Algorithm.DataAbstract;
                if (K != dataInJson.K)
                    throw new ApplicationException($"map needs K {K}");
                var dataInCode = algorithm.DataAbstract;
                if (dataInCode != null)
                {
                    if (dataInCode.WithRelief != dataInJson.WithRelief)
                        throw new ApplicationException("data WithRelief mismatch");
                }
            }
            else
            {
                algorithm.DataAbstract.Init();
                Bundle = new Bundle<T>(algorithm);
            }
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
            if (Data.IsRunning)
            {
                Data.IsRunning = false;
                Data.RunningTask.Wait();
            }
            if (_jsonNeeded)
            {
                SaveJson();
            }
            else
            {
                // for meridian maps mainly
                SaveBitmap(Data.Frame);
            }
            Process.Start(GetFileName(Data.Colors, FrameToString(Data.Frame)));
        }

        public void ShiftAxis(
            int framesCount = 200, /* for k6 */
            Func<int, int> slowFrames = null,
            Func<int, int> timeStepByFrame = null)
        {
            var algorithm = Bundle.Algorithm as ShiftAxis;
            var lastTimeOfSaveJson = algorithm.DataAbstract.Time;
            algorithm.Shift(
                framesCount,
                slowFrames,
                delegate(int frame)
                {
                    Draw();
                    SaveBitmap(algorithm.DataAbstract.Frame);
                    var time = algorithm.DataAbstract.Time;
                    if (_jsonNeeded && time > lastTimeOfSaveJson + 5000)
                    {
                        SaveJson(frame);
                        lastTimeOfSaveJson = time;
                    }
                },
                timeStepByFrame);
        }

        public override void Draw()
        {
            Data.Draw(Bmp, 0, null, YResolution, Scale, Projection);
        }

        protected static string FrameToString(int? frame)
        {
            return frame.HasValue ? $"{frame:00000}" : null;
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

        private void SaveJson(int? frame = null)
        {
            Bundle.Algorithm.Diff = Data.RecheckOcean();
            using (var file = File.CreateText(StatsFileName(frame)))
            {
                new JsonSerializer().Serialize(file, Bundle);
            }
        }
    }
}