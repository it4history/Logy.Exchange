using System.Collections.Generic;
using System.Drawing.Imaging;
using Logy.Maps.Geometry.WanderMaps;
using Logy.Maps.ReliefMaps.Map2D;

namespace Logy.Maps.Geometry.Tests
{
    /// <summary>
    /// http://hist.tk/ory/Карта_театра_войны_Российской_империи_против_Франции_и_её_союзников,_1813
    /// </summary>
    public class PoleSearchTests : PoleSearchData
    {
        public PoleSearchTests(Map2DBase<City> map) : base(map,
            new List<City>
            {
                new City("Житомир", 28.66, 50.25, 48.43), // старая широта 48° 25' (48.42)
                new City("Овруч", 28.81, 51.32, 49.2, true), // 49.12
                new City("Яссы", 27.59, 47.17, 45.99), // старая широта 45°50' (45.83)

                new City("Псков", 28.34, 57.82, 54.3), // старая широта 54° (54)
                new City("Полоцк", 28.78, 55.49, 52.3), // 52.15
                new City("Рига", 24.11, 56.95, 54.9, true), // старая широта 53°35' (53.58)

                new City("Вильнюс", 25.28, 54.68, 52.8, true), // 51.5
                new City("Минск", 27.55, 53.92, 51.4), // 50.91
                // new City("Париж", 2.35, 48.86, 48.8),


                //new City("Москва", 37.62, 55.76, 50),
                //new City("Питер", 30.32, 59.95, 55),
                //https://chispa1707.livejournal.com/3479284.html?thread=41651188#t41651188
                new City("New Castle", -75.5653, 39.6647, 40),
            })
        {
        }
    }

    public class PoleSearch1813 : Map2DBase<City>
    {
        public PoleSearch1813() : base(7, LegendType.None)
        {
            YResolution = 3;
        }

        protected override ImageFormat ImageFormat => ImageFormat.Tiff;

        protected override DataForMap2D<City> MapData => new PoleSearchTests(this);
    }
}