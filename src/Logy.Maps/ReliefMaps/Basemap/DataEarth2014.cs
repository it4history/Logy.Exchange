using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Logy.Maps.Coloring;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.MwAgent.Sphere;
using Newtonsoft.Json;

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

        protected DataEarth2014(T[] basins, double? min = null, double? max = null, bool readAllAtStart = false)
        {
            InitialBasins = basins;
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

        public int Accuracy { get; set; } = 5;

        /// <summary>
        /// mainly not ReliefType.Bed
        /// </summary>
        [IgnoreDataMember]
        public virtual ReliefType ReliefType => ReliefType.Sur;

        [IgnoreDataMember]
        public virtual ReliefType ReliefBedType => ReliefType.Bed;

        /// <summary>
        /// not needed for Projection.Equirectangular
        /// </summary>
        [JsonIgnore]
        public PixelsManager<T> PixMan { get; private set; }

        /// <summary>
        /// are packed into PixMan in Init()
        /// </summary>
        public T[] InitialBasins { private get; set; }

        [IgnoreDataMember]
        public Action<HealCoor, Bitmap, Point2, int> PoleDraw { get; set; }

        /// <summary>
        /// value that treated as splitter for colors
        /// if null then is (max-min)/2
        /// </summary>
        [IgnoreDataMember]
        public double? ColorsMiddle { get; set; }

        [IgnoreDataMember]
        public double? MaxDefault { get; set; }
        internal double? MinDefault { get; set; }

        // physical surface 
        protected Earth2014Manager Relief { get; private set; }

        // relief without water and ice masses 
        protected Earth2014Manager ReliefBed { get; private set; }

        protected virtual bool IsReliefShape => false;

        protected virtual bool IsReliefBedShape => IsReliefShape;

        #region init methods that may be protected
        /// <summary>
        /// moved out constructor to enable deserialization
        /// </summary>
        public void Init(Map2DBase<T> map = null)
        {
            PixMan = new PixelsManager<T>(HealpixManager, InitialBasins);
            if (map?.Projection != Projection.NoHealpix)
            {
                Relief = new Earth2014Manager(ReliefType, Accuracy, IsReliefShape, _readAllAtStart);
                ReliefBed = new Earth2014Manager(ReliefBedType, Accuracy, IsReliefBedShape, _readAllAtStart);
            }
        }

        /// <summary>
        /// also sets Colors
        /// </summary>
        public ColorsManager InitAltitudes(T[] basins, Map map = null)
        {
            double? min = MinDefault, max = MaxDefault;
            foreach (var basin in basins)
            {
                basin.Altitude = null;
                var altitude = GetAltitude(basin);
                if (basin.Altitude == null)
                    basin.Altitude = altitude; // not needed for Projection.Equirectangular
                if (altitude.HasValue)
                {
                    CheckMaxMin(altitude.Value, ref min, ref max);
                }
            }
            Colors = new ColorsManager(min.Value, max.Value, ColorsMiddle, map);
            return Colors;
        }
        #endregion

        /// <summary>
        /// either return altitude or fill basin.Altitude
        /// </summary>
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

        /// <param name="deltaX">from 0 to 4 * HealpixManager.Nside, deltaY to YResolution * HealpixManager.Nside</param>
        public virtual void Draw(
            Bitmap bmp,
            double deltaX = 0,
            IEnumerable basins = null,
            int yResolution = 2,
            int scale = 1,
            Projection projection = Projection.Healpix)
        {
            if (basins != null && Colors != null)
            {
                Point2 previousPoint = null;
                HealCoor previousCoor = null;
                var equirectangular = new Equirectangular(HealpixManager, yResolution);
                foreach (var pixel in basins)
                {
                    var healCoor = (HealCoor)pixel;
                    /*var phi1 = 97d;
                    //healCoor.X *= Math.Cos((phi1 / 180) * Math.PI) * 180;
                    healCoor.Y -= phi1;
                    healCoor.Normalize<HealCoor>();*/
                    var point = equirectangular.Offset(healCoor.X, healCoor.Y);

                    Colors.SetPixelOnBmp(
                        healCoor.Altitude,
                        bmp,
                        (int)(point.X + deltaX),
                        (int)point.Y,
                        scale);

                    if (projection == Projection.Healpix2EquirectangularFast
                        && previousPoint != null
                        && (int)previousPoint.Y == (int)point.Y
                        && (int)previousPoint.X > (int)point.X)
                    {
                        var segments = Math.Min(previousPoint.X - point.X, 4); // 4 segments produce 3 points
                        var segmentLength = (previousPoint.X - point.X) / segments;
                        for (var i = 1; i < segments; i++)
                        {
                            var approximateAltitude = (healCoor.Altitude * (segments - i) / segments)
                                                      + (previousCoor.Altitude * i / segments);
                            Colors.SetPixelOnBmp(
                                approximateAltitude,
                                bmp,
                                (int)(point.X + deltaX + (i * segmentLength)),
                                (int)point.Y,
                                scale);
                        }
                    }
                    previousPoint = point;
                    previousCoor = healCoor;

                    PoleDraw?.Invoke(healCoor, bmp, point, scale);
                }
            }
        }

        /// <summary>
        /// not for LegentType.Hue
        /// </summary>
        public void SetColorLists()
        {
            Colors.SetColorLists(
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

        public virtual void Dispose()
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