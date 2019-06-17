using System;
using System.Linq;
using Logy.Maps.Approximations;
using Logy.Maps.Coloring;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public abstract class DataForMap2D : DataEarth2014<HealCoor>
    {
        /// <summary>
        /// bad design
        /// </summary>
        private readonly Map2DBase _map;

        protected DataForMap2D(Map2DBase map)
        {
            _map = map;
            K = map.HealpixManager.K;
            ColorsMiddle = 0;

            // do not deserialize descendant classes 
            Init();
        }

        /// <summary>
        /// stores Altitudes
        /// 
        /// created for Projection.Healpix2Equirectangular and Projection.Healpix
        /// used mainly for Projection.Healpix2Equirectangular
        /// 
        /// trying not to put this field into DataEarth2014
        /// </summary>
        public ApproximationManager ApproxMan { get; set; }

        /// <summary>
        /// order is determined
        /// </summary>
        public Point2[] Basins()
        {
            switch (_map.Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                case Projection.Healpix2Equirectangular:
                    ApproxMan = new ApproximationManager(HealpixManager);
                    break;
            }

            switch (_map.Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                    return ApproxMan.Pixels;
                case Projection.Healpix2Equirectangular:
                case Projection.Equirectangular:
                    var height = _map.YResolution * HealpixManager.Nside;
                    var width = 4 * HealpixManager.Nside;
                    var points = new Point2[height * width];
                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            // todo do not store points in memory
                            points[(y * width) + x] = new Point2(x, y);
                        }
                    }
                    return points;
            }
            return null;
        }

        public override void Log()
        {
            Console.WriteLine("lines: {0}", _map.YResolution * HealpixManager.Nside);
            base.Log();
        }

        /// <summary>
        /// also sets Colors
        /// 
        /// good to override InitAltitudes(...) but seems not possible
        /// </summary>
        internal void InitPoints(Point2[] pixels, bool isGrey)
        {
            ColorsManager colorsManager = null;
            switch (_map.Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                case Projection.Healpix2Equirectangular: /*mean min, nax will be calculated later  */
                    colorsManager = InitAltitudes(ApproxMan.Pixels, isGrey);
                    break;
                case Projection.Equirectangular: /*min and max may not be precalculated*/
                    double? min = MinDefault, max = MaxDefault;
                    if (min == null || max == null)
                    {
                        // slow?
                        var pix = (from pixel in pixels
                                select new HealCoor(
                                    Equirectangular.CoorFromXY(pixel, _map.YResolution, HealpixManager)))
                            .ToArray();
                        colorsManager = InitAltitudes(pix, isGrey);
                    }
                    break;
            }
            colorsManager.SetColorLists(_map.ColorsAbove, _map.ColorsUnder);
        }
    }
}