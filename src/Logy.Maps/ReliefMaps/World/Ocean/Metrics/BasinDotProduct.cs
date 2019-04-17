using System;
using System.Linq;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Meridian;
using Logy.Maps.ReliefMaps.Water;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics
{
    /// <summary>
    /// reserved class
    /// </summary>
    public class BasinDotProduct : Basin3
    {
        public double[] deltasH;
        public double[] Koef;
        public double[] Koef2;
        public double[] NormLengths;
        public UnitVector3D SpecNormal;

        public Point2D Q_traverse
        {
            get { return new Point2D(r * Math.Cos(Lambda.Value), r * Math.Sin(Lambda.Value)); }
        }

        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane MeridianCalm
        {
            get { return new Plane(OzEnd, Q3, O3); }
        }

        /// <summary>
        /// calm sphere
        /// </summary>
        public Plane TraverseCalm
        {
            get { return new Plane(MeridianCalm.Normal.ToPoint3D(), Q3, O3); }
        }

        public UnitVector3D NormalCalm { get; set; }

        /// <summary>
        /// tan of Delta_g_traverse (angle KyQQy from http://hist.tk/hw/file:Gradient_traverse.png)
        /// </summary>
        public double KQQaxisTanCotan_traverse { get; set; }

        internal override void PreInit(HealpixManager man)
        {
            base.PreInit(man);

            NormalCalm = new UnitVector3D(new UnitVector3D(-1, 0, 0) * Matrix.Transpose());

            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(GetKQQaxis_traverse());
        }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(false);

            var KQQaxis_traverse = GetKQQaxis_traverse();
            KQQaxisTanCotan_traverse = CalcKQQaxisTanCotan_traverse(KQQaxis_traverse);
            return KQQaxis_traverse;
        }

        public double Beta_traverse;

        public double Lambda_meridian;

        public double IntersectTraverse(Basin3 otherBasin)
        {
            var otherQtraverseProjection = otherBasin.Q3.ProjectOn(TraverseCalm);
            var Qt = otherQtraverseProjection.ToVector3D();
            Lambda_meridian = Qt.AngleTo(Q3.ToVector3D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_traverse,
                       r,
                       Lambda_meridian)
                   -
                   //r;
                   otherBasin.r; //there's deformation
            //Qt.Length;
        }

        public override double Intersect(BasinBase otherBasin)
        {
            var other = (MeridianBase)otherBasin;
            var Qt = new Vector2D(
                other.Q.X * Math.Cos(Lambda.Value - otherBasin.Lambda.Value),
                other.Q.Y);
            Beta_traverse = Qt.AngleTo(Qb /*or Q?*/.ToVector2D()).Radians;
            return Triangles.SinusesTheorem(
                       Math.PI / 2 + Delta_g_meridian,
                       r,
                       Beta_traverse)
                   -
                   //r;
                   otherBasin.r; //there's deformation
        }

        public Line2D KQ_traverse
        {
            get { return MeridianBase.GetKQ(KQQaxisTanCotan_traverse, Q_traverse); }
        }

        /// <returns>-Pi..Pi</returns>
        internal double GetKQQaxis_traverse()
        {
            //return Lambda.Value;
            return Lambda.Value > 1.5 * Math.PI ? Lambda.Value - 2.5 * Math.PI : Lambda.Value - .5 * Math.PI;
        }

        private double CalcKQQaxisTanCotan_traverse(double KqQaxisTraverse)
        {
            return Math.Tan(KqQaxisTraverse);
            return Math.Abs(KqQaxisTraverse) < Math.PI / 4
                ? Math.Tan(KqQaxisTraverse)
                : Math.Tan(Math.PI / 2 - KqQaxisTraverse);
        }
    }

    public class BasinDataDotProduct : WaterMoving<BasinDotProduct>
    {
        private readonly bool _withRelief;
        private readonly bool _spheric;

        public override ReliefType ReliefBedType
        {
            get { return ReliefType.Tbi; }
        }

        public BasinDataDotProduct(HealpixManager man, bool withRelief = false, bool spheric = false,
            double? min = null, double? max = null)
            : base(man, null, min, max)
        {
            _withRelief = withRelief;
            _spheric = spheric;
            ColorsMiddle = 0;

            foreach (var basin in PixMan.Pixels)
            {
                RecalcDelta_g(basin);

                // 0 .. almost Pi/2 
                //Gammas = new double[4];

                var alphas = new double[4];

                var bisectors = new Point3D[4];
                var lengths = new double[4];
                //!var surface = basin.Surface;
                var xyPlane = new Plane(basin.Q3, new UnitVector3D(0, 0, basin.Vartheta > 0 ? 1 : -1));
                //!var xAxis = xyPlane.IntersectionWith(surface);
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = PixMan.Pixels[man.Neibors.Get(to, basin)];
                    basin.Neibors[to] = toBasin;

                    basin.froms[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);

                    basin.MeanEdges[(int)to] = man.Neibors.MeanBoundary(basin, to);

                    basin.InitialHto[(int)to] = basin.Metric(toBasin, (int)to);

                    //!var otherQprojection = toBasin.Q3.ProjectOn(surface);//TraverseCalm);
                    //var dx = toBasin.Qb.X * Math.Sin(basin.Lambda.Value - toBasin.Lambda.Value);
                    //var dy = new Line3D(toBasin.Q3, otherQprojection).Length;
                    //lengths[(int)to] = Math.Sqrt(dx * dx + dy * dy);
                    //basin.Gammas[(int)to] = Math.Asin(dy / lengths[(int)to]);
                    /*!var Q_otherProjection = new Line3D(basin.Q3, otherQprojection);
                    lengths[(int) to] = Q_otherProjection.Length;

                    var thirdPoint = new Ray3D(basin.Q3, surface.Normal).LineTo(toBasin.Q3).StartPoint;
                    var anglePlane = new Plane(basin.Q3, thirdPoint, toBasin.Q3);
                    var lineQ3s = new Line3D(basin.Q3, toBasin.Q3);
                    bisectors[(int) to] = Point3D.MidPoint(toBasin.Q3, thirdPoint);*/

                    //var xAxis = surface.Normal.Rotate(new UnitVector3D(0, 0, 1), 90, AngleUnit.Degrees);

                    //!alphas[(int) to] = xAxis.Direction.SignedAngleTo(Q_otherProjection.Direction, surface.Normal).Radians;
                    /*телесный угол, calc Cos by DotProduct
                    alphas[(int)to] = surface.Normal.SignedAngleTo(lineQ3s.Direction,
                        anglePlane.Normal//surface.Normal.Orthogonal
                        ).Radians;//*/
                }
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {

                    basin.NormLengths[(int)to] = lengths[(int)to] / lengths.Sum();
                    var deltaAngle = (alphas[(int)to] - alphas[(int)NeighborManager.GetOpposite(to)]);
                    var koef = Math.Cos(1 * (Math.Abs(deltaAngle) - Math.PI));
                    //koef = Math.Sin(.5 * (alphas[(int)to] - alphas[(int)NeighborManager.GetOpposite(to)]));
                    //koef = Math.Sin(1 * (alphas[(int)to]));
                    //koef = Math.Tan(alphas[(int)to] - Math.PI/2);
                    basin.Koef[(int)to] = Math.Abs(Math.Pow(koef, 1));
                }
                // !basin.SpecNormal = new Line3D(basin.Q3, Point3D.Centroid(bisectors).MirrorAbout(surface)).Direction;

                if (_withRelief)
                {
                    int waterHeight;
                    var hOQ = GetHeights(basin, (int)basin.rOfEllipse, out waterHeight);
                    basin.hOQ = hOQ;
                    if (waterHeight > 0)
                    {
                        basin.Depth = waterHeight - hOQ;
                    }
                    else
                    {
                        basin.Depth = -hOQ;
                    }
                }
            }

            foreach (var basin in PixMan.Pixels)
            {
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = (BasinDotProduct)basin.Neibors[to];
                    basin.Koef2[(int)to] = basin.Koef[(int)to] * toBasin.Koef[(int)NeighborManager.GetOpposite(to)];
                }
            }
        }

        internal void RecalcDelta_g(Basin3 basin)
        {
            if (_spheric)
            {
                basin.InitROfEllipse(HealpixManager, Ellipsoid.MeanRadius);
                basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
            }
        }

        internal override void GradientAndHeightCrosses()
        {
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                if (basin.HasWater())
                {
                    for (int to = 0; to < 4; to++)
                    {
                        var toBasin = basin.Neibors[to];
                        var hto = basin.Metric(toBasin, to);
                        basin.Hto[(int)to] = hto;

                        /*                       
                        var HtoVer = basin.Intersect(toBasin);
                        var HtoHor = basin.IntersectTraverse(toBasin);
                        var deltaHfull = Math.Sin(gammas[(int) to]) * HtoVer + Math.Cos(gammas[(int) to]) * HtoHor;
                        basin.deltasH[(int) to] = deltaHfull * basin.NormLengths[(int) to];*/

                    }
                    /*if (basin.Ring > 1)
                    {
                        basin.Hto[(int) NeighborVert.North] = GetHto(basin, NeighborVert.North);
                    }
                    if (basin.Ring < HealpixManager.RingsCount)
                    {
                        basin.Hto[(int) NeighborVert.South] = GetHto(basin, NeighborVert.South);
                    }
                    basin.Hto[2 + (int)NeighborHor.East]
                        = (basin.IntersectTraverse(basin.Neibors[Direction.Ne])
                           + basin.IntersectTraverse(basin.Neibors[Direction.Se])) / 2;
                    basin.Hto[2 + (int)NeighborHor.West]
                        = (basin.IntersectTraverse(basin.Neibors[Direction.Nw])
                           + basin.IntersectTraverse(basin.Neibors[Direction.Sw])) / 2;*/
                }
            }
        }

        private static double GetHto(BasinDotProduct basin, NeighborVert direction)
        {
            Basin3 inter;
            /*if (basin.Type.HasValue && NeighborManager.GetVert(basin.Type.Value) == direction) 
            {
                var neibor = basin.Neibors[NeighborManager.GetOppositeHor(basin.Type.Value)];
                inter = neibor;
            }
            else*/
            {
                Basin3 east;
                Basin3 west;
                if (direction == NeighborVert.North)
                {
                    east = basin.Neibors[Direction.Ne];
                    west = basin.Neibors[Direction.Nw];
                }
                else
                {
                    east = basin.Neibors[Direction.Se];
                    west = basin.Neibors[Direction.Sw];
                }

                inter = new Basin3
                {
                };
                var lambda = (east.Lambda + west.Lambda).Value / 2;
                inter.Qb = new Point2D(.5 * (east.Qb.X + west.Qb.X)
                                       //correct projection
                                       * Math.Cos(basin.Lambda.Value - lambda),
                    .5 * (east.Qb.Y + west.Qb.Y));
            }
            return basin.Intersect(inter);
        }

        protected override void CalcDeltasH()
        {
            foreach (var basin in PixMan.Pixels)
            {
                if (basin.HasWater()
                    //   && basin.P==62
                )
                {
                    // -.5Pi .. 1.5Pi
                    var alphas = new double[4];
                    // Pi .. .5Pi
                    var gammas = new double[4];
                    var lengths = new double[4];
                    var normLengths = new double[4];
                    foreach (Direction to in Enum.GetValues(typeof(Direction)))
                    {
                        var toBasin = basin.Neibors[to];
                        var deltaBeta = (basin.Beta - toBasin.Beta).Value;
                        var deltaLambda = (basin.Lambda - toBasin.Lambda).Value
                                          * Math.Sin(basin.Beta.Value)
                            ; //*2;

                        var length = Math.Sqrt(deltaBeta * deltaBeta + deltaLambda * deltaLambda);
                        lengths[(int)to] = length;

                        //var asin = Math.Asin(deltaBeta / length);
                        //alphas[(int) to] = deltaLambda > 0 ? asin : Math.PI - asin;
                        gammas[(int)to] = Math.Atan(Math.Abs(deltaBeta / deltaLambda));
                    }
                    /*foreach (int to in Enum.GetValues(typeof(Direction)))
                    {
                        lengths[to] = lengths.Max() - lengths[to];
                    }*/
                    foreach (int to in Enum.GetValues(typeof(Direction)))
                    {
                        normLengths[to] = lengths[to] / lengths.Sum();
                    }

                    //todo exclude second calculaton of the same pair, set and look on basin.Volumes
                    foreach (Direction to in Enum.GetValues(typeof(Direction)))
                    {
                        var toBasin = basin.Neibors[to];
                        double HtoVer;

                        var hor = NeighborManager.GetHor(to);
                        var HtoHor = basin.Hto[2 + hor] - toBasin.Hto[2 + 1 - hor];
                        //(basin.hOQ - toBasin.hOQ) //not r because of bottom relief  * 1; //hack
                        if (Math.Abs(HtoHor) > 10)
                        {
                        }

                        var vert = NeighborManager.GetVert(to);
                        if (basin.Ring == 1 && vert == NeighborVert.North
                            || basin.Ring == HealpixManager.RingsCount && vert == NeighborVert.South
                            || basin.Type == to)
                        {
                            basin.froms[(int)to] = (int)NeighborManager.GetOppositeHor(to);
                            HtoVer = 0;
                        }
                        else
                        {
                            var from = NeighborManager.GetOpposite(to);
                            basin.froms[(int)to] = (int)from;
                            HtoVer = basin.Hto[(int)vert] -
                                     toBasin.Hto[1 - (int)vert];

                            //if (basin.Type == from && HtoVer < 0) HtoVer = -HtoVer * .5;//hack
                        }

                        var deltaHfull = Math.Sin(gammas[(int)to]) * HtoVer + Math.Cos(gammas[(int)to]) * HtoHor;
                        basin.deltasH[(int)to] = deltaHfull * normLengths[(int)to];
                        if (Math.Abs(basin.deltasH[(int)to]) > 100
                            || basin.P == 17)
                        {
                        }
                    }
                }
            }
        }

        public override double? GetAltitude(BasinDotProduct basin)
        {
            //return basin.Delta_g_meridian*1000;
            //return basin.Visual * 1000;
            //            var superKoef = basin.Koef2.Min() / basin.Koef2.Max();
            //            var superKoef = basin.Koef.Min() / basin.Koef.Max();
            //return superKoef;
            if (basin.HasWater())
            {
                if (basin.Type == Direction.Nw)
                {
                }
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = basin.Neibors[to];

                    var @from = basin.froms[(int)to];
                    var koef
                        = .25;
                    //= basin.Koef[(int)to] / basin.Koef.Sum();
                    //var koef = basin.Koef2[(int)to] / basin.Koef2.Sum(); koef = koef2;

                    //todo balance deltaH relative to basin.WaterHeight
                    var height = basin.Hto[(int)to] - toBasin.Hto[(int)@from];

                    var moved = Water.PutV(basin, toBasin,
                        //height / 4,
                        height * koef,
                        //height* superKoef/4,
                        (int)to, @from);
                    if (Math.Abs(moved) > 0)
                    {
                    }
                    if (basin.hOQ > 1)
                    {
                    }
                }
            }
            return basin.HasWater() ? basin.hOQ : (double?)null;
        }
    }
}