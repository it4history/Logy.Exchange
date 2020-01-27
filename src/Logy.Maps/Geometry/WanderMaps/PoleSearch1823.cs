using System.Collections.Generic;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.WanderMaps
{
    /// <summary>
    /// http://hist.tk/ory/Карта_Кавказа_С._Броневского,_А._Максимовича
    /// </summary>
    public class PoleSearch1823Data : PoleSearchData
    {
        public PoleSearch1823Data(Map2DBase<City> map) : base(map,
            new List<City>
            {
                new City("Дербент", 48.295830, 42.069170, 43.05),
                new City("Кизляр", 46.716670, 43.850000, 44.85),
                new City("Кума, устье", 46.900600, 44.809900, 45.7),
                new City("Поти", 41.666670, 42.150000, 43.3, true),
                new City("Тмутаракань", 36.714061, 45.219069, 45.2, true),
                new City("Ардлер", 39.916110, 43.433610, 44.55, true),
            })
        {
        }
    }

    public class PoleSearch1823 : Map2DBase<City>
    {
        public PoleSearch1823() : base(9, LegendType.None)
        {
            YResolution = 3;
        }

        protected override DataForMap2D<City> MapData => new PoleSearch1823Data(this);
    }
}