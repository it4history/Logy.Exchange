using System;
using Logy.MwAgent.Sphere;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Geometry
{
    public class Ellipsoid
    {
        #region https://en.wikipedia.org/wiki/Geodetic_Reference_System_1980

        /// <summary>
        /// Tyapkin - 6378160 (IAG-67)
        /// </summary>
        public const double BigRadius = 6378137d; // big axis, angle is measured from here

        /// <summary>
        /// Tyapkin - 6356778
        /// </summary>
        public const double LessRadius = 6356752.3141;

        /// <summary>
        /// near Earth2014Manager.Radius2Add
        /// </summary>
        public const double MeanRadius = 6371008.8;

        // Period of rotation(sidereal day) in seconds
        public const double SunDayMeanInSeconds = 86200.002;

        #endregion

        public static readonly double Ratio = BigRadius / LessRadius;

        protected static readonly double E2 = 1 - ((LessRadius * LessRadius) / (BigRadius * BigRadius));

        /// <summary>
        /// Hirt_Rexer2015_Earth2014.pdf
        /// </summary>
        /// <param name="varphi">http://hist.tk/ory/Широта#геоцентрическая</param>
        public static double Radius(double varphi)
        {
            var varphiSin = Math.Sin(varphi);
            var sin2 = varphiSin * varphiSin;
            return BigRadius * Math.Sqrt((1 - (E2 * (2 - E2) * sin2)) / (1 - (E2 * sin2)));
        }
        public static double VarphiPaleo(Coor coor, UnitVector3D axis)
        {
            var theta = axis.AngleTo(Utils3D.Cartesian(coor)).Radians;
            return (Math.PI / 2) - theta;
        }
        public static double RadiusPaleo(Coor coor, Datum datum)
        {
            return Radius(VarphiPaleo(coor, datum.Axis));
        }

        /// <summary>
        /// https://ru.wikipedia.org/wiki/Радиус
        /// </summary>
        public static double RadiusRu(double angleFromBigAxis)
        {
            var cos2 = Math.Cos(angleFromBigAxis) * Math.Cos(angleFromBigAxis);
            return LessRadius / Math.Sqrt(1 - (E2 * cos2));
        }

        /// <summary>
        /// conversion from spherical to ellipsoid
        /// 
        /// http://hist.tk/ory/Claessens,_S.J._2006
        /// </summary>
        /// <returns>Math.Tan(theta), http://hist.tk/ory/co-latitude#geocentric</returns>
        public static double CalcThetaTan(double beta)
        {
            return Ratio * Math.Tan(beta);
        }

        /// <returns>0 on poles, +Pi/2 on north equator, -Pi/2 on south equator</returns>
        public static double CalcVarTheta(double thetaTan)
        {
            return Math.Atan(thetaTan / (Ratio * Ratio));
        }

        /// <returns>
        /// varphi
        /// http://hist.tk/ory/Широта#геоцентрическая
        /// on equator varphi == 0 
        /// in sourth hemisphere varphi < 0
        /// </returns>
        public static double CalcVarPhi(double thetaTan)
        {
            var varphi = (Math.PI / 2) - Math.Atan(thetaTan);
            return varphi > (Math.PI / 2) ? varphi - Math.PI : varphi;
        }
    }
}