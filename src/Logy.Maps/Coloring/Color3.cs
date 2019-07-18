using System;
using System.Drawing;

namespace Logy.Maps.Coloring
{
    public class Color3 : IComparable
    {
        public Color3(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color3(Color c) : this(c.R, c.G, c.B)
        {
        }

        /// <summary>
        /// from 0 to 255
        /// </summary>
        public double R { get; }
        public double G { get; }
        public double B { get; }

        public double Sum => R + G + B;

        public static Color3 operator -(Color3 a, Color3 b)
        {
            return new Color3(a.R - b.R, a.G - b.G, a.B - b.B);
        }

        public static Color3 operator +(Color3 a, Color3 b)
        {
            return new Color3(a.R + b.R, a.G + b.G, a.B + b.B);
        }

        public static Color3 operator *(double b, Color3 a)
        {
            return new Color3(a.R * b, a.G * b, a.B * b);
        }

        public static explicit operator Color(Color3 a)
        {
            return Color.FromArgb((int)a.R, (int)a.G, (int)a.B);
        }

        #region IComparable
        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return (int)Sum;
        }

        public int CompareTo(object other)
        {
            var b = other as Color3;
            if (b != null)
            {
                return Sum.CompareTo(b.Sum);
            }
            return -1;
        }
        #endregion
    }
}