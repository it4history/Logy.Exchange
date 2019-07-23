using System;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.ReliefMaps.World
{
    public class Earth2014CorrectionData : DataForMap2D<HealCoor>
    {
        private int _landPixelsCount;

        public Earth2014CorrectionData(Map2DBase<HealCoor> map) : base(map)
        {
            ColorsMiddle = 0;
        }

        public override ReliefType ReliefBedType => ReliefType.Mask;

        public override double? GetAltitude(HealCoor basin)
        {
            var surface = Relief.GetAltitude(basin);
            var mask = (MaskType)ReliefBed.GetAltitude(basin);
            if (mask == MaskType.OceanBathymetry)
            {
                if (surface > 0)
                {
                    _landPixelsCount++;
                    return surface;
                }
            }

            return mask == MaskType.OceanBathymetry ? -.01 /*by ColorsUnder*/ : (double?)null;
        }

        public override void Log()
        {
            Console.WriteLine($"Ocean with surface > 0m; basins count: {_landPixelsCount}");
            base.Log();
        }
    }
}