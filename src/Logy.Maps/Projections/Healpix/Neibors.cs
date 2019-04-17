using System.Collections;

namespace Logy.Maps.Projections.Healpix
{
    public class Neibors<T> : IEnumerable where T : HealCoor
    {
        private readonly T[] _basins;

        public Neibors(T[] basins)
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
            get { return _basins[(int) direction]; }
            set { _basins[(int) direction] = value; }
        }

        /*        public static implicit operator T[] (Neibors<T> neibors) { return neibors._basins; }
                public static implicit operator Neibors<T>(T[] data) { return new Neibors<T>(data); }*/
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _basins.GetEnumerator();
        }
    }
}