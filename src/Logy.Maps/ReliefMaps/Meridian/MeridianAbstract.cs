using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Meridian
{
    public abstract class MeridianAbstract : BasinAbstract
    {
        /// <summary>
        /// top face intersects axis Y in point Ky with angle KyQQy
        /// tan of Theta-delta_g (angle KyQQy from http://hist.tk/ory/file:Gradient_spheric.png)
        /// 
        /// fortunately KyQQy == Pi/2 on equator
        /// </summary>
        public double KQQaxisTan { get; set; }

        public Point2D Q => new Point2D(Radius * Math.Sin(Theta), Radius * Math.Cos(Theta));

        /// <summary>
        /// fast
        /// </summary>
        public Line2D KQ
        {
            get
            {
                var grad_dTan = KQQaxisTan; /// Triangles.TansSum(KQQaxisTan, hTan);

                return GetKQ(grad_dTan, Q);
            }
        }

        /// <summary>
        /// from Q to vertial axis, length of L_vartheta
        /// </summary>
        public double R_vartheta => Math.Abs(Radius * Math.Sin(Theta) / Math.Sin(Vartheta));

        /// <param name="tanOrCotan">-Pi/2..PI/2 if cotan == false</param>
        public static Line2D GetKQ(double tanOrCotan, Point2D q, bool cotan = false)
        {
            var pointK = Math.Abs(tanOrCotan) < Math.Tan(Math.PI / 4)
                ? new Point2D(0, (tanOrCotan * q.X) + q.Y)
                : new Point2D((cotan ? tanOrCotan * q.Y : q.Y / tanOrCotan) + q.X, 0);
            return new Line2D(pointK, q);
        }

        public override void OnInit(HealpixManager man)
        {
            base.OnInit(man);
            KQQaxisTan = Math.Tan(Vartheta);
        }

        public override void RecalculateDelta_g(Datum datum = null, bool revert = true)
        {
            RecalculateDelta_gCorrectIfDelta_g_traverseZero(datum, revert);
            /// return basin.Vartheta - Math.Atan(newDeflectionAngleTan);
            var aKQQaxis = Theta - Delta_g_meridian;
            /// return newKQQaxis;

            var newKQQaxisTan =
                /// Triangles.TansSum(basin.ThetaTan, -newDeflectionAngleTan);
                Math.Tan(aKQQaxis);

            // must be 0
            // return (basin.KQQaxisTan - newKQQaxisTan)*10000000;
            KQQaxisTan = newKQQaxisTan;
        }

        public static Line2D OQ(Point2D q)
        {
            return new Line2D(O, q);
        }

        /// <summary>
        /// todo do not use DistanceTo, return Point2D instead of double
        /// be careful with points
        /// </summary>
        public override double Intersect(BasinAbstract otherBasin)
        {
            var other = (MeridianAbstract)otherBasin;
            return KQ.IntersectWith(OQ(other.Q)).Value.DistanceTo(other.Q);
        }

        #region Love numbers

        //// const double strangeAngleKoeficient = 1.945;

        public double GethTan(double a)
        {
            // error in mydeflectionAngle because I missed gHpure
            // var mydeflectionAngle = aV / aH;// aH / (gVpure - aV);
            // diff = Math.Tan(basin.GoodDeflectionAngle) - mydeflectionAngle;
            /* strangeAngleKoeficient estimation for k9-k11 is the same: 1.945+-.002
            return basin.Ring == HealpixManager.RingsCount / 2 + 1
                ? (double?) null //here diff == 0.002
                : diff * 100;//*/

            /* range 21m (or 0.2% from ThetaPix for k9, 0.3% for k10, 0.6% for k11)
            return (Math.Tan(GoodDeflectionAngle) - 1.945 *mydeflectionAngle)
                   //* basin.r; 
                   / HealpixManager.ThetaPix * 100;//*/

            return Triangles.TansSum(ThetaTan, -KQQaxisTan) * Radius;
            /*return Math.Abs(basin.KQQaxisTan) < Math.Tan(Math.PI / 4)
                ? basin.KQQaxisTan
                : 1 / basin.KQQaxisTan;*/

            // var gVpureByThetaCos = gVpure / Math.Cos(Theta);
            // return strangeAngleKoeficient / (gVpureByThetaCos / a - ThetaTan); // aH / (gVpure - aV);
        }

        #endregion

        #region geocentric

        /// <summary>
        /// http://hist.tk/ory/—читать_градиент_спокойстви€#далее
        /// </summary>
        public double WaveHeight(Point2D n, double deltage)
        {
            var aQq = Theta - Math.Atan(n.Y / n.X);
            var aNq = (Math.PI / 2) - aQq + Delta_gq;

            return Triangles.SinusesTheorem(deltage, n.DistanceTo(O), aNq);
        }

        /// <returns>starts with Q(point on ellipsoid)</returns>
        public Line2D L_vartheta()
        {
            var sign = Vartheta >= 0 ? 1 : -1;
            var aQGeodes = new Point2D(
                0, Q.Y - (sign * R_vartheta * Math.Cos(Vartheta)));
            return new Line2D(Q, aQGeodes);
        }

        /// <summary>
        /// find M - intersection of R_vartheta and northBasin.R_vartheta
        /// and distances to ellipsoid
        /// </summary>
        public void IntersectGeodesic(MeridianCoor northBasin, out double more, out double less)
        {
            var lineQ = L_vartheta();
            var lineNorth = northBasin.L_vartheta();
            var pointM = lineQ.IntersectWith(lineNorth);
            var lengthMQ = new Line2D(lineQ.StartPoint, pointM.Value).Length;
            var lengthMQnorth = new Line2D(lineNorth.StartPoint, pointM.Value).Length;
            more = Radius > northBasin.Radius ? lengthMQ : lengthMQnorth;
            less = Radius > northBasin.Radius ? lengthMQnorth : lengthMQ;
        }

        #endregion
    }
}