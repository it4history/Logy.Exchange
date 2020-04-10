using System;
using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Logy.Maps.Geometry;
using Logy.Maps.Metrics;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Water
{
    public abstract class WaterMoving<T> : DataEarth2014<T> where T : HealCoor
    {
        private bool? _isDynamicScale;
        private double _oceanVolume;

        protected WaterMoving() { }
        protected WaterMoving(
            HealpixManager man,
            T[] basins,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false) : base(basins, min, max, readAllAtStart)
        {
            K = man.K;
        }

        public static double MaxOceanVolume { get; set; } = 1340;
        
        public bool WithRelief { get; set; }

        public bool Spheric { get; set; }

        /// <summary>
        /// gravitation like a sphere has, but Spheric may be equal to false
        /// </summary>
        public bool SamePolesAndEquatorGravitation { get; set; }

        public WaterModel Water { get; set; }

        [IgnoreDataMember]
        public bool WithFormattor { get; set; }

        [IgnoreDataMember]
        public bool IsDynamicScale
        {
            get { return _isDynamicScale ?? (!MinDefault.HasValue && !MaxDefault.HasValue); }
            set { _isDynamicScale = value; }
        }

        /// <summary>
        /// maybe value decreases but not increases
        /// </summary>
        /// [IgnoreDataMember] was commented!
        public bool IntegrationEndless { get; set; } = true;

        [IgnoreDataMember]
        public bool IsRunning { get; set; }

        [IgnoreDataMember]
        public Task RunningTask { get; set; }

        public MetricType MetricType { get; set; } = MetricType.Middle;

        public int Frame { get; set; } = -1;
        public int Time { get; set; }
        public int TimeStep { get; set; } = 1;
        public double? Min { get; set; }
        public double? Max { get; set; }

        /// <param name="reliefFromDb">if true then Depth, Hoq are got from db, it is slow</param>
        public virtual void Init(bool reliefFromDb = true, Datum datum = null)
        {
            base.Init();
            var wasDeserialized = Water;
            Water = new WaterModel(HealpixManager);
            if (wasDeserialized != null)
            {
                Water.Fluidity = wasDeserialized.Fluidity;
                Water.Threshhold = wasDeserialized.Threshhold;
            }
        }

        public void DoFrame(bool isDynamicScale = true)
        {
            GradientAndHeightCrosses();
            
            // todo calculate Max..Min only for time that will be drawn,  problem with Altitude
            CalcAltitudes(isDynamicScale);
        }

        /// <param name="onFrame">parameter is frame counted from 0</param>
        /// <param name="tillFrame">if (tillFrame==0) then will be one frame 0</param>
        public void DoFrames(Func<int, int> onFrame, int? tillFrame = null)
        {
            IsRunning = true;
            var w = tillFrame ?? (HealpixManager.Nside * 2);
            RunningTask = Task.Run(() =>
            {
                while (Frame < w && IsRunning)
                {
                    Frame++;

                    // viscosity = i < 0 ? .6 : .4;// .6 - .2 * i / (2 * w);
                    for (var step = 0; step < TimeStep; step++)
                    {
                        /* private static Basin3[] was;
                        if (Frame == 6)
                        {
                            var basinBad = (PixMan.Pixels[0] as Basin3);
                            var normal = basinBad.Normal;
                            var q3 = basinBad.Q3;
                            if (step == 0)
                            {
                                if (was != null)
                                {
                                    GradientAndHeightCrosses();
                                }
                            }
                            if (step == 1)
                            {
                                if (was == null)
                                {
                                    was = new Basin3[PixMan.Pixels.Length];
                                    for (var index = 0; index < was.Length; index++)
                                    {
                                        var basin = PixMan.Pixels[index] as Basin3;
                                        was[index] = new Basin3
                                        {
                                            Hoq = basin.Hoq,
                                            Altitude = basin.Altitude,
                                            Delta_g_meridian =basin.Delta_g_meridian,
                                            Delta_g_traverse = basin.Delta_g_traverse,
                                            _normal = basin.Normal,
                                            _q3 = new Point3D(basin.Q3.ToVector()),
                                            _actualQ3 = true
                                        };
                                        was[index].Hto = new double[4];
                                        basin.Hto.CopyTo(was[index].Hto, 0);
                                        was[index].MetricRays = new Ray3D[4];
                                        basin.MetricRays.CopyTo(was[index].MetricRays, 0);
                                    }
                                }
                                else
                                {
                                    for (var index = 0; index < was.Length; index++)
                                    {
                                        var basin = PixMan.Pixels[index] as Basin3;
                                        var basinWas = was[index];
                                        if (basinWas.Hoq != basin.Hoq
                                            //|| basinWas.Altitude != basin.Altitude
                                            || basinWas.Hto[0] != basin.Hto[0]
                                            || basinWas.Hto[1] != basin.Hto[1]
                                            || basinWas.Hto[2] != basin.Hto[2]
                                            || basinWas.Hto[3] != basin.Hto[3]
                                            || basinWas.Normal != basin.Normal
                                            || basinWas.Q3 != basin.Q3
                                        )
                                        {
                                        }
                                    }
                                }
                            }
                        }*/
                        DoFrame(IsDynamicScale);
                    }
                    Time += TimeStep;
                    SetColorLists();
                    if (IsDynamicScale)
                    {
                        if (Min.HasValue && Max.HasValue)
                            if (Colors.Min < Min || Colors.Max > Max)
                            {
                                if (Colors.Max < Max)
                                {
                                    Console.WriteLine("wag detected");
                                }
                                /// Console.WriteLine("Max was {1:#.}, now {2:.#}, min {3:.#}", 0, previousMax, Colors.Max, Colors.Min);
                            }
                            else
                            {
                                if (!IntegrationEndless)
                                    if (!MinDefault.HasValue && !MaxDefault.HasValue) /*not IsDynamicScale*/
                                    {
                                        Console.WriteLine("integration finished at frame {0}", Frame);
                                        break;
                                    }
                            }
                        Max = Colors.Max;
                        Min = Colors.Min;
                    }
                    TimeStep = onFrame(Frame);
                }
            });
            RunningTask.Wait();
        }

        public override void Draw(
            Bitmap bmp,
            double deltaX = 0,
            IEnumerable basins = null,
            int yResolution = 2,
            int scale = 1,
            Projection projection = Projection.Healpix)
        {
            base.Draw(bmp, deltaX, PixMan.Pixels, yResolution, scale, projection);
            Log();
        }

        public override void Log()
        {
            if (!MinDefault.HasValue || !MaxDefault.HasValue)
                base.Log();
        }

        public virtual double GetOceanVolume()
        {
            var oceanVolume = 0d;
            foreach (var basin in PixMan.Pixels)
            {
                oceanVolume += (basin as BasinAbstract).Volume;
            }
            return ToMilCumKm(oceanVolume);
        }

        public void CheckOcean()
        {
            _oceanVolume = GetOceanVolume();
            if (K > 3 /* sometimes for MetricType.Middle > 1 */)
            {
                Assert.GreaterOrEqual(_oceanVolume, 1329);
                Assert.LessOrEqual(_oceanVolume, MaxOceanVolume);
            }
        }

        public double RecheckOcean()
        {
            // todo ошибка округений возможно возникает из-за измененния RadiusOfGeoid 
            var newOceanVolume = GetOceanVolume();
            var diff = newOceanVolume - _oceanVolume;
            Console.WriteLine("initial ocean: {0:.##}; increased on: {1}", _oceanVolume, diff);
            return diff;
        }

        /// <summary>
        /// пересечения градиента с радиусами (высотами тазиков)
        /// http://hist.tk/ory/Gradient_and_height_crosses
        /// </summary>
        public virtual void GradientAndHeightCrosses()
        {
        }

        public double GetBasinHeight(Basin3 basin, int to)
        {
            var toBasin = basin.Neighbors[to];
            var from = basin.Opposites[to];
            return GetBasinHeight(basin, toBasin, to, from);
        }

        internal double GetBasinHeight(Basin3 basin, Basin3 toBasin, int to, int from)
        {
            // null for maps that only visualize
            if (toBasin == null)
                return 0;

            switch (MetricType)
            {
                case MetricType.Edge:
                    Compass sameRingCompass, sameRingCompass2;
                    var compass = NeighborManager.Compasses((Direction)to, out sameRingCompass);
                    var compass2 = NeighborManager.Compasses((Direction)from, out sameRingCompass2);
                    return (basin.Hto[(int)compass] + basin.Hto[(int)sameRingCompass]
                            - toBasin.Hto[(int)compass2] - toBasin.Hto[(int)sameRingCompass2]) * .5;
                default:
                    /* bug http://hist.tk/ory/Искажение_начала_перетекания
                     * may be fixed by balancing deltaH (of BasinAbstract.WaterIn method) relative to basin.WaterHeight in HealpixFormattor */
                    return basin.Hto[to] - toBasin.Hto[from];
            }
        }

        /// <param name="isDynamicScale">whether recalculate min, max and therefore possibly make scale dynamic</param>
        internal void CalcAltitudes(bool isDynamicScale = true)
        {
            if (isDynamicScale || Colors == null)
            {
                InitAltitudes(PixMan.Pixels);
            }
            else
                foreach (var basin in PixMan.Pixels)
                {
                    // basin.Altitude =  is needed for meridian maps
                    basin.Altitude = GetAltitude(basin);
                }
        }

        protected virtual void CalcDeltasH()
        {
        }

        /// <returns>millions of cubic km</returns>
        protected double ToMilCumKm(double oceanVolume)
        {
            return oceanVolume / (1000000000 * 1000000d);
        }
    }
}