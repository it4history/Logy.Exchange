using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Logy.Maps.Approximations;
using Logy.Maps.Coloring;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Water
{
    public abstract class WaterMoving<T> : DataEarth2014<T> where T : HealCoor
    {
        private readonly T[] _basins;
        private bool? _isDynamicScale;
        private double _oceanVolume;

        protected WaterMoving() { }
        protected WaterMoving(
            HealpixManager man,
            T[] basins,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false) : base(min, max, readAllAtStart)
        {
            K = man.K;
            _basins = basins;
        }

        public bool WithRelief { get; set; }

        [IgnoreDataMember]
        public Action<T> GetHeightsExternal { get; set; }

        public bool Spheric { get; set; }

        [IgnoreDataMember]
        public WaterModel Water { get; set; }

        [IgnoreDataMember]
        public bool IsDynamicScale
        {
            get { return _isDynamicScale ?? (!MinDefault.HasValue && !MaxDefault.HasValue); }
            set { _isDynamicScale = value; }
        }

        /// <summary>
        /// maybe value decreases but not increases
        /// </summary>
        [IgnoreDataMember]
        public bool IntegrationEndless { get; set; }

        [IgnoreDataMember]
        public bool SamePolesAndEquatorGravitation { get; set; }

        [IgnoreDataMember]
        public bool IsRunning { get; set; }

        [IgnoreDataMember]
        public Task RunningTask { get; set; }
        public int Frame { get; set; }
        public int Time { get; set; }
        public int TimeStep { get; set; } = 1;
        public double? Max { get; set; }
        public double? Min { get; set; }

        protected internal PixelsManager<T> PixMan { get; private set; }

        public override void Init()
        {
            base.Init();
            PixMan = new PixelsManager<T>(HealpixManager, _basins);
            Water = new WaterModel(HealpixManager);
        }

        public void DoFrame(bool isDynamicScale = true)
        {
            GradientAndHeightCrosses();
            
            // todo calculate Max..Min only for time that will be drawn
            MoveAllWater(isDynamicScale);
        }

        /// <param name="action">parameter is frame counted from 0</param>
        /// <param name="width">if (width == 1) then 2 frames: -1 and 0</param>
        public void DoFrames(Func<int, int> action, int? width = null)
        {
            IsRunning = true;
            var w = width ?? (HealpixManager.Nside * 2);
            RunningTask = Task.Run(() =>
            {
                for (; Frame < w && IsRunning; Frame++)
                {
                    // viscosity = i < 0 ? .6 : .4;// .6 - .2 * i / (2 * w);
                    for (var step = 0; step < TimeStep; step++)
                    {
                        DoFrame(IsDynamicScale);
                    }
                    Time += TimeStep;
                    SetScales();
                    TimeStep = action(Frame);

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
                }
            });
            RunningTask.Wait();
        }

        public override void Draw(
            Bitmap bmp, 
            double deltaX = 0, 
            IEnumerable basins = null,
            int yResolution = 2, 
            int scale = 1)
        {
            base.Draw(bmp, deltaX, PixMan.Pixels, yResolution, scale);
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
                oceanVolume += (basin as BasinBase).Volume;
            }
            return ToMilCumKm(oceanVolume);
        }

        public void CheckOcean()
        {
            _oceanVolume = GetOceanVolume();
            Assert.GreaterOrEqual(_oceanVolume, 1330);
            Assert.LessOrEqual(_oceanVolume, 1340);
        }

        public double RecheckOcean()
        {
            var newOceanVolume = GetOceanVolume();
            var diff = newOceanVolume - _oceanVolume;
            Console.WriteLine("initial ocean: {0:.##}; increased on: {1}", _oceanVolume, diff);
            return diff;
        }

        /// <summary>
        /// пересечения градиента с радиусами (высотами тазиков)
        /// http://hist.tk/ory/Gradient_and_height_crosses
        /// </summary>
        internal virtual void GradientAndHeightCrosses()
        {
        }

        /// <param name="isDynamicScale">whether recalculate min, max and therefore possibly make scale dynamic</param>
        internal void MoveAllWater(bool isDynamicScale = true)
        {
            if (isDynamicScale || Colors == null)
            {
                InitAltitudes(PixMan.Pixels);
            }
            else
                foreach (var basin in PixMan.Pixels)
                {
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

        private void SetScales()
        {
            Colors.SetScales(
                new SortedList<int, Color3>
                {
                    { 0, ColorsManager.WaterBorder },
                    { 25, new Color3(Color.Yellow) },
                    { 50, new Color3(Color.SandyBrown) },
                    { 100, new Color3(Color.Red) },
                },
                new SortedList<int, Color3>
                {
                    { 0, ColorsManager.WaterBorder },
                    { 100, ColorsManager.DarkBlue },
                });
        }
    }
}