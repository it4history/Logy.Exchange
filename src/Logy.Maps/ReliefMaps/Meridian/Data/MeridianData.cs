using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.ReliefMaps.Meridian.Data
{
    public abstract class MeridianData<T> : WaterMoving<T> where T : MeridianCoor
    {
        protected MeridianData(HealpixManager man, T[] basins) : base(man, basins)
        {
            Init();
        }

        protected MeridianData(HealpixManager man, double? min = null, double? max = null) 
            : base(man, GetBasins(man), min, max)
        {
            Init();
        }

        public override void Init(bool reliefFromDb = true, Datum datum = null)
        {
            base.Init(false);
            Water.IsMeridian = true;
        }

        protected MeridianCoor GetNorthBasin(MeridianCoor basin)
        {
            if (basin.Ring == 1)
                return null;

            var northBasin = PixMan.Pixels.Length == HealpixManager.Npix
                ? PixMan.Pixels[HealpixManager.Neighbors.NorthMean(basin)]
                : (from p in PixMan.Pixels
                    where p.Ring == basin.Ring - 1
                    select p).First(); /*slow*/
            if (northBasin.Ring == basin.Ring)
            {
                throw new ApplicationException("northBasin.Ring == basin.Ring");
            }
            return northBasin;
        }

        private static T[] GetBasins(HealpixManager man)
        {
            var pix = new List<T>();
            T last;
            for (var south = man.Npix - 3; south > 0; south = man.Neighbors.NorthMean(last))
            {
                last = man.GetCenter<T>(south);
                pix.Add(last);
            }
            pix.Reverse(); /// GetAltitude looks for northbasin
            return pix.ToArray();
        }
    }
}