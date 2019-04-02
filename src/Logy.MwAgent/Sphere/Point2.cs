using System.Text.RegularExpressions;

namespace Logy.MwAgent.Sphere
{
    /// <summary>
    /// point on spheroid
    /// </summary>
    public class Point2
    {
        private double _x;
        private double _y;

        public Point2()
        {
        }

        public Point2(Point2 original) : this(original.X, original.Y)
        {
        }

        public Point2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2(string original)
        {
            var match = Regex.Match(original, @"(-?[\d\.]+):(-?[\d\.]+)");
            if (match.Success)
            {
                X = double.Parse(match.Groups[1].Value);
                Y = double.Parse(match.Groups[2].Value);
            }
        }

        /// <summary>
        /// 0 corresponds to West
        /// </summary>
        public virtual double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// 0 corresponds to North
        /// </summary>
        public virtual double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Point2 Normalize()
        {
            while (_x < 0) _x += 360;
            while (_x > 360) _x -= 360;
            while (_y < 0) _y += 180;
            while (_y > 180) _y -= 180;
            return this;
        }
    }
}