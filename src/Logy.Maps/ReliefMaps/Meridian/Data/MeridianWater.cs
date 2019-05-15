using System;
using Logy.Maps.Projections.Healpix;

namespace Logy.Maps.ReliefMaps.Meridian.Data
{
    public class MeridianWater<T> : MeridianData<T> where T : MeridianCoor
    {
        private readonly bool _symmetricPoles = true;

        public MeridianWater(HealpixManager man, double? min = null, double? max = null) : base(man, min, max)
        {
            MaxRing = man.RingsCount;
        }

        internal MeridianWater(HealpixManager man, T[] basins, bool symmetricPoles = true) : base(man, basins)
        {
            _symmetricPoles = symmetricPoles;
            foreach (var basin in basins)
            {
                MaxRing = Math.Max(MaxRing, basin.Ring);
            }
        }

        public int MaxRing { get; }

        public bool WasWaterMoved()
        {
            foreach (var basin in PixMan.Pixels)
            {
                if (basin.Volumes[0] || basin.Volumes[1])
                {
                    return true;
                }
            }
            return false;
        }

        public override void Log()
        {
            // base.Log();
        }

        public override double? GetAltitude(T basin)
        {
            var northBasin = GetNorthBasin(basin);
            var southBasin = GetSouthBasin(basin);

            // return basin.Vartheta;// KQQaxisTan;
            /* диапазон 1,8м для k8, 1м для k9 http://hist.tk/hw/file:Deltah_Q-h_Q_s.png
            var hQ = northBasin.Intersect(basin);
            return basin.Altitude = basin.Hto[0] - hQ;//*/

            /* симметричной относительно экватора и диапазон меньше 1 см для k9.
            var VtoN = WaterModel.GetV(basin, northBasin, NeighborVert.North);
            var VtoS = WaterModel.GetV(basin, southBasin, NeighborVert.South);
            return VtoN + VtoS;//*/

            /* trying to accelerate Intersect()
            if (Math.Abs(basin.Vartheta) > .2)
                return (basin.Beta - northBasin.Beta) * basin.rOfEllipse;
            return null;*/

            Water.Move(basin, northBasin, NeighborVert.North);
            Water.Move(basin, southBasin, NeighborVert.South);

            // return (basin.Vartheta < 0 ? Math.PI - basin.Delta_g_meridian : basin.Delta_g_meridian) * 1000;
            return basin.HeightOQ;

            /*if (northBasin != null)
            {
                //return basin.Beta;
                // Угол Δβ не равен ThetaPix(θpix)
                //var dbeta = basin.Theta - northBasin.Theta;

                //// геоцентрические координаты, obsolete
                // range 65% .. 87%
                //return (dbeta / HealpixManager.ThetaPix) * 100;

                // geiod faces and sides faces intersection 
                //var Qq = basin.Theta - (Math.PI / 2 - Math.Atan(N.Y / N.X));
                // range -.3km .. .3km, not symmetric to equator
                //return (dbeta - Qq * 2)*basin.r;
            }*/
        }

        internal override void GradientAndHeightCrosses()
        {
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                if (basin.HasWater())
                {
                    var northBasin = GetNorthBasin(basin);
                    var southBasin = GetSouthBasin(basin);

                    // null if _symmetricPoles == false
                    if (northBasin != null)
                        basin.Hto[(int)NeighborVert.North] = basin.Intersect(northBasin);
                    if (southBasin != null)
                        basin.Hto[(int)NeighborVert.South] = basin.Intersect(southBasin);
                }
            }
        }

        /// <summary>
        /// PixMan.Pixels ordered asc
        /// PixMan.Pixels.Length often != HealpixManager.RingsCount and maybe PixMan.Pixels.Length == _maxRing
        /// </summary>
        protected new MeridianCoor GetNorthBasin(MeridianCoor basin)
        {
            if (basin.Ring == 1)
            {
                return _symmetricPoles ? PixMan.Pixels[1] : null;
            }
            return PixMan.Pixels[basin.Ring - 2];
        }

        protected MeridianCoor GetSouthBasin(MeridianCoor basin)
        {
            if (basin.Ring == MaxRing)
            {
                return _symmetricPoles ? PixMan.Pixels[MaxRing - 2] : null;
            }
            return PixMan.Pixels[basin.Ring];
        }
    }
}