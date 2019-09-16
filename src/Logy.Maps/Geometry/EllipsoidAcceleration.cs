using System;
using Logy.Maps.Metrics;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Geometry
{
    public class EllipsoidAcceleration : Ellipsoid
    {
        #region https://en.wikipedia.org/wiki/Clairaut's_theorem#Formula
        public const double GWithAOnEquator = 9.7803253359;
        public const double GOnPoles = 9.8321849378;
        #endregion

        /// <summary>
        /// https://en.wikipedia.org/wiki/Theoretical_gravity#Formulas
        /// 1984 Ellipsoidal Gravity Formula
        /// </summary>
        public static double GravitationalAcceleration(double angleFromBigAxis)
        {
            return GravitationalSomigliana(angleFromBigAxis, GWithAOnEquator, .00193185265241, .00669437999013);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Theoretical_gravity#Formulas
        /// International Gravity Formula 1980 (IGF80)
        /// on the WGS80 ellipsoid but now using the Somigliana equation
        /// </summary>
        public static double GravitationalAcceleration2(double angleFromBigAxis)
        {
            return GravitationalSomigliana(angleFromBigAxis, 9.7803267715, .001931851353, .006694380029);
        }

        /// <summary>
        /// https://ru.wikipedia.org/wiki/Теорема_Клеро#Уравнение_Сомильяны
        /// includes Centrifugal
        /// </summary>
        /// <param name="k">(b * Gp - a * Ge) / a * Ge</param>
        /// <returns>g perpendicular to ellipsoid surface</returns>
        public static double GravitationalSomigliana(
            double varphi, 
            double gWithAOnEquator = GWithAOnEquator,
            double gOnPoles = GOnPoles,
            double? k = null, 
            double? e2 = null)
        {
            var sin2 = Math.Sin(varphi) * Math.Sin(varphi);
            var realK = k ?? ((LessRadius * gOnPoles) - (BigRadius * gWithAOnEquator)) / (BigRadius * gWithAOnEquator);
            return gWithAOnEquator * ((1 + (realK * sin2)) / Math.Sqrt(1 - ((e2 ?? E2) * sin2)));
        }

        /// <summary>
        /// Грушинский Н.П.Гравиметрия // Физическая энциклопедия : [в 5 т.] / Гл. ред. А. М. Прохоров. 
        /// М.: Советская энциклопедия, 1988
        /// </summary>
        public static double GravitationalAcceleration1(double angleFromEquator)
        {
            return GravitationalClairaut(angleFromEquator, 9.780318);
        }

        /// <summary>
        /// includes Centrifugal but is good when supposed not include Centrifugal
        /// 
        /// from https://en.wikipedia.org/wiki/Clairaut%27s_theorem#Formula
        /// </summary>
        public static double GravitationalClairaut(
            double angleFromEquator,
            double gWithAOnEquator = GWithAOnEquator)
        {
            var sin = Math.Sin(angleFromEquator);
            var sin2a = Math.Sin(2 * angleFromEquator);
            return gWithAOnEquator * (1 + (.0053024 * sin * sin) - (.0000059 * sin2a * sin2a));
        }
    }
}