using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Meridian;
using Logy.Maps.ReliefMaps.Meridian.Data;

namespace Logy.Maps.ReliefMaps.Water
{
    public class WaterAndBottomData : MeridianWater<MeridianCoor>
    {
        public WaterAndBottomData(HealpixManager man, double? min = null, double? max = null) : base(man, min, max)
        {
            ColorsMiddle = 0;

            var p = 0;
            for (var ring = 1; ring <= MaxRing; ring++)
            {
                var basin = PixMan.Pixels[ring - 1];
                var waterHeightAll = 0d;
                var surfaceHeightAll = 0d;
                var pixelsInRing = HealpixManager.PixelsCountInRing(ring);
                for (var inRing = 1; inRing < pixelsInRing; inRing++, p++)
                {
                    var coor = HealpixManager.GetCenter<HealCoor>(p);

                    int waterHeight;
                    surfaceHeightAll += GetHeights(coor, (int)basin.rOfEllipse, out waterHeight);
                    waterHeightAll += waterHeight;
                }

                if (waterHeightAll > 0)
                {
                    basin.hOQ = 0;
                    basin.Depth = waterHeightAll / pixelsInRing;
                }
                else
                {
                    basin.hOQ = surfaceHeightAll / pixelsInRing;
                    basin.Depth = -surfaceHeightAll / pixelsInRing;
                }
            }
        }

        public override ReliefType ReliefBedType
        {
            get { return ReliefType.Tbi; }
        }

        protected override bool IsReliefShape
        {
            // maybe impossible to calculate mean depth relative to shape radius
            get { return false; }
        }

        /*public override int Accuracy { get { return 1; } }

        protected override bool IsDynamicScale { get { return true; } }*/

        public override double? GetAltitude(MeridianCoor basin)
        {
            var hOQ = base.GetAltitude(basin);
            // return basin.Depth;
            return basin.HasWater() ? hOQ : null;
            return basin.HasWater() ? basin.WaterHeight : (double?)null;
        }

        public override double GetOceanVolume()
        {
            var oceanVolume = 0d;
            for (var ring = 1; ring <= MaxRing; ring++)
            {
                var basin = PixMan.Pixels[ring - 1];
                oceanVolume += basin.Volume * basin.PixelsCountInRing; //basin.RingArea;
            }
            return ToMilCumKm(oceanVolume);
        }
    }
}