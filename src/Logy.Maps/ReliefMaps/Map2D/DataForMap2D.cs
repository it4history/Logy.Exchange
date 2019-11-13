using System;
using System.Linq;
using Logy.Maps.Coloring;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.ReliefMaps.Map2D
{
    public abstract class DataForMap2D<T> : DataEarth2014<T> where T : HealCoor
    {
        /// <summary>
        /// bad design
        /// </summary>
        protected readonly Map2DBase<T> Map;

        protected DataForMap2D(Map2DBase<T> map) : this(map, null)
        {
        }

        protected DataForMap2D(Map2DBase<T> map, T[] basins) : base(basins)
        {
            Map = map;
            K = map.HealpixManager.K;

            // do not deserialize descendant classes 
            Init();
        }

        /// <summary>
        /// order is determined
        /// </summary>
        public Point2[] Basins()
        {
            switch (Map.Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                    return PixMan.Pixels;
                case Projection.Healpix2Equirectangular:
                case Projection.Equirectangular:
                    var height = Map.YResolution * HealpixManager.Nside;
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
            Console.WriteLine("lines: {0}", Map.YResolution * HealpixManager.Nside);
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
            switch (Map.Projection)
            {
                case Projection.Healpix:
                case Projection.Healpix2EquirectangularFast:
                case Projection.Healpix2Equirectangular: /*mean min, nax will be calculated later  */
                    colorsManager = InitAltitudes(PixMan.Pixels, isGrey);
                    break;
                case Projection.Equirectangular: /*min and max may not be precalculated*/
                    double? min = MinDefault, max = MaxDefault;
                    if (min == null || max == null)
                    {
                        // slow?
                        var pix = (from pixel in pixels
                                select (T)Activator.CreateInstance(
                                    typeof(HealCoor),
                                    Equirectangular.CoorFromXY(pixel, Map.YResolution, HealpixManager)))
                            .ToArray();
                        colorsManager = InitAltitudes(pix, isGrey);
                    }
                    break;
            }
            colorsManager.SetColorLists(Map.ColorsAbove, Map.ColorsUnder);
        }
    }
}