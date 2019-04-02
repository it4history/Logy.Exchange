using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Meridian
{
    public abstract class MeridianBase : BasinBase
    {
        /// <summary>
        /// top face intersects axis Y in point Ky with angle KyQQy
        /// tan of Theta-delta_g (angle KyQQy from http://hist.tk/hw/file:Gradient_spheric.png)
        /// 
        /// fortunately KyQQy == Pi/2 on equator
        /// </summary>
        public double KQQaxisTan { get; set; }

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);
            KQQaxisTan = Math.Tan(Vartheta);
        }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(revert);
            //return basin.Vartheta - Math.Atan(newDeflectionAngleTan);
            var KQQaxis = Theta - Delta_g_meridian;
            //return newKQQaxis;

            var newKQQaxisTan =
                //Triangles.TansSum(basin.ThetaTan, -newDeflectionAngleTan);
                Math.Tan(KQQaxis);
            // must be 0
            //return (basin.KQQaxisTan - newKQQaxisTan)*10000000;
            KQQaxisTan = newKQQaxisTan;

            return aTraverse;
        }

        public Point2D Q
        {
            get { return new Point2D(r * Math.Sin(Theta), r * Math.Cos(Theta)); }
        }

        /// <summary>
        /// fast
        /// </summary>
        public Line2D KQ
        {
            get
            {
                var grad_dTan = KQQaxisTan; //Triangles.TansSum(KQQaxisTan, hTan);

                return GetKQ(grad_dTan, Q);
            }
        }

        /// <param name="tanOrCotan">-Pi/2..PI/2 if cotan == false</param>
        public static Line2D GetKQ(double tanOrCotan, Point2D q, bool cotan = false)
        {
            var K = Math.Abs(tanOrCotan) < Math.Tan(Math.PI / 4)
                ? new Point2D(0, tanOrCotan * q.X + q.Y)
                : new Point2D((cotan ? tanOrCotan * q.Y : q.Y / tanOrCotan) + q.X, 0);
            return new Line2D(K, q);
        }

        /// <summary>
        /// todo do not use DistanceTo, return Point2D instead of double
        /// be careful with points
        /// </summary>
        public override double Intersect(BasinBase otherBasin)
        {
            var other = (MeridianBase)otherBasin;
            return KQ.IntersectWith(OQ(other.Q)).Value.DistanceTo(other.Q);
        }

        #region Love numbers

        const double strangeAngleKoeficient = 1.945;

        public double GethTan(double a)
        {
            // error in mydeflectionAngle because I missed gHpure
            //var mydeflectionAngle = aV / aH;// aH / (gVpure - aV);
            //diff = Math.Tan(basin.GoodDeflectionAngle) - mydeflectionAngle;
            /* strangeAngleKoeficient estimation for k9-k11 is the same: 1.945+-.002
            return basin.Ring == HealpixManager.RingsCount / 2 + 1
                ? (double?) null //here diff == 0.002
                : diff * 100;//*/

            /* range 21m (or 0.2% from ThetaPix for k9, 0.3% for k10, 0.6% for k11)
            return (Math.Tan(GoodDeflectionAngle) - 1.945 *mydeflectionAngle)
                   //* basin.r; 
                   / HealpixManager.ThetaPix * 100;//*/

            return Triangles.TansSum(ThetaTan, -KQQaxisTan) * r;
            /*return Math.Abs(basin.KQQaxisTan) < Math.Tan(Math.PI / 4)
                ? basin.KQQaxisTan
                : 1 / basin.KQQaxisTan;*/


            var gVpureByThetaCos = gVpure / Math.Cos(Theta);
            return strangeAngleKoeficient / (gVpureByThetaCos / a - ThetaTan); // aH / (gVpure - aV);
        }

        #endregion

        #region geocentric

        /// <summary>
        /// http://hist.tk/hw/—читать_градиент_спокойстви€#далее
        /// </summary>
        public double WaveHeight(Point2D N, double deltage)
        {
            var Qq = Theta - Math.Atan(N.Y / N.X);
            var Nq = Math.PI / 2 - Qq + Delta_gq;

            return Triangles.SinusesTheorem(deltage, N.DistanceTo(O), Nq);
        }

        /// <summary>
        /// from Q to vertial axis, length of L_vartheta
        /// </summary>
        public double R_vartheta
        {
            get { return Math.Abs(r * Math.Sin(Theta) / Math.Sin(Vartheta)); }
        }

        /// <returns>starts with Q(point on ellipsoid)</returns>
        public Line2D L_vartheta()
        {
            var sign = Vartheta >= 0 ? 1 : -1;
            var QGeodes = new Point2D(
                0, Q.Y - sign * R_vartheta * Math.Cos(Vartheta));
            return new Line2D(Q, QGeodes);
        }

        /// <summary>
        /// find M - intersection of R_vartheta and northBasin.R_vartheta
        /// and distances to ellipsoid
        /// </summary>
        public void IntersectGeodesic(MeridianCoor northBasin, out double more, out double less)
        {
            var lQ = L_vartheta();
            var lNorth = northBasin.L_vartheta();
            var M = lQ.IntersectWith(lNorth);
            var MQ = new Line2D(lQ.StartPoint, M.Value).Length;
            var MQnorth = new Line2D(lNorth.StartPoint, M.Value).Length;
            more = r > northBasin.r ? MQ : MQnorth;
            less = r > northBasin.r ? MQnorth : MQ;
        }

        #endregion
    }
}