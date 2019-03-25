using System;
using Logy.Maps.ReliefMaps.Meridian;
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

        // Period of rotation(sidereal day) in seconds
        public static double SiderealDayInSeconds = 86164.100637;

        public static UnitVector3D AxisOfRotation = Basin.Oz;

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
        /// https://ru.wikipedia.org/wiki/�������_�����#���������_���������
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
            return gWithAOnEquator 
                * ((1 + (k ?? (LessRadius * gOnPoles - BigRadius * gWithAOnEquator) / (BigRadius * gWithAOnEquator)) * sin2) 
                / Math.Sqrt(1 - (e2 ?? E2) * sin2));
        }

        /// <summary>
        /// ���������� �.�.����������� // ���������� ������������ : [� 5 �.] / ��. ���. �. �. ��������. 
        /// � �.: ��������� ������������, 1988
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
            return gWithAOnEquator * (1 + .0053024 * sin * sin - .0000059 * sin2a * sin2a);
        }

        public static double Centrifugal(MeridianBase basin)
        {
            double a, aTraverse;
            return Centrifugal(basin, out a, out aTraverse);
        }

        /// <param name="a">perpendicular to AxisOrRotation</param>
        /// <param name="aTraverse"></param>
        /// <returns>aMeridian,  directed to equator of OZ</returns>
        public static double Centrifugal(MeridianBase basin, out double a, out double aTraverse)
        {
            aTraverse = 0;
            if (AxisOfRotation == Basin.Oz)
            {
                a = Centrifugal(basin.r * Math.Cos(basin.Varphi));
                return a * Math.Abs(Math.Cos(basin.Theta));
            }

            var b = (Basin) basin;
            var axisEnd = AxisOfRotation.ToPoint3D();
            var axisOrtohonal = new Line3D(axisEnd, Basin.O3).LineTo(b.Q3, false);
            a = Centrifugal(axisOrtohonal.Length);

            var surfaceCalm = new Plane(b.NormalCalm, b.r);
            var QonAxisPlane = new Plane(axisEnd, b.Q3, Basin.O3);

            // aSphere direction
            var aSphereLine = surfaceCalm.IntersectionWith(QonAxisPlane);

            var b3unit = new Line3D(Basin.O3, b.Q3).Direction;
            // lays in surfaceCalm plane, directed to equator of AxisOfRotation if Math.Abs used
            var aSphere = //Math.Abs
                (a * AxisOfRotation.DotProduct(b3unit));// axisOrtohonal.Direction.DotProduct(aSphereLine.Direction)); 

            var aMeridianLine = surfaceCalm.IntersectionWith(b.MeridianCalm); //new Plane(OzEnd, Q3, O3);
            var aTraverseLine = surfaceCalm.IntersectionWith(b.TraverseCalm);
            Assert.AreEqual(0, aMeridianLine.Direction.DotProduct(aTraverseLine.Direction), .000000001);

            aTraverse = Math.Abs(aSphere * aSphereLine.Direction.DotProduct(aTraverseLine.Direction));

            var planeOZ = new Plane(Basin.Oz);
            var planeAxis = new Plane(AxisOfRotation);

            //directed to equator of Oz if Math.Abs used
            double aMeridian;
            /*if (aSphereLine.IsCollinear(aMeridianLine, .1))
            {
                aMeridian = aSphere;
            }
            else*/
            {
                var dotProduct = aSphereLine.Direction.DotProduct(aMeridianLine.Direction);
                aMeridian = Math.Abs(aSphere * dotProduct);
            }


            // if (AxisOfRotation != Basin.Oz)
            var spin = new Plane(Basin.OzEnd, b3unit.ToPoint3D(), axisEnd).Normal.DotProduct(b3unit);

            // "north" hemisphere of AxisOfRotation
            if (b3unit.DotProduct(AxisOfRotation) > 0) 
            {
                if (spin > 0)
                {
                    aTraverse = -aTraverse;
                }
            }
            else
            {
                if (spin < 0)
                {
                    aTraverse = -aTraverse;
                }
            }

            //aMeridian<0 if Q3 between planes or inside of cones (bug when angle is near 90) of OZ and AxisOfRotation
            if (planeOZ.SignedDistanceTo(b.Q3) * planeAxis.SignedDistanceTo(b.Q3) < 0)
            {
                aMeridian = -aMeridian;
            }
            else
            {
                var coneAxis = new UnitVector3D((Basin.Oz + AxisOfRotation).ToVector());
                if (new UnitVector3D(b.Q3.ToVector()).DotProduct(coneAxis) > coneAxis.DotProduct(Basin.Oz))
                {
                    //inside cone
                    aMeridian = -aMeridian;
                }
            }

            b.Reserved = aTraverse;
            b.Reserved = aMeridian;
            return aMeridian;
        }

        private static double Centrifugal(double distanceToAxis)
        {
            return distanceToAxis  * SpeedAngular() * SpeedAngular();
        }

        private static double SpeedAngular(double shift = 0)
        {
            return 2 * Math.PI / SiderealDayInSeconds; //SunDayMeanInSeconds
        }
    }
}