using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.Projections;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Map2D
{
    [Category(Slow)]
    public class RotationStopMap<T> : Map2DBase<T> where T : BasinAbstract
    {
        private bool _jsonNeeded;

        public const string FilePrefix = "stats";

        public RotationStopMap() : this(6)
        {
        }
        public RotationStopMap(int k) : base(k)
        {
        }

        public Bitmap Bmp { get; set; }

        public Bundle<T> Bundle { get; protected set; }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        /// <summary>
        /// !setting may delete maps!
        /// </summary>
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

        /// <summary>
        /// Tiff is beatufil but ScreenToGif does not read it
        /// </summary>
        protected override ImageFormat ImageFormat => ImageFormat.Png;

        /// <summary>
        /// like a constructor
        /// </summary>
        public void InitData(Algorithm<T> algorithm, bool jsonNeeded = false)
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

        public string StatsFileName(int? frame = null)
        {
            return Path.Combine(Dir, $"{FilePrefix}{FrameToString(frame)}.json");
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
            if (Bundle != null)
            {
                if (Data.IsRunning)
                {
                    Data.IsRunning = false;
                    Data.RunningTask.Wait();
                }
                if (_jsonNeeded)
                {
                    SaveJson(Data.Frame);
                }
                if (Data.Colors != null)
                {
                    var fileName = GetFileName(Data.Colors, FrameToString(Data.Frame));
                    if (!File.Exists(fileName))
                    {
                        // for meridian maps or when !IntegrationEndless
                        SaveBitmap(Data.Frame);
                    }
                    OpenPicture(fileName);
                }
            }
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
                    foreach (var pair in algorithm.Poles)
                    {
                        if (pair.Value.Axis != Basin3.Oz && frame >= pair.Key)
                        {
                            var line = (int)Math.Max(0, point.X + pair.Key);

                            if (line < Bmp.Width)
                                for (var y = -10; y < 0; y++)
                                    Bmp.SetPixel(
                                        line,
                                        (YResolution * Scale * HealpixManager.Nside) + y,
                                        pair.Value.GravityFirstUse ? Color.Red : Color.Black);
                        }
                    }
            }
            SaveBitmap(Bmp, Data.Colors, FrameToString(frame));
        }

        protected void ShiftAxis(
            int framesCount = 200, /* for k6 */
            Func<int, int> slowFrames = null,
            Func<int, int> timeStepByFrame = null)
        {
            var algorithm = Bundle.Algorithm as ShiftAxis;
            var lastTimeOfSaveJson = algorithm.DataAbstract.Time;
            algorithm.Shift(
                framesCount,
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
                slowFrames,
                timeStepByFrame);
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