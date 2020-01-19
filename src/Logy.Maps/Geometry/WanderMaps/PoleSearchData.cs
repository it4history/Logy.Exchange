using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.WanderMaps
{
    public abstract class PoleSearchData : DataForMap2D<City>
    {
        private const bool CurrentPoleDemo = false;
        private const int AllowedExceptionCitiesCount = CurrentPoleDemo ? 0 : 0;

        private readonly List<City> _poles = new List<City>();

        private readonly List<City> _cities;

        protected PoleSearchData(Map2DBase<City> map, List<City> cities) : base(map)
        {
            _cities = cities;
        }

        public override double? GetAltitude(City basin)
        {
            var accuracyError = HealpixManager.ThetaPix * (CurrentPoleDemo ? 2 : 1);
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
            PoliticalMap.Draw(bmp, HealpixManager, yResolution, scale);
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