using System;
using System.Runtime.Serialization;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public abstract class BasinAbstract : HealCoor
    {
        private double _deltaGMeridian;
        public static Point2D O { get; } = new Point2D(0, 0);

        #region static angles
        public double BetaSin { get; private set; }
        public double BetaCos { get; private set; }
        public double LambdaSin { get; private set; }
        public double LambdaMinusPi2Sin { get; private set; }
        public double LambdaCos { get; private set; }

        /// <summary>
        /// from 0 to Pi
        /// </summary>
        public double Theta { get; set; } /*try to remove*/
        public double ThetaTan { get; set; }

        /// <summary>
        /// from 0 to Pi/2, from -Pi/2 to -0
        /// </summary>
        public double Vartheta { get; set; }

        /// <summary>
        /// Pi/2 .. -Pi/2
        /// </summary>
        public double Varphi => (Math.PI / 2) - Theta;

        /// <summary>
        /// phi
        /// </summary>
        public double Delta_gq => Theta - Vartheta;
        #endregion

        /// <summary>
        /// http://hist.tk/ory/Сферический_датум#радиус
        /// r
        /// </summary>
        public double Radius => Hoq + RadiusOfEllipse;

        /// <summary>
        /// geoid surface set by a mathematical formula
        /// </summary>
        public double RadiusOfEllipse { get; private set; }
        public bool RadiusSpheric { get; private set; }

        public double RingArea { get; private set; }
        public double Area { get; private set; }

        /// <summary>
        /// acceleration, vertical to sphere, no Centrifugal
        /// </summary>
        public double GVpure { get; set; }

        /// <summary>
        /// acceleration, horizontal to sphere, no Centrifugal
        /// directed to equator of Oz, 0 on poles and equator
        /// max: .016 on +-45grad
        /// </summary>
        public double GHpure { get; set; }
        /// <summary>
        /// used when gravitational axis is shifted 
        /// GHpureTraverse and GHpure are orthogonal vectors that lay on S_sphere
        /// directed to East
        /// </summary>
        public double GHpureTraverse { get; set; }

        /// <summary>
        /// angle, directed to equator of Oz
        /// it approximates geoid surface to sphere with RadiusOfEllipse radius 
        /// </summary>
        public virtual double Delta_g_meridian
        {
            get { return _deltaGMeridian; }
            set { _deltaGMeridian = value; }
        }

        /// <summary>
        /// key - NeighborVert
        /// </summary>
        public abstract double[] Hto { get; set; }

        /// <summary>
        /// whether water was moved during WaterMoving.Frame(...)
        /// </summary>
        public abstract bool[] Volumes { get; set; }

        #region water
        /// <summary>
        /// directed up
        /// relative to rOfEllipse 
        /// may include geoidUndulation
        /// </summary>
        [DataMember]
        public virtual double Hoq
        {
            get;
            /* may influence on many things! like S_q, Qb, Q3 of Basin3 */
            internal set;
        }

        /// <summary>
        /// directed down
        /// relative to rOfEllipse 
        /// null, if not limited in depth 
        /// if hOQ is less than -Depth.Value then this is underground water hydroconnected with ground
        /// 
        /// int? is not enough when geoidUndulation included
        /// </summary>
        /// let be serialized if really needed 
        [DataMember]
        public double? Depth { get; set; }

        // h_{water}
        public double WaterHeight => Hoq + Depth.Value;

        public double Volume
        {
            get
            {
                if (Depth != null && HasWater())
                {
                    return WaterHeight * Area;
                }
                return 0;
            }
        }

        public double varphi1 { get; internal set; }
        #endregion

        /// <summary>
        /// delta_gq ForVisualzation 
        /// from 0 or poles and equator to 0.0017 on +- 45 parallels
        /// </summary>
        public static double GoodDeflection(double vartheta, double delta_gq)
        {
            return vartheta < 0 ? Math.PI - delta_gq : delta_gq;
        }

        public bool HasWater(double threshhold = 0)
        {
            return Depth == null || WaterHeight > threshhold;
        }

        public virtual void WaterReset()
        {
        }

        /// <param name="deltaH">!= 0</param>
        public void WaterIn(double deltaH, int direction)
        {
            lock (this)
            {
                Hoq += deltaH;
                Volumes[direction] = true;
            }
        }

        /// <param name="newR">null for spheric Earth</param>
        public void InitROfEllipse(HealpixManager man, double? newR = null)
        {
            RadiusOfEllipse = newR ?? Earth2014Manager.Radius2Add;
            RadiusSpheric = newR == null;
            Hoq = Hoq; // clearing cache

            Area = RadiusOfEllipse * RadiusOfEllipse * man.OmegaPix;
            RingArea = Area * PixelsCountInRing;
        }

        public override void OnInit(HealpixManager man)
        {
            base.OnInit(man);
            LambdaSin = Math.Sin(Lambda.Value);
            LambdaMinusPi2Sin = Math.Sin(Lambda.Value - (Math.PI / 2));
            LambdaCos = Math.Cos(Lambda.Value);
            BetaSin = Math.Sin(Beta.Value);
            BetaCos = Math.Cos(Beta.Value);

            /* old
            var thetaTan = Ellipsoid.CalcThetaTan(Beta.Value);
            var varphi = Ellipsoid.CalcVarPhi(thetaTan);
            Theta = Math.PI / 2 - varphi; // faster than Atan(thetaTan) and Atan(thetaTan)<0 when thetaTan>Pi/2
            */

            // the same geoid-ellipsoid is approximated by spheres with different radiuses
            Theta = Beta.Value;

            InitROfEllipse(man, Ellipsoid.Radius(Varphi));

            Vartheta = Ellipsoid.CalcVarTheta(Math.Tan(Theta));
            var goodDeflectionAngle = GoodDeflection(Vartheta, Delta_gq);
            GHpure = CalcGpure(Varphi, Theta, Vartheta, goodDeflectionAngle); /// needs Vartheta

            Delta_g_meridian = goodDeflectionAngle;
        }

        public double CalcGpure(double varphi, double theta, double vartheta, double goodDeflectionAngle)
        {
            // vertical to ellipsoid surface
            var g = EllipsoidAcceleration.GravitationalSomigliana(varphi);
            /// return g * 100;

            double a, aVertical;
            // this is 3) method http://hist.tk/ory/Способ_расчета_центробежного_ускорения, use b.Q3 for 2)
            var aMeridian = Datum.Normal.CentrifugalSimple(RadiusOfEllipse, varphi, theta, out a, out aVertical);
            /// vertical to ellipsoid surface
            var aVert = Math.Abs(a * Math.Sin(vartheta));
            /// horizontal to ellipsoid surface
            /// var aHoriz = a * Math.Cos(Vartheta);

            // vertical to sphere
            GVpure = (g + aVert) * Math.Cos(goodDeflectionAngle); /// Triangles.CalcGPureToCenter

            // return gVpure*10000;
            // horizontal to sphere
            // max: .03299
            // horizontal to sphere
            var gHor = (g + aVert) * Math.Sin(goodDeflectionAngle);

            // gToCenterByThetaCos = gVpure / Math.Abs(Math.Cos(Theta));
            // return basin.GoodDeflectionAngle * 1000;
            // vertical to sphere
            var aV = a * Math.Sin(theta);
            /// return aV * 100;*/
            /// return aH * 100;
            return gHor - aMeridian;
        }

        /// <returns>Delta_g_traverse</returns>
        public virtual double RecalculateDelta_g(Datum datum, bool revert = true)
        {
            // return basin.gHpure * 1000;
            double a;
            double aTraverse;
            double aVertical;
            var aMeridian = datum.Centrifugal(this, out a, out aTraverse, out aVertical);

            // range: 0..0.0034
            var gv = GVpure - aVertical;
            var newDeflectionAngleTan = (GHpure + aMeridian) / gv;

            // todo try to rid of newDeflectionAngle and calculate without Atan, Tan
            var newDeflectionAngle = Math.Atan(newDeflectionAngleTan);
            /// range -0.1..0m
            /// return (basin.GoodDeflectionAngle - newDeflectionAngle) * basin.r;
            /// range: 0..0.0034
            /// return Triangles.TansSum(-basin.ThetaTan, newDeflectionAngle)*1000;

            Delta_g_meridian = (revert && Vartheta < 0)
                ? Math.PI - newDeflectionAngle
                : newDeflectionAngle;

            // range -0.1..0.1m
            // return (basin.Delta_gq - newDelta_g) * basin.r;

            return Math.Atan((GHpureTraverse + aTraverse) / gv);
        }

        /// <summary>
        /// maybe rudiment
        /// </summary>
        public virtual double Intersect(BasinAbstract otherBasin)
        {
            return 0;
        }
    }
}