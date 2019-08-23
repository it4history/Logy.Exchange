using System.Collections.Generic;
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Rectangle<T> where T : HealCoor
    {
        public Rectangle(double leftDownX, double leftDownY, double rightUpX, double rightUpY)
        {
            From = new Coor(leftDownX, leftDownY);
            To = new Coor(rightUpX, rightUpY);
        }

        public Coor From { get; set; }
        public Coor To { get; set; }

        public bool Contains(T basin)
        {
            return basin.X >= From.X && basin.Y >= From.Y &&
                   basin.X <= To.X && basin.Y <= To.Y;
        }

        public T[] Subset(HealpixManager man)
        {
            var result = new List<T>();
            for (var p = 0; p < man.Npix; p++)
            {
                var basin = man.GetCenter<T>(p);
                if (Contains(basin))
                    result.Add(basin);
            }
            return result.ToArray();
        }
    }
}