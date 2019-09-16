using System;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.Projections
{
    public class Equirectangular
    {
        private const double BorderWidth = .1; 

        private readonly double _resx;
        private readonly double _resy;
        private readonly double _width;

        /// <summary>
        /// x from 0 to 4 * man.Nside
        /// y from 0 to YResolution * man.Nside
        /// </summary>
        public Equirectangular(HealpixManager man, int yResolution = 2)
        {
            var collapse1PixelWidth = 4 * man.Nside / ((4 * man.Nside) + 0 /*shrinking on 0 pixel at width*/ + BorderWidth);
            _resx = man.Nside / 90.0 * collapse1PixelWidth;
            _resy = _resx * yResolution / 2;
        }

        /// <param name="accuracyMin">in arc-min</param>
        public Equirectangular(int accuracyMin)
        {
            var r = 60d / accuracyMin;
            _width = 21600d / accuracyMin;
            var height = 10800d / accuracyMin;
            _resx = ((_width - 1) / _width) * r;
            _resy = ((height - 1) / height) * r;
        }

        public double X(double x)
        {
            return (x + 180) * _resx;
        }
        public double Y(double y)
        {
            return (-y + 90) * _resy;
        }

        public int FullOffset(Coor coor)
        {
            return (int)Math.Round(
                (/*rounding is important*/Math.Round(Y(coor.Y), MidpointRounding.AwayFromZero) * _width)
                + X(coor.X), 
                MidpointRounding.AwayFromZero);
        }

        public Point2 Offset(Coor coor)
        {
            return new Point2(
                (int)Math.Round(X(coor.X), MidpointRounding.AwayFromZero),
                (int)Math.Round(Y(coor.Y), MidpointRounding.AwayFromZero));
        }

        internal static Coor CoorFromXY(Point2 p, int yResolution, HealpixManager man, int frame = 0)
        {
            var lat = 90d - ((p.Y + BorderWidth) * 180d / (yResolution * man.Nside));
            var lon = ((p.X + BorderWidth) * 360 / (4 * man.Nside)) - (180 - frame);
            return new Coor(lon, lat).Normalize<Coor>();
        }
    }
}