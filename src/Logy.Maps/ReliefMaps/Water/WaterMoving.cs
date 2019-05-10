using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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

        protected internal readonly PixelsManager<T> PixMan;

        public WaterModel Water;

        protected WaterMoving(HealpixManager man, T[] basins,
            double? min = null, double? max = null, bool readAllAtStart = false) : base(man, min, max, readAllAtStart)
        {
            PixMan = new PixelsManager<T>(man, basins);
            Water = new WaterModel(man);
        }

        public bool IsDynamicScale
        {
            get { return _isDynamicScale ?? (!MinDefault.HasValue && !MaxDefault.HasValue); }
            set { _isDynamicScale = value; }
        }
        
        /// <summary>
        /// maybe value decreases but not increases
        /// </summary>
        public bool IntegrationEndless { get; set; }

        public bool SamePolesAndEquatorGravitation { get; set; }

        /// <summary>
        /// пересечения градиента с радиусами (высотами тазиков)
        /// http://hist.tk/hw/Gradient_and_height_crosses
        /// </summary>
        internal virtual void GradientAndHeightCrosses()
        {
        }

        /// <param name="baseCall">whether recalculate min, max and therefore possibly make scale dynamic</param>
        internal void MoveAllWater(bool baseCall = true)
        {
            if (baseCall || Colors == null)
            {
                InitAltitudes(PixMan.Pixels);
            }
            else
                foreach (var basin in PixMan.Pixels)
                {
                    basin.Altitude = GetAltitude(basin);
                }
        }

        public void Cycle(bool baseCall = true)
        {
            GradientAndHeightCrosses();
            //CalcDeltasH();
            MoveAllWater(baseCall);
        }

        protected virtual void CalcDeltasH()
        {
        }

        /// <param name="action"></param>
        /// <param name="width">if 1 then 2 cycles: -1 and 0</param>
        public void Cycle(Func<int, int> action, int? width = null)
        {
            double? previousMax = null;
            double? previousMin = null;
            var w = width ?? HealpixManager.Nside;
            for (var step = -w; step < w; step++)
            {
                SetScales();
                int timeKoef = action(step + w);

                //viscosity = i < 0 ? .6 : .4;// .6 - .2 * i / (2 * w);
                for (var cycle = 0; cycle < timeKoef; cycle++)
                {
                    Cycle(IsDynamicScale);
                }

                if (IsDynamicScale)
                {
                    if (previousMin.HasValue && previousMax.HasValue)
                        if (Colors.Min < previousMin || Colors.Max > previousMax)
                        {
                            if (Colors.Max < previousMax)
                            {
                                Console.WriteLine("wag detected");
                            }
                            //Console.WriteLine("Max was {1:#.}, now {2:.#}, min {3:.#}", 0, previousMax, Colors.Max, Colors.Min);
                        }
                        else
                        {
                            if (!IntegrationEndless)
                                if (!MinDefault.HasValue && !MaxDefault.HasValue) /*not IsDynamicScale*/
                                {
                                    Console.WriteLine("integration finished at step {0}", step);
                                    break;
                                }
                        }
                    previousMax = Colors.Max;
                    previousMin = Colors.Min;
                }
            }
        }

        private void SetScales()
        {
            if (Colors != null)
                Colors.SetScales(
                    new SortedList<int, Color3>
                    {
                        { 0, ColorsManager.WaterBorder },
                        { 25, new Color3(Color.Yellow) },
                        { 50, new Color3(Color.SandyBrown) },
                        { 100, new Color3(Color.Red) },
                    }, new SortedList<int, Color3>
                    {
                        { 0, ColorsManager.WaterBorder },
                        { 100, ColorsManager.DarkBlue },
                    });
        }

        public override void Draw(Bitmap bmp, double deltaX = 0, IEnumerable basins = null, 
            int yResolution = 2, int scale = 1)
        {
            base.Draw(bmp, deltaX, PixMan.Pixels, yResolution, scale);
            Log();
        }

        public override void Log()
        {
            if (!MinDefault.HasValue || !MaxDefault.HasValue)
                base.Log();
        }

        /// <returns>millions of cubic km</returns>
        protected double ToMilCumKm(double oceanVolume)
        {
            return oceanVolume / (1000000000 * 1000000d);
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
        private double _oceanVolume;
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
            Console.WriteLine("initial ocean: {0:.##}; increased on: {1}",
                _oceanVolume, diff);
            return diff;
        }
    }
}