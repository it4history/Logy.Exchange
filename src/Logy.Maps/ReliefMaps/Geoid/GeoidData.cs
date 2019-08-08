using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class GeoidData : BasinDataAbstract<GeoidBasin>
    {
        private readonly Earth2014Manager _reliefMask;

        public GeoidData(HealpixManager man) : base(man)
        {
            _reliefMask = new Earth2014Manager(ReliefType.Mask, 1);
        }

        public override void InitMetrics(GeoidBasin basin)
        {
            basin.Mask = (MaskType)_reliefMask.GetAltitude(basin);
            switch (basin.Mask)
            {
                case MaskType.OceanBathymetry:
                    if (basin.Hoq > 0)
                    {
                        // Earth2014 correction described at Logy.Maps.ReliefMaps.World.Earth2014Correction
                        basin.Hoq = 0; /// or Depth = -Hoq;
                    }
                    break;
                case MaskType.LandAbove:
                case MaskType.LandBelow:
                case MaskType.IceLake:
                    if (basin.WaterHeight > 0)
                    {
                        // ignore groundwater, ice lakes
                        basin.Depth = -basin.Hoq;
                    }
                    break;
                /// case MaskType.IceAbove:
                case MaskType.IceBelow:
                case MaskType.IceShelf:
                    if (basin.WaterHeight > 0)
                    {
                        // ignore ice
                        basin.Depth = -basin.Hoq;
                    }
                    break;
            }

            base.InitMetrics(basin);
        }
        public override void Dispose()
        {
            _reliefMask.Dispose();
        }
   }
}