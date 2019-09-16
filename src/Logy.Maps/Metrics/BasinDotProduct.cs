using System;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.Metrics
{
    /// <summary>
    /// reserved class
    /// </summary>
    public class BasinDotProduct : Basin3
    {
        public double[] DeltasH { get; set; }
        public double[] Koef { get; set; }
        public double[] Koef2 { get; set; }
        public double[] NormLengths { get; set; }
        public UnitVector3D SpecNormal { get; set; }

        public Point2D Q_traverse => new Point2D(Radius * LambdaCos, Radius * LambdaSin);

        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane TraverseCalm => new Plane(Meridian.Normal.ToPoint3D(), Q3, O3);

        public UnitVector3D NormalCalm { get; set; }

        /// <summary>
        /// tan of Delta_g_traverse (angle KyQQy from http://hist.tk/ory/file:Gradient_traverse.png)
        /// </summary>
        public double KQQaxisTanCotan_traverse { get; set; }

        public double Beta_traverse { get; set; }

        public double Lambda_meridian { get; set; }

        public Line2D KQ_traverse => MeridianAbstract.GetKQ(KQQaxisTanCotan_traverse, Q_traverse);

        public override void OnInit(HealpixManager man)
        {
            base.OnInit(man);

            NormalCalm = new UnitVector3D(OxMinus * Matrix.Transpose());

            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(GetKQQaxis_traverse());
        }

        public override double RecalculateDelta_g(Datum datum = null, bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(datum, false);

            var aKQQaxis_traverse = GetKQQaxis_traverse();
            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(aKQQaxis_traverse);
            return aKQQaxis_traverse;
        }

        public double IntersectTraverse(Basin3 otherBasin)
        {
            var otherQtraverseProjection = otherBasin.Q3.ProjectOn(TraverseCalm);
            var vectorQt = otherQtraverseProjection.ToVector3D();
            Lambda_meridian = vectorQt.AngleTo(Q3.ToVector3D()).Radians;
            return Triangles.SinusesTheorem(
                       (Math.PI / 2) + Delta_g_traverse,
                       Radius,
                       Lambda_meridian)
                   -
                   /// r;
                   otherBasin.Radius; /// there's deformation

            // Qt.Length;
        }

        public override double Intersect(BasinAbstract otherBasin)
        {
            var other = (MeridianAbstract)otherBasin;
            var vectorQt = new Vector2D(
                other.Q.X * Math.Cos(Lambda.Value - otherBasin.Lambda.Value),
                other.Q.Y);
            Beta_traverse = vectorQt.AngleTo(Qmeridian.ToVector2D()).Radians; /*or Q?*/
            return Triangles.SinusesTheorem(
                       (Math.PI / 2) + Delta_g_meridian,
                       Radius,
                       Beta_traverse)
                   -
                   /// r;
                   otherBasin.Radius; /// there's deformation
        }

        /// <returns>-Pi..Pi</returns>
        internal double GetKQQaxis_traverse()
        {
            // return Lambda.Value;
            return Lambda.Value > 1.5 * Math.PI ? Lambda.Value - (2.5 * Math.PI) : Lambda.Value - (.5 * Math.PI);
        }

        private double CalcKQQaxisTanCotan_traverse(double aKqQaxisTraverse)
        {
            return Math.Tan(aKqQaxisTraverse);
            /*return Math.Abs(KqQaxisTraverse) < Math.PI / 4
                ? Math.Tan(KqQaxisTraverse)
                : Math.Tan(Math.PI / 2 - KqQaxisTraverse);*/
        }
    }
}