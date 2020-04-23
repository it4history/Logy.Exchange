using System;
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
        /// p indices and ParentP (because HealpixManager.GetParent is not implemented yet)
        /// v - row from NE to SW, u - column from SE to NW 
        /// !!! attention !!! u is ordered from right to left
        /// filled in CalcDem()
        /// </summary>
        private Tuple<int, int?>[,] _dem;

        /// <param name="parentK">0 means for all Earth</param>
        public HealDemManager(int kidsK, int parentK = 0)
        {
            for (int i = 0; i <= kidsK; i++)
                _levels.Add(new HealpixManager(i));
            ParentK = parentK;
            Size = 1 << (kidsK - parentK);
        }

        public HealpixManager KidsMan => _levels.Last();

        private int ParentK { get; }
        private int Size { get; }
        private int Half => Size / 2;

        /// <param name="parentCell">P of basin from ParentK to calculate kids of</param>
        /// <param name="parentPK">K for what to calculate ParentP, used for CorrectionBundle</param>
        /// <returns></returns>
        public int[,] CalcDem(int parentCell, int? parentPK = null)
        {
            _dem = new Tuple<int, int?>[Size, Size];
            Call(ParentK, parentCell, 0, 0, parentPK ?? _levels.Count - 2, null);

            // for tests
            var demOnlyP = new int[Size, Size];
            for (var v = 0; v < Size; v++)
            for (var u = 0; u < Size; u++)
            {
                demOnlyP[v, u] = _dem[v, u].Item1;
            }
            return demOnlyP;
        }

        public Basin3[] GetNewBasins()
        {
            var result = new Basin3[Size * Size];
            var kidsman = _levels.Last();
            for (var v = 0; v < Size; v++)
            for (var u = 0; u < Size; u++)
            {
                var p = _dem[v, u].Item1;
                var basin = kidsman.GetCenter<Basin3>(p);
                if (_dem[v, u].Item2 != null)
                    basin.ParentP = _dem[v, u].Item2.Value;
                result[v * Size + u] = basin;
            }
            return result;
        }

        public BasinDem[] GetDem(WaterMoving<Basin3> data, bool withCurvature = false)
        {
            var center = withCurvature ? GetCurvatureCenter(data) : (Plane?) null;

            var result = new BasinDem[Size * Size];
            for (var v = 0; v < Size; v++)
            for (var u = 0; u < Size; u++)
            {
                var i = v * Size + u;
                var pixman = data.PixMan;

                result[i] = new BasinDem(
                    pixman.Full ? pixman.Pixels[_dem[v, u].Item1] : pixman.Pixels[i],
                    center);
            }
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
                var kids = _levels[k].GetCenter(p).GetKids(_levels[k], _levels[k + 1]);

                var someParent = k == parentPK ? p : parentP;

                // kids array order is   0   and json   1   described at http://logy.gq/lw/HEALPix#for_DEM
                //                      2 1            3 0
                //                       3              2
                // !!! u is ordered in _dem from right to left !!!
                Call(k + 1, kids[1], u, v, parentPK, someParent);
                Call(k + 1, kids[0], u + kidsShift, v, parentPK, someParent);
                Call(k + 1, kids[3], u, v + kidsShift, parentPK, someParent);
                Call(k + 1, kids[2], u + kidsShift, v + kidsShift, parentPK, someParent);
            }
            else
            {
                _dem[v, u] = new Tuple<int, int?>(p, parentP);
            }
        }
    }
}