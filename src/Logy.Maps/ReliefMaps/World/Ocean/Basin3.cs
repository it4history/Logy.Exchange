﻿using System;
using System.Linq;
using Logy.Maps.Geometry;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class Basin3 : BasinAbstract
    {
        private double _deltaGTraverse;

        /// <summary>
        /// nulled when gradient changed
        /// </summary>
        private UnitVector3D? _normal;

        /// <summary>
        /// depends on hOQ only from r
        /// </summary>
        private Point3D _q3;

        private bool _actualQ3;

        private Plane? _s_q;

        public static Point3D O3 { get; } = new Point3D(0, 0, 0);
        public static UnitVector3D Oz { get; } = new UnitVector3D(0, 0, 1);

        public static Point3D OzEnd => Oz.ToPoint3D();

        public override double Hoq
        {
            get
            {
                return base.Hoq;
            }
            internal set
            {
                base.Hoq = value;
                _actualQ3 = false;
                _s_q = null;
            }
        }

        public Point2D Qmeridian
        {
            get { return new Point2D(Radius * BetaSin, Radius * BetaCos); }
            set { }
        }

        public Point3D Q3
        {
            get
            {
                if (!_actualQ3)
                {
                    var qb = Qmeridian;
                    _q3 = new Point3D(
                        LambdaMinusPi2Sin * qb.X,
                        LambdaSin * qb.X,
                        qb.Y);
                    _actualQ3 = true;
                }
                return _q3;
            }
        }

        /// <summary>
        /// may depend on AxisOfRotation
        /// ignores internal waters, marks only Mean sea level
        /// </summary>
        public Point3D Qgeiod
        {
            get
            {
                var paleoRadius = RadiusSpheric ? RadiusOfEllipse : Ellipsoid.RadiusPaleo(this);
                var x = paleoRadius * BetaSin;
                return new Point3D(
                    LambdaMinusPi2Sin * x,
                    LambdaSin * x,
                    paleoRadius * BetaCos);
            }
        }

        public Line3D RadiusLine => new Line3D(O3, Q3);

        public Ray3D RadiusRay => new Ray3D(O3, RadiusLine.Direction);

        public UnitVector3D? Normal
        {
            get
            {
                if (_normal == null)
                {
                    var normal = Matrixes.RotationVector; 

                    // Matrixes.Rotate() analog in radians
                    normal = normal.Rotate(
                        new UnitVector3D(0, 1, 0),
                        new Angle((Math.Sign(Vartheta) * (Delta_g_meridian + Delta_s_meridian)) + Phi, AngleUnit.Radians));
                    _normal = normal.Rotate(
                        new UnitVector3D(0, 0, 1),
                        new Angle(-(Delta_g_traverse + Delta_s_traverse) - Lambda.Value, AngleUnit.Radians));
                }
                return _normal.Value;
            }
            private set
            {
                _normal = value;
                _s_q = null;
            }
        }

        /// <summary>
        /// Top face; surface; plane
        /// used in metrics
        /// </summary>
        public Plane S_q
        {
            get
            {
                if (_s_q == null)
                {
                    _s_q = new Plane(Normal.Value, Q3); // Q3 and not r - tested in BasinTests
                }
                return _s_q.Value;
            }
        }

        /// <summary>
        /// Mean sea level
        /// </summary>
        public Plane S_geiod => new Plane(Normal.Value, Qgeiod);

        public Neibors<Basin3> Neibors { get; } = new Neibors<Basin3>(new Basin3[4]);

        /// <summary>
        /// key - NeighborVert (DirectionType ?)
        /// relative to Q, positive to equator and to ?
        /// </summary>
        public override double[] Hto { get; set; }

        /// <summary>
        /// key - Direction
        /// </summary>
        public override bool[] Volumes { get; set; }

        /// <summary>
        /// neighbor of the same ring,
        /// is not too correct for pole closest rings
        /// </summary>
        public Direction? Type { get; set; }
        public int[] Opposites { get; set; }

        public Matrix<double> Matrix { get; set; }

        /// <summary>
        /// was named InitialHto
        /// </summary>
        public double[] HtoBase { get; set; }
        public Ray3D[] MeanEdges { get; set; }

        /// <summary>
        /// angle, directed to East
        /// </summary>
        public double Delta_g_traverse
        {
            get
            {
                return _deltaGTraverse;
            }
            set
            {
                _deltaGTraverse = value;
                Normal = null;
            }
        }
        public override double Delta_g_meridian
        {
            get
            {
                return base.Delta_g_meridian;
            }
            set
            {
                base.Delta_g_meridian = value;
                Normal = null;
            }
        }

        /// <summary>
        /// set _normal = null;
        /// </summary>
        public double Delta_s_meridian { get; set; }

        /// <summary>
        /// set _normal = null;
        /// </summary>
        public double Delta_s_traverse { get; set; }

        #region reserved
        /// <summary>
        /// in Rotation Axis plane
        /// as a rule from north to south
        /// http://hist.tk/ory/Радиус_Земли#меридианный
        /// </summary>
        public double Am { get; set; }

        /// <summary>
        /// perpendicular to Rotation Axis plane that holds ν prime vertical radius of curvature
        /// as a rule from west to east
        /// http://hist.tk/ory/Радиус_Земли#Transverse
        /// </summary>
        public double Atrans { get; set; }
        #endregion

        public override void OnInit(HealpixManager man)
        {
            // todo why angle with opposite sign?
            var rotation = Matrix3D.RotationAroundYAxis(new Angle(-Phi, AngleUnit.Radians))
                           * Matrix3D.RotationAroundZAxis(new Angle(Lambda.Value, AngleUnit.Radians));
            Matrix = rotation.Transpose();

            base.OnInit(man);

            Hto = new double[4];
            Volumes = new bool[4];
            Opposites = new int[4];
            MeanEdges = new Ray3D[4];
            HtoBase = new double[4];
        }

        public override void WaterReset()
        {
            base.WaterReset();
            for (int to = 0; to < 4; to++)
                Volumes[to] = false;
        }

        /// <returns>typeof Direction</returns>
        public int GetFromAndFillType(Direction to, Basin3 toBasin, HealpixManager man)
        {
            if (Ring == toBasin.Ring)
            {
                Type = to;
            }

            var vert = NeighborManager.GetVert(to);
            if ((Ring == 1 && vert == NeighborVert.North)
                || (Ring == man.RingsCount && vert == NeighborVert.South)
                || Type == to)
            {
                return (int)NeighborManager.GetOppositeHor(to);
            }
            return (int)NeighborManager.GetOpposite(to);
        }

        public override double RecalculateDelta_g(bool revert = true)
        {
            var aTraverse = base.RecalculateDelta_g(false);
            Delta_g_traverse = Math.Atan(aTraverse / GVpure);
            return 0;
        }

        public virtual void InitMetrics()
        {
            /// CorrectionSurface();
            for (int to = 0; to < 4; to++)
            {
                HtoBase[to] = Metric(to, true);
            }
        }

        /// <param name="to">Direction</param>
        public double Metric(int to, bool initial = false)
        {
            return Metric(Neibors[to], to, initial);
        }
        public virtual double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            /* MeanEdge metric */
            return (initial ? S_geiod : S_q).IntersectionWith(MeanEdges[to]).DistanceTo(O3) 
                   - HtoBase[to]; /// needed for BasinDataTests.HighBasin_31
        }

        /// <summary>
        /// MeanEdges are required
        /// </summary>
        internal void CorrectionSurface()
        {
            var points = (from edge in MeanEdges select edge.Direction.ToPoint3D()).ToArray();

            var correctionVector1 = new Plane(points[0], points[2], points[1]).Normal;
            var correctionVector2 = new Plane(points[1], points[2], points[3]).Normal;
            /// var correctionVector = (correctionVector1 + correctionVector2) * Matrix;
            var correctionVector = new UnitVector3D((correctionVector1 + correctionVector2).ToVector()) * Matrix;
            var k = 20;

            Altitude =
            Delta_s_meridian = -(Vartheta < 0 ? 1 : -1) * correctionVector[2] / k;
            Delta_s_traverse = -correctionVector[1] / k; 
            Normal = null;
        }
    }
}