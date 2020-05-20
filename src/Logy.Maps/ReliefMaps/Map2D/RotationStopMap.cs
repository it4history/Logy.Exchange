using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Logy.Maps.Exchange;
using Logy.Maps.Metrics;
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
        public const string FilePrefix = "stats";

        public RotationStopMap() : this(6)
        {
        }
        public RotationStopMap(int k, LegendType legendType = LegendType.Horizontal) : base(k, legendType)
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
                    /* danger
                    if (Directory.Exists(Dir))
                        Array.ForEach(Directory.GetFiles(Dir), File.Delete); */
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

        protected bool JsonNeeded { private get; set; }

        protected bool WithPoliticalMap { get; set; }

        public static string FindJson(string dir)
        {
            return Directory.GetFiles(dir, "*" + RotationStopMap<BasinAbstract>.FilePrefix + "*.json")
                .LastOrDefault();
            /// throw new ApplicationException("needed json at " + dir);
        }

        /// <param name="json2loadFrame">null to load latest json, 0 to start new</param>
        /// <param name="algorithm">null to get algo from json</param>
        public void InitDataWithJson(int? json2loadFrame = null, Algorithm<T> algorithm = null)
        {
            InitData(
                algorithm,
                true,
                json2loadFrame == null ? RotationStopMap<Basin3>.FindJson(Dir) : StatsFileName(json2loadFrame));
        }

        /// <summary>
        /// like a constructor
        /// </summary>
        public void InitData(Algorithm<T> algorithm, bool jsonNeeded = false, string jsonfileToLoad = null)
        {
            JsonNeeded = jsonNeeded;
            if (jsonfileToLoad == null)
                jsonfileToLoad = StatsFileName();
            if (File.Exists(jsonfileToLoad))
            {
                Bundle = Bundle<T>.DeserializeFile(jsonfileToLoad);
                var dataInJson = Bundle.Algorithm.DataAbstract;
                if (K != dataInJson.K)
                    throw new ApplicationException($"datum {jsonfileToLoad} needs K {K}");

                var dataInCode = algorithm?.DataAbstract;
                if (dataInCode != null)
                {
                    if (dataInCode.WithRelief != dataInJson.WithRelief)
                        throw new ApplicationException("data WithRelief mismatch");
                }

                var algo = Bundle.Algorithm as ShiftAxis;
                if (algo != null && algo.Geoisostasy)
                    algo.SetGeoisostasyDatum(algo);
            }
            else
            {
                algorithm.DataAbstract.Init();
                Bundle = new Bundle<T>(algorithm);
            }
        }

        public string StatsFileName(object frame = null)
        {
            if (frame is int)
                frame = FrameToString((int)frame);
            return Path.Combine(Dir, $"{FilePrefix}{frame}.json");
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
                if (JsonNeeded)
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

        protected void SaveBitmap(int frame, string suffix = null)
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
                        if (pair.Value.Axis != BasinAbstract.Oz && frame >= pair.Key)
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
            SaveBitmap(Bmp, Data.Colors, FrameToString(frame) + suffix);
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
                    if (WithPoliticalMap)
                        PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
                    SaveBitmap(algorithm.DataAbstract.Frame);
                    var time = algorithm.DataAbstract.Time;
                    if (JsonNeeded && time > lastTimeOfSaveJson + 5000)
                    {
                        SaveJson(frame);
                        lastTimeOfSaveJson = time;
                    }
                },
                slowFrames,
                timeStepByFrame);
        }

        /// <summary>
        /// makes http://hist.tk/ory/Искажение_начала_перетекания visible
        /// </summary>
        /// <param name="risky"></param>
        protected void HighFluidity(bool risky = false)
        {
            Data.Water.Fluidity = risky ? 1 : .95;
            if (Data.MetricType == MetricType.RadiusIntersection)
                Data.Water.Fluidity *= .5;

            Data.Water.Threshhold *= risky ? .01 : .1; /// for k6 standard Threshhold is 2.4m
        }

        protected void DrawPoliticalMap()
        {
            Data.CalcAltitudes(false); // Data.InitAltitudes(Data.PixMan.Pixels, this);
            Data.SetColorLists();
            Draw();
            PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame, "political");
        }

        private void SaveJson(int? frame = null)
        {
            Bundle.Algorithm.Diff = Data.RecheckOcean();
            if (!frame.HasValue || !Bundle.Deserialized.ContainsKey(frame.Value))
                using (var file = File.CreateText(StatsFileName(frame)))
                {
                    new JsonSerializer().Serialize(file, Bundle);
                }
        }
    }
}