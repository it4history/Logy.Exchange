using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
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
    }

    public class PoleSearchByLatitude : Map2DBase<City>
    {
        public PoleSearchByLatitude() : base(7)
        {
            LegendNeeded = false;
        }

        public override Projection Projection => Projection.Healpix;

        protected override ImageFormat ImageFormat => ImageFormat.Png;

        protected override DataForMap2D<City> MapData => new PoleSearchData(this);
    }

    public class PoleSearchData : DataForMap2D<City>
    {
        // from https://chispa1707.livejournal.com/3235906.html
        private readonly City[] _cities =
        {
            new City("Житомир", 28.66, 50.25, 48.42), // старая широта 48° 25'
            new City("Яссы", 27.59, 47.17, 45.83), // старая широта 45°50'
            new City("Псков", 28.34, 57.82, 54), // старая широта 54°
            new City("Рига", 24.11, 56.95, 53.58, true), // старая широта 53°35'
        };

        public PoleSearchData(Map2DBase<City> map) : base(map)
        {
        }

        public override double? GetAltitude(City basin)
        {
            var width = .005;
            var found = 0;
            int? foundI = null;
            for (var i = 0; i < _cities.Length; i++)
            {
                var city = _cities[i];
                var radianToPixel = city.DistanceTo(basin);
                var toOldPoleRadian = ((90 - city.Y) / 180) * Math.PI;
                if (radianToPixel < width)
                {
                    city.Basin = basin;
                    return i;
                }

                if (radianToPixel >= toOldPoleRadian - width
                    && radianToPixel <= toOldPoleRadian + width)
                {
                    found++;
                    foundI = i;
                }
            }
            return found == _cities.Length ? 10 : foundI;
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
            var font = new Font("Tahoma", 10);
            var equirectangular = new Equirectangular(HealpixManager, yResolution);
            for (var i = 0; i < _cities.Length; i++)
            {
                var city = _cities[i];
                var point = equirectangular.OffsetDouble(city.Basin, scale);
                var measure0 = g.MeasureString(city.Name, font);
                var left = (city.Lefted ? -(int)measure0.Width : 5) + (int)point.X;
                var top = (int)point.Y - (int)measure0.Height / 2; /// Map.Top + 15 * (i - _cities.Length - 1)
                g.DrawString(city.Name, font, new SolidBrush((Color)Colors.Get(i)), left, top);
            }
            g.Flush();

            
        }
    }
}