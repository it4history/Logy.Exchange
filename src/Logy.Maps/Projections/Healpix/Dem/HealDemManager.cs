using System.Collections.Generic;
using System.Linq;

namespace Logy.Maps.Projections.Healpix.Dem
{
    public class HealDemManager
    {
        private readonly List<HealpixManager> _levels= new List<HealpixManager>();
        private int[,] _dem;

        public HealDemManager(int kidsK)
        {
            for (int i = 0; i <= kidsK; i++)
                _levels.Add(new HealpixManager(i));
        }

        public int[,] GetDem(int baseCell)
        {
            var kidsMan = _levels.Last();
            _dem = new int[kidsMan.Nside, kidsMan.Nside];
            Call(0, baseCell, 0, 0);
            return _dem;
        }

        private void Call(int k, int p, int x, int y)
        {
            if (k < _levels.Count - 1)
            {
                var kidsMan = 1 << (_levels.Count - 1 - k - 1);
                var kids = _levels[k].GetCenter(p).GetKids(_levels[k + 1]);
                Call(k + 1, kids[0], x, y);
                Call(k + 1, kids[1], x + kidsMan, y);
                Call(k + 1, kids[2], x, y + kidsMan);
                Call(k + 1, kids[3], x + kidsMan, y + kidsMan);
            }
            else
            {
                _dem[y, x] = p;
            }
        }
    }
}