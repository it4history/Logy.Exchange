using System;
using MathNet.Numerics;

namespace Logy.Maps.Geometry
{
    public class Triangles
    {
        /// <summary>
        /// sinuses theorem for triangles
        /// </summary>
        /// <returns>a</returns>
        public static double SinusesTheorem(double alpha, double c, double beta)
        {
            return (Math.Sin(alpha) / Math.Sin(Math.PI - alpha - beta)) * c;
        }

        public static double TansSum(double a, double b)
        {
            return (a + b) / (1 + (a * b));
        }

        /// <summary>
        /// obsolete, I thought that g directed not to geocenter and a is 
        /// </summary>
        private double CalcGPureToCenter(double varphi, double a, double g)
        {
            var tan = Math.Tan(varphi);
            var s = FindRoots.Quadratic((a * a) - (g * g), -2 * a, 1 + (tan * tan));
            var gx = s.Item1.Real;
            var gy = gx * tan;
            return Math.Sqrt((gx * gx) + (gy * gy));
        }
    }
}