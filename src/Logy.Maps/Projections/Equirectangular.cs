using System;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.DotNetWikiBot.Sphere;
using Logy.MwAgent.DotNetWikiBot.Wikidata;

namespace Logy.Maps.Projections
{
    public class Equirectangular
    {
        private readonly double _resx;
        private readonly double _resy;
        public readonly double _width;

        private const double BorderWidth = 0;//why was .5 ?

        /// <summary>
        /// x from 0 to 4 * man.Nside
        /// y from 0 to YResolution * man.Nside
        /// </summary>
        public Equirectangular(HealpixManager man, int yResolution = 2)
        {
            var collapse1PixelWidth =  4 * man.Nside / (4 * man.Nside + 1 /*shrinking on one pixel at width*/ + BorderWidth);
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
                /*rounging is important*/Math.Round(Y(coor.Y)) * _width 
                + X(coor.X));
        }

        public Point2 Offset(Coor coor)
        {
            return new Point2(
                (int) Math.Round(X(coor.X)),
                (int) Math.Round(Y(coor.Y))
            );
        }

        internal static Coor CoorFromXY(Point2 p, int yResolution, HealpixManager man, int step = 0)
        {
            var lat = 90d - (p.Y + BorderWidth) * 180d / (yResolution * man.Nside);
            var lon = (p.X + BorderWidth) * 360 / (4 * man.Nside) - (180 - step);
            var coor = (Coor)new Coor { Y = lat, X = lon }.Normalize();
            return coor;
        }
    }
}