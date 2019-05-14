using System.Runtime.Serialization;
using Logy.Maps.Coloring;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public abstract class DataEarth
    {
        // to store max and min at least
        [IgnoreDataMember]
        public ColorsManager Colors;

        [IgnoreDataMember]
        public string Dimension = "m";
    }
}