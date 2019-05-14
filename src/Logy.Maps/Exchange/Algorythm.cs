using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;

namespace Logy.Maps.Exchange
{
    public class Algorythm<T> where T : HealCoor
    {
        public Algorythm(WaterMoving<T> data)
        {
            Data = data;
        }

        public WaterMoving<T> Data { get; }

        public string Name => GetType().Name;

        /// <summary>
        /// ocean volume increase from initial, mln cub km
        /// </summary>
        public double Diff { get; set; }
    }
}