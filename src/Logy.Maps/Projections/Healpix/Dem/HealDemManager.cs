using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    public class HealDemManager
    {
        private readonly List<HealpixManager> _levels = new List<HealpixManager>();
        private int[,] _dem;

        public HealDemManager(int kidsK, int baseK = 0)
        {
            for (int i = 0; i <= kidsK; i++)
                _levels.Add(new HealpixManager(i));
            BaseK = baseK;
            Size = 1 << (_levels.Count - baseK - 1);
            Basins = new Basin3[Size * Size];
        }

        public HealpixManager KidsMan => _levels.Last();
        public Basin3[] Basins { get; }
        private int BaseK { get; }
        private int Size { get; }

        /// <param name="baseCell">P of basin from BaseK to calculate kids of</param>
        /// <param name="parentPK">K for what to calculate ParentP, used for CorrectionBundle</param>
        /// <returns></returns>
        public int[,] CalcDem(int baseCell, int? parentPK = null)
        {
            _dem = new int[Size, Size];
            Call(BaseK, baseCell, 0, 0, parentPK ?? _levels.Count - 2, null);
            return _dem;
        }

        public BasinDem[] GetDem(WaterMoving<Basin3> data)
        {
            int p = 0;
            return Basins.Select(b => new BasinDem(data.PixMan.Pixels[p++])).ToArray();
        }

        private void Call(int k, int p, int x, int y, int parentPK, int? parentP)
        {
            if (k < _levels.Count - 1)
            {
                var kidsShift = 1 << (_levels.Count - 1 - k - 1);
                var kids = _levels[k].GetCenter(p).GetKids(_levels[k + 1]);

                var someParent = k == parentPK ? p : parentP;
                Call(k + 1, kids[0], x, y, parentPK, someParent);
                Call(k + 1, kids[1], x + kidsShift, y, parentPK, someParent);
                Call(k + 1, kids[2], x, y + kidsShift, parentPK, someParent);
                Call(k + 1, kids[3], x + kidsShift, y + kidsShift, parentPK, someParent);
            }
            else
            {
                _dem[y, x] = p;

                // because HealpixManager.GetParent is not implemented and to accelerate
                var basin = _levels[k].GetCenter<Basin3>(p);
                basin.ParentP = parentP.Value;
                Basins[(y * Size) + x] = basin; // ApplyBasin3(data.PixMan.Pixels[p]);
            }
        }
    }
}