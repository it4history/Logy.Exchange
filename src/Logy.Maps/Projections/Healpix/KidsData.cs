using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Projections.Healpix
{
    public class KidsData : DataForMap2D<HealCoor>
    {
        /// <summary>
        /// key - parent P
        /// </summary>
        private readonly Dictionary<int, HealCoor[]> _kids = new Dictionary<int, HealCoor[]>(); 

        public KidsData(Map2DBase<HealCoor> map) : base(map)
        {
            var fromPixelInRing = 3;
            var parentsMan = new HealpixManager(K - 1);
            for (int ring = 1; ring <= parentsMan.RingsCount; ring += 1)
            {
                var parentP = parentsMan.GetP(ring, fromPixelInRing);
                _kids.Add(
                    parentP,
                    parentsMan.GetCenter(parentP).GetKids(parentsMan, HealpixManager).Select(kidP => HealpixManager.GetCenter(kidP)).ToArray());
                parentP = parentsMan.GetP(ring, fromPixelInRing + 1);
                _kids.Add(
                    -parentP,
                    parentsMan.GetCenter(parentP).GetKids(parentsMan, HealpixManager).Select(kidP => HealpixManager.GetCenter(kidP)).ToArray());
            }
        }

        public override double? GetAltitude(HealCoor basin)
        {
            foreach (var pair in _kids)
            {
                foreach (var kid in pair.Value)
                {
                    if (kid.P == basin.P)
                        return pair.Key;
                }
            }
            return null;
        }
    }
}