using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.ServiceModel.Configuration;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.Tests
{
    public class City : HealCoor
    {
        public City()
        {
        }

        public City(string name, double x, double y, double oldY, bool lefted = false) : base(x, y)
        {
            OldLatitudeDegree = oldY;
            Name = name;
            Lefted = lefted;
        }

        public double OldLatitudeDegree { get; set; }

        public string Name { get; set; }
        public bool Lefted { get; set; }
        public City Basin { get; set; }
        public string Except { get; set; }

        public override string ToString()
        {
            return base.ToString() + (string.IsNullOrEmpty(Except) ? null : " except: " + Except);
        }
    }

    public class PoleSearchByLatitude : Map2DBase<City>
    {
        public PoleSearchByLatitude() : base(8)
        {
            LegendNeeded = false;
            YResolution = 3;
        }

        protected override ImageFormat ImageFormat => ImageFormat.Tiff;

        protected override DataForMap2D<City> MapData => new PoleSearchData(this);
    }

    public class PoleSearchData : DataForMap2D<City>
    {
        private const bool CurrentPoleDemo = false;
        private const int AllowedExceptionCitiesCount = CurrentPoleDemo ? 0 : 0;

        // from http://hist.tk/ory/Карта_театра_войны_Российской_империи_против_Франции_и_её_союзников,_1813
        private readonly List<City> _cities = new List<City>
        {
            new City("Житомир", 28.66, 50.25, 48.43), // старая широта 48° 25' (48.42)
            new City("Овруч", 28.81, 51.32, 49.2, true), // 49.12
            new City("Яссы", 27.59, 47.17, 45.99), // старая широта 45°50' (45.83)

            new City("Псков", 28.34, 57.82, 54.3), // старая широта 54° (54)
            new City("Полоцк", 28.78, 55.49, 52.3), // 52.15
            new City("Рига", 24.11, 56.95, 54.9, true), // старая широта 53°35' (53.58)

            new City("Вильнюс", 25.28, 54.68, 52.8, true), // 51.5
            new City("Минск", 27.55, 53.92, 51.4), // 50.91
            // new City("Париж", 2.35, 48.86, 48.8),


            //new City("Москва", 37.62, 55.76, 50),
            //new City("Питер", 30.32, 59.95, 55),
        };

        private readonly List<City> _poles = new List<City>();

        public PoleSearchData(Map2DBase<City> map) : base(map)
        {
        }

        public override double? GetAltitude(City basin)
        {
            var accuracyError = HealpixManager.ThetaPix;
            if (CurrentPoleDemo)
                accuracyError *= 2;
            var found = new List<City>();
            foreach (var city in _cities)
            {
                var radianToPixel = city.DistanceTo(basin);
                if (radianToPixel < (K <= 7 ? .007 : .005))
                {
                    city.Basin = basin;
                    return ColorIndex(city);
                }

                var toOldPoleRadian = ((90 - (CurrentPoleDemo ? city.Y : city.OldLatitudeDegree)) / 180) * Math.PI;
                if (radianToPixel >= toOldPoleRadian - accuracyError
                    && radianToPixel <= toOldPoleRadian + accuracyError)
                {
                    found.Add(city);
                }
            }
            var isOldPole = found.Count >= _cities.Count - AllowedExceptionCitiesCount;
            if (isOldPole)
            {
                if (found.Count < _cities.Count)
                    basin.Except = (from c in _cities where !found.Contains(c) select c.Name)
                        .Aggregate((s, prop) => s + ", " + prop);
                _poles.Add(basin);
            }
            return isOldPole ? -1 : (found.Count == 0 ? (int?)null : ColorIndex(found.Last()));
        }

        public override void Draw(
            Bitmap bmp,
            double deltaX = 0,
            IEnumerable basins = null,
            int yResolution = 2,
            int scale = 1,
            Projection projection = Projection.Healpix)
        {
            Colors.DefaultColor = Color.FromArgb(228, 251, 232);
            base.Draw(bmp, deltaX, basins, yResolution, scale, projection);
            Map2DBase<HealCoor>.DrawPoliticalMap(bmp, HealpixManager, yResolution, scale);
            var g = Map2DBase<HealCoor>.GetFont(bmp);
            var font = new Font("Tahoma", K + 1);
            var equirectangular = new Equirectangular(HealpixManager, yResolution);
            foreach (var city in _cities)
            {
                var point = equirectangular.OffsetDouble(city.Basin, scale);
                var measure0 = g.MeasureString(city.Name, font);
                var left = (city.Lefted ? -(int)measure0.Width : 5) + (int)point.X;
                var top = (int)point.Y - (int)measure0.Height / 2; /// Map.Top + 15 * (i - _cities.Length - 1)
                g.DrawString(city.Name, font, new SolidBrush((Color)Colors.Get(ColorIndex(city))), left, top);
            }
            g.Flush();
        }

        public override void Log()
        {
            base.Log();
            foreach (var pole in _poles)
            {
                Console.WriteLine("pole: {0}", pole);
            }
        }

        private int ColorIndex(City city)
        {
            return _cities.IndexOf(city) + 1;
        }
    }
}