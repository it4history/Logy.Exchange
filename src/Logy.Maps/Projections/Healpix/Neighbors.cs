using System.Collections;

namespace Logy.Maps.Projections.Healpix
{
    public class Neighbors<T> : IEnumerable where T : HealCoor
    {
        private readonly T[] _basins;

        public Neighbors(T[] basins)
        {
            _basins = basins;
        }

        public T this[int direction]
        {
            get { return _basins[direction]; }
            set { _basins[direction] = value; }
        }

        public T this[Direction direction]
        {
            get { return _basins[(int)direction]; }
            set { _basins[(int)direction] = value; }
        }

        /*        public static implicit operator T[] (Neighbors<T> neighbors) { return neighbors._basins; }
                public static implicit operator Neighbors<T>(T[] data) { return new Neighbors<T>(data); }*/
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _basins.GetEnumerator();
        }
    }
}