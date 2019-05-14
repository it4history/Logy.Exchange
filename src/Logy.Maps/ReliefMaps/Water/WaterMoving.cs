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
        private bool? _isDynamicScale;
        private double _oceanVolume;

        protected WaterMoving(
            HealpixManager man,
            T[] basins,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false) : base(man, min, max, readAllAtStart)
        {
            PixMan = new PixelsManager<T>(man, basins);
            Water = new WaterModel(man);
        }

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
        public int Frame { get; set; } = -1;
        public int Time { get; set; } = -1;

        protected internal PixelsManager<T> PixMan { get; }

        public void DoFrame(bool isDynamicScale = true)
        {
            GradientAndHeightCrosses();
            MoveAllWater(isDynamicScale);
        }

        /// <param name="action">parameter is frame counted from 0</param>
        /// <param name="width">if (width == 1) then 2 frames: -1 and 0</param>
        public void DoFrames(Func<int, int> action, int? width = null)
        {
            double? previousMax = null;
            double? previousMin = null;
            Time = 0;
            IsRunning = true;
            var w = width ?? (HealpixManager.Nside * 2);
            RunningTask = Task.Run(() =>
            {
                for (Frame = 0; Frame < w && IsRunning; Frame++)
                {
                    SetScales();
                    int timeKoef = action(Frame);

                    // viscosity = i < 0 ? .6 : .4;// .6 - .2 * i / (2 * w);
                    for (var step = 0; step < timeKoef; step++)
                    {
                        DoFrame(IsDynamicScale);
                    }
                    Time += timeKoef;

                    if (IsDynamicScale)
                    {
                        if (previousMin.HasValue && previousMax.HasValue)
                            if (Colors.Min < previousMin || Colors.Max > previousMax)
                            {
                                if (Colors.Max < previousMax)
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
                        previousMax = Colors.Max;
                        previousMin = Colors.Min;
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
        /// http://hist.tk/hw/Gradient_and_height_crosses
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
            Colors?.SetScales(
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