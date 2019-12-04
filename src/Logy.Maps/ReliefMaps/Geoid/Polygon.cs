using System.Collections.Generic;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    /// <summary>
    /// todo use SortUtils for border pairs
    /// </summary>
    public class Polygon<T> where T : Basin3
    {
        public Polygon(SurfaceType surfaceType, T first, T from = null)
        {
            From = from;
            First = first;
            SurfaceType = surfaceType;
        }

        /// <summary>
        /// neighbor basin
        /// </summary>
        public T From { get; }

        public T First { get; }

        public SurfaceType SurfaceType { get; }

        /// <summary>
        /// cache
        /// </summary>
        public List<T> Basins { get; set; } = new List<T>();
    }
}