using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Exchange
{
    public class Algorythm<T> where T : BasinBase
    {
        public Algorythm(WaterMoving<T> data)
        {
            Data = data;
        }

        public string Name => GetType().Name;

        /// <summary>
        /// ocean volume increase from initial, mln cub km
        /// </summary>
        public double Diff { get; set; }

        public WaterMoving<T> Data { get; set; }
    }
}