using System;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    public class DemData : WaterMoving<Basin3>
    {
        public DemData(HealpixManager man, Basin3[] basins) : base(man, basins)
        {
        }

        public override ReliefType ReliefType => ReliefType.Mask;
        public override ReliefType ReliefBedType => ReliefType.Tbi;

        public double Hoq { get; set; }

        public override void Init(bool reliefFromDb = true, Datum datum = null)
        {
            base.Init(reliefFromDb, datum);
            foreach (var basin in PixMan.Pixels)
            {
                var tbi = ReliefBed.GetAltitude(basin);
                var mask = (MaskType)Relief.GetAltitude(basin);
                switch (mask)
                {
                    case MaskType.OceanBathymetry:
                    case MaskType.LakeAbove:
                    case MaskType.LakeBelow:
                        basin.Depth = tbi;
                        break;
                    default:
                        basin.Depth = -tbi;
                        break;
                }
                basin.Hoq = Hoq;
            }
        }

        public override double? GetAltitude(Basin3 basin)
        {
            throw new NotImplementedException();
        }
    }
}