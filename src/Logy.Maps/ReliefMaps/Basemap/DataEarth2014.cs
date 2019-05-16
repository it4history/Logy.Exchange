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
        private readonly bool _readAllAtStart;
        private int _k;
        private HealpixManager _healpixManager;

        protected DataEarth2014()
        {
        }

        protected DataEarth2014(double? min = null, double? max = null, bool readAllAtStart = false)
        {
            MinDefault = min;
            MaxDefault = max;
            _readAllAtStart = readAllAtStart;
        }

        public int K
        {
            get
            {
                return _k;
            }
            set /*dangerous, used for deserialization*/
            {
                _k = value;
                _healpixManager = null;
            }
        }

        [IgnoreDataMember]
        public HealpixManager HealpixManager 
            => _healpixManager ?? (_healpixManager = new HealpixManager(K));

        public virtual int Accuracy => 5;

        /// <summary>
        /// mainly not ReliefType.Bed
        /// </summary>
        [IgnoreDataMember]
        public virtual ReliefType ReliefType => ReliefType.Sur;

        [IgnoreDataMember]
        public virtual ReliefType ReliefBedType => ReliefType.Bed;

        /// <summary>
        /// value that treated as splitter for colors
        /// if null then is (max-min)/2
        /// </summary>
        [IgnoreDataMember]
        public double? ColorsMiddle { get; set; }

        [IgnoreDataMember]
        public double? MaxDefault { get; set; }
        protected double? MinDefault { get; set; }

        // physical surface 
        protected Earth2014Manager Relief { get; private set; }

        // relief without water and ice masses 
        protected Earth2014Manager ReliefBed { get; private set; }

        protected virtual bool IsReliefShape => false;

        protected virtual bool IsReliefBedShape => IsReliefShape;

        public virtual void OnInit()
        {
            Relief = new Earth2014Manager(ReliefType, Accuracy, IsReliefShape, _readAllAtStart);
            ReliefBed = new Earth2014Manager(ReliefBedType, Accuracy, IsReliefBedShape, _readAllAtStart);
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

        public int GetHeights(HealCoor coor, int radiusOfEllipse, out int waterHeight)
        {
            var surface = Relief.GetAltitude(coor);
            var hOQ = surface;
            if (IsReliefShape)
            {
                // hOQ includes undulation 
                hOQ += Earth2014Manager.Radius2Add - radiusOfEllipse;
            }

            var bed = ReliefBed.GetAltitude(coor);

            waterHeight = surface - bed;
            if (waterHeight > 0) 
            {
                /* lakes in ice are ignored */
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
                    var projection = new Equirectangular(HealpixManager, yResolution);
                    var point = projection.Offset(healCoor);
                    Colors.SetPixelOnBmp(
                        healCoor.Altitude,
                        bmp,
                        (int)(point.X + deltaX),
                        (int)point.Y, 
                        scale);
                }
        }

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