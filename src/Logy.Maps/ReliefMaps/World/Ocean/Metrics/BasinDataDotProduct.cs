using System;
using System.Linq;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics
{
    public class BasinDataDotProduct : WaterMoving<BasinDotProduct>
    {
        public BasinDataDotProduct(
            HealpixManager man, 
            double? min = null, 
            double? max = null)
            : base(man, null, min, max)
        {
            ColorsMiddle = 0;

            Init();
            foreach (var basin in PixMan.Pixels)
            {
                RecalcDelta_g(basin);

                // 0 .. almost Pi/2 
                //// Gammas = new double[4];

                var alphas = new double[4];

                var bisectors = new Point3D[4];
                var lengths = new double[4];
                /// !var surface = basin.Surface;
                /// var xyPlane = new Plane(basin.Q3, new UnitVector3D(0, 0, basin.Vartheta > 0 ? 1 : -1));
                /// !var xAxis = xyPlane.IntersectionWith(surface);
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = PixMan.Pixels[man.Neibors.Get(to, basin)];
                    basin.Neibors[to] = toBasin;

                    basin.Froms[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);

                    basin.MeanEdges[(int)to] = man.Neibors.MeanBoundary(basin, to);

                    basin.HtoBase[(int)to] = basin.Metric(toBasin, (int)to);

                    // !var otherQprojection = toBasin.Q3.ProjectOn(surface);//TraverseCalm);
                    // var dx = toBasin.Qb.X * Math.Sin(basin.Lambda.Value - toBasin.Lambda.Value);
                    // var dy = new Line3D(toBasin.Q3, otherQprojection).Length;
                    // lengths[(int)to] = Math.Sqrt(dx * dx + dy * dy);
                    // basin.Gammas[(int)to] = Math.Asin(dy / lengths[(int)to]);
                    /* !var Q_otherProjection = new Line3D(basin.Q3, otherQprojection);
                    lengths[(int) to] = Q_otherProjection.Length;

                    var thirdPoint = new Ray3D(basin.Q3, surface.Normal).LineTo(toBasin.Q3).StartPoint;
                    var anglePlane = new Plane(basin.Q3, thirdPoint, toBasin.Q3);
                    var lineQ3s = new Line3D(basin.Q3, toBasin.Q3);
                    bisectors[(int) to] = Point3D.MidPoint(toBasin.Q3, thirdPoint);*/

                    // var xAxis = surface.Normal.Rotate(new UnitVector3D(0, 0, 1), 90, AngleUnit.Degrees);

                    // !alphas[(int) to] = xAxis.Direction.SignedAngleTo(Q_otherProjection.Direction, surface.Normal).Radians;
                    /*телесный угол, calc Cos by DotProduct
                    alphas[(int)to] = surface.Normal.SignedAngleTo(lineQ3s.Direction,
                        anglePlane.Normal//surface.Normal.Orthogonal
                        ).Radians;//*/
                }

                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    basin.NormLengths[(int)to] = lengths[(int)to] / lengths.Sum();
                    var deltaAngle = alphas[(int)to] - alphas[(int)NeighborManager.GetOpposite(to)];
                    var koef = Math.Cos(1 * (Math.Abs(deltaAngle) - Math.PI));
                    /// koef = Math.Sin(.5 * (alphas[(int)to] - alphas[(int)NeighborManager.GetOpposite(to)]));
                    /// koef = Math.Sin(1 * (alphas[(int)to]));
                    /// koef = Math.Tan(alphas[(int)to] - Math.PI/2);
                    basin.Koef[(int)to] = Math.Abs(Math.Pow(koef, 1));
                }

                /// !basin.SpecNormal = new Line3D(basin.Q3, Point3D.Centroid(bisectors).MirrorAbout(surface)).Direction;

                if (WithRelief)
                {
                    int waterHeight;
                    var heightOQ = GetHeights(basin, (int)basin.RadiusOfEllipse, out waterHeight);
                    basin.Hoq = heightOQ;
                    if (waterHeight > 0)
                    {
                        basin.Depth = waterHeight - heightOQ;
                    }
                    else
                    {
                        basin.Depth = -heightOQ;
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

        public override ReliefType ReliefBedType => ReliefType.Tbi;

        public override double? GetAltitude(BasinDotProduct basin)
        {
            // return basin.Delta_g_meridian*1000;
            // return basin.Visual * 1000;
            //            var superKoef = basin.Koef2.Min() / basin.Koef2.Max();
            //            var superKoef = basin.Koef.Min() / basin.Koef.Max();
            // return superKoef;
            if (basin.HasWater())
            {
                if (basin.Type == Direction.Nw)
                {
                }
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = basin.Neibors[to];

                    var @from = basin.Froms[(int)to];
                    var koef
                        = .25;
                    /// = basin.Koef[(int)to] / basin.Koef.Sum();
                    /// var koef = basin.Koef2[(int)to] / basin.Koef2.Sum(); koef = koef2;

                    // todo balance deltaH relative to basin.WaterHeight
                    var height = basin.Hto[(int)to] - toBasin.Hto[@from];

                    var moved = Water.PutV(
                        basin,
                        toBasin,
                        /* height / 4, */
                        height * koef,
                        /* height* superKoef/4, */
                        (int)to, 
                        @from);
                    if (Math.Abs(moved) > 0)
                    {
                    }
                    if (basin.Hoq > 1)
                    {
                    }
                }
            }
            return basin.HasWater() ? basin.Hoq : (double?)null;
        }

        public override void GradientAndHeightCrosses()
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

        protected override void CalcDeltasH()
        {
            foreach (var basin in PixMan.Pixels)
            {
                if (basin.HasWater() /* && basin.P==62 */)
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
                        var deltaLambda = (basin.Lambda - toBasin.Lambda).Value * basin.BetaSin; /// *2;

                        var length = Math.Sqrt((deltaBeta * deltaBeta) + (deltaLambda * deltaLambda));
                        lengths[(int)to] = length;

                        // var asin = Math.Asin(deltaBeta / length);
                        // alphas[(int) to] = deltaLambda > 0 ? asin : Math.PI - asin;
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

                    // todo exclude second calculaton of the same pair, set and look on basin.Volumes
                    foreach (Direction to in Enum.GetValues(typeof(Direction)))
                    {
                        var toBasin = basin.Neibors[to];
                        double heightToVer;

                        var hor = NeighborManager.GetHor(to);
                        var heightToHor = basin.Hto[2 + hor] - toBasin.Hto[2 + 1 - hor];
                        /// (basin.hOQ - toBasin.hOQ) //not r because of bottom relief  * 1; //hack
                        if (Math.Abs(heightToHor) > 10)
                        {
                        }

                        var vert = NeighborManager.GetVert(to);
                        if ((basin.Ring == 1 && vert == NeighborVert.North)
                            || (basin.Ring == HealpixManager.RingsCount && vert == NeighborVert.South)
                            || basin.Type == to)
                        {
                            basin.Froms[(int)to] = (int)NeighborManager.GetOppositeHor(to);
                            heightToVer = 0;
                        }
                        else
                        {
                            var from = NeighborManager.GetOpposite(to);
                            basin.Froms[(int)to] = (int)from;
                            heightToVer = basin.Hto[(int)vert] -
                                     toBasin.Hto[1 - (int)vert];

                            // if (basin.Type == from && HtoVer < 0) HtoVer = -HtoVer * .5;//hack
                        }

                        var deltaHfull = (Math.Sin(gammas[(int)to]) * heightToVer) + (Math.Cos(gammas[(int)to]) * heightToHor);
                        basin.DeltasH[(int)to] = deltaHfull * normLengths[(int)to];
                        if (Math.Abs(basin.DeltasH[(int)to]) > 100
                            || basin.P == 17)
                        {
                        }
                    }
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
                inter.Qmeridian = new Point2D(
                    (.5 * (east.Qmeridian.X + west.Qmeridian.X)
                     /* correct projection */
                     * Math.Cos(basin.Lambda.Value)) - lambda,
                    .5 * (east.Qmeridian.Y + west.Qmeridian.Y));
            }
            return basin.Intersect(inter);
        }

        private void RecalcDelta_g(Basin3 basin)
        {
            if (Spheric)
            {
                basin.InitROfEllipse(HealpixManager); /// Ellipsoid.MeanRadius
                basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
            }
        }
    }
}