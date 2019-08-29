using System;
using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Projections.Healpix
{
    public class Node : IComparable
    {
        public Node(Basin3 basin, int? direction = null)
        {
            // koef to Hto
            Basin = basin;
            Direction = direction;
        }

        public Node(Node node)
        {
            // Hoq
            Basin = node.Basin;
        }

        public Basin3 Basin { get; }
        public int? Direction { get; set; }
        public double Koef { get; set; } = 1;

        public int NodeInRing => Basin.Ring - Basin.PixelInRing;
        public string Key => ToString();

        public override string ToString()
        {
            // return $"{Basin.P}{(Direction.HasValue ? "_" + Direction.Value : "")}";
            return $"{Basin.Ring}{NodeInRing}{(Direction.HasValue ? "_" + Direction.Value : "")}";
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            var other = obj as Node;
            return other == null ? 1 : Key.CompareTo(other.Key);
        }
    }
}