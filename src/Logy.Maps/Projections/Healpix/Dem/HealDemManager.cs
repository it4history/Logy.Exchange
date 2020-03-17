using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Projections.Healpix.Dem
{
    public class HealDemManager
    {
        private readonly List<HealpixManager> _levels = new List<HealpixManager>();
        /// <summary>
        /// p indices 
        /// v - row from NE to SW, u - column from SE to NW
        /// filled in CalcDem()
        /// </summary>
        private int[,] _dem;

        /// <param name="parentK">0 means for all Earth</param>
        public HealDemManager(int kidsK, int parentK = 0)
        {
            for (int i = 0; i <= kidsK; i++)
                _levels.Add(new HealpixManager(i));
            ParentK = parentK;
            Size = 1 << (_levels.Count - parentK - 1);
        }

        public HealpixManager KidsMan => _levels.Last();

        /// <summary>
        /// _dem equivalent
        /// filled in CalcDem()
        /// </summary>
        public Basin3[] Basins { get; private set; }
        private int ParentK { get; }
        private int Size { get; }
        private int Half => Size / 2;

        /// <param name="parentCell">P of basin from ParentK to calculate kids of</param>
        /// <param name="parentPK">K for what to calculate ParentP, used for CorrectionBundle</param>
        /// <returns></returns>
        public int[,] CalcDem(int parentCell, int? parentPK = null)
        {
            _dem = new int[Size, Size];
            Basins = new Basin3[Size * Size];
            Call(ParentK, parentCell, 0, 0, parentPK ?? _levels.Count - 2, null);
            return _dem;
        }

        public BasinDem[] GetDem(WaterMoving<Basin3> data, bool withCurvature = false)
        {
            var center = withCurvature ? GetCurvatureCenter(data) : (Plane?) null;
            int p = 0;
            var result = Basins.Select(b => new BasinDem(data.PixMan.Pixels[p++], center)).ToArray();
            return result;
        }

        internal Plane GetCurvatureCenter(WaterMoving<Basin3> data)
        {
            var center = new[]
            {
                (Half - 1) * (Size + 1),
                (Half - 1) * (Size + 1) + 1,
                Half * (Size + 1) - 1,
                Half * (Size + 1)
            };
            var centerQ = center.Select(p => data.PixMan.Pixels[p].Q3.ToVector3D())
                              .Aggregate((q, qNext) => q + qNext) / 4;
            var centerNormal = center.Select(p => data.PixMan.Pixels[p].Normal.Value)
                .Aggregate((q, qNext) => (q + qNext).Normalize());
            return new Plane(centerQ.ToPoint3D(), centerNormal);
        }

        private void Call(int k, int p, int u, int v, int parentPK, int? parentP)
        {
            if (k < _levels.Count - 1)
            {
                var kidsShift = 1 << (_levels.Count - 1 - k - 1);
                var kids = _levels[k].GetCenter(p).GetKids(_levels[k + 1]);

                var someParent = k == parentPK ? p : parentP;

                // order http://logy.gq/lw/HEALPix#for_DEM is strangely no 1 0 3 2
                Call(k + 1, kids[0], u + kidsShift, v, parentPK, someParent);
                Call(k + 1, kids[1], u, v, parentPK, someParent);
                Call(k + 1, kids[2], u + kidsShift, v + kidsShift, parentPK, someParent);
                Call(k + 1, kids[3], u, v + kidsShift, parentPK, someParent);
            }
            else
            {
                _dem[v, u] = p;

                // because HealpixManager.GetParent is not implemented and to accelerate
                var basin = _levels[k].GetCenter<Basin3>(p);
                basin.ParentP = parentP.Value;
                Basins[(v * Size) + u] = basin; // ApplyBasin3(data.PixMan.Pixels[p]);
            }
        }
    }
}