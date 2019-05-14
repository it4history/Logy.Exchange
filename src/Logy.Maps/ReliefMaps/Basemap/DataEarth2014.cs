using System;
using System.Collections;
using System.Drawing;
using System.Runtime.Serialization;
using Logy.Maps.Coloring;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public abstract class DataEarth2014<T> : DataEarth, IDisposable where T : HealCoor
    {
        public readonly HealpixManager HealpixManager;

        /// <summary>
        /// value that treated as splitter for colors
        /// if null then is (max-min)/2
        /// </summary>
        [IgnoreDataMember]
        public double? ColorsMiddle;

        [IgnoreDataMember]
        public double? MaxDefault;
        protected double? MinDefault;

        protected readonly Earth2014Manager

            // physical surface 
            Relief,

            // relief without water and ice masses 
            ReliefBed;

        protected DataEarth2014(HealpixManager man, double? min = null, double? max = null, bool readAllAtStart = false)
        {
            HealpixManager = man;
            MinDefault = min;
            MaxDefault = max;
            Relief = new Earth2014Manager(ReliefType, Accuracy, IsReliefShape, readAllAtStart);
            ReliefBed = new Earth2014Manager(ReliefBedType, Accuracy, IsReliefBedShape, readAllAtStart);
        }

        /// <summary>
        /// also sets Colors
        /// </summary>
        public ColorsManager InitAltitudes(T[] basins, bool isGrey = false)
        {
            double? min = MinDefault, max = MaxDefault;
            foreach (var basin in basins)
            {
                var altitude = GetAltitude(basin);
                basin.Altitude = altitude; // not needed for Projection.Equirectangular
                if (altitude.HasValue)
                {
                    CheckMaxMin(altitude.Value, ref min, ref max);
                }
            }
            Colors = new ColorsManager(min.Value, max.Value, ColorsMiddle, isGrey);
            return Colors;
        }

        public abstract double? GetAltitude(T basin);

        public int GetHeights(HealCoor coor, int rOfEllipse, out int waterHeight)
        {
            var surface = Relief.GetAltitude(coor);
            var hOQ = surface;
            if (IsReliefShape)
                // hOQ includes undulation 
                hOQ += Earth2014Manager.Radius2Add - rOfEllipse;

            var bed = ReliefBed.GetAltitude(coor);

            waterHeight = surface - bed;
            if (waterHeight > 0) /* lakes in ice are ignored */
            {
            }
            return hOQ;
        }

        public virtual void Draw(
            Bitmap bmp, 
            double deltaX = 0, 
            IEnumerable basins = null, 
            int yResolution = 2, 
            int scale = 1)
        {
            if (basins != null && Colors != null)
                foreach (var pixel in basins)
                {
                    var healCoor = (HealCoor)pixel;
                    var eqProjection = new Equirectangular(HealpixManager, yResolution);
                    var point = eqProjection.Offset(healCoor);
                    Colors.SetPixelOnBmp(
                        healCoor.Altitude, 
                        bmp,
                        (int)(point.X + deltaX), 
                        (int)point.Y, scale);
                }
        }

        public virtual int Accuracy => 5;

        /// <summary>
        /// mainly not ReliefType.Bed
        /// </summary>
        [IgnoreDataMember]
        public virtual ReliefType ReliefType => ReliefType.Sur;

        [IgnoreDataMember]
        public virtual ReliefType ReliefBedType => ReliefType.Bed;

        protected virtual bool IsReliefShape => false;

        protected virtual bool IsReliefBedShape => IsReliefShape;

        public void Dispose()
        {
            Relief.Dispose();
            ReliefBed.Dispose();
        }

        public virtual void Log()
        {
            if (Colors != null)
                Console.WriteLine(
                    "{0:0.#}..{1:0.#}", ////; {2}",
                    Colors.Min, 
                    Colors.Max, 
                    GetType().Name);
        }

        public void CheckMaxMin(double? altitude)
        {
            double? min = Colors.Min, max = Colors.Max;
            CheckMaxMin(altitude.Value, ref min, ref max);
            Colors.Min = min.Value;
            Colors.Max = max.Value;
        }

        private void CheckMaxMin(double altitude, ref double? min, ref double? max)
        {
            if (min == null || altitude < min)
                min = altitude;
            if (max == null || altitude > max)
                max = altitude;
        }
    }
}