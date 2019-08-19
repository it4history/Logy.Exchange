using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.Geometry;
using Logy.Maps.Metrics;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Basemap;
using Logy.Maps.ReliefMaps.Geoid;
using Logy.Maps.ReliefMaps.Water;
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

        private Polygon<Basin3> _polygon;

        public static Point3D O3 { get; } = new Point3D(0, 0, 0);
        public static UnitVector3D Oz { get; } = new UnitVector3D(0, 0, 1);

        /// <summary>
        /// RotationVector
        /// </summary>
        public static UnitVector3D OxMinus { get; } = new UnitVector3D(-1, 0, 0);

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
                    var normal = OxMinus; 

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
                    _s_q = new Plane(Normal.Value, Q3); // Q3 and not r - tested in OceanDataTests
                }
                return _s_q.Value;
            }
        }

        /// <summary>
        /// Mean sea level
        /// </summary>
        public Plane S_geiod => new Plane(Normal.Value, Qgeiod);

        public Neighbors<Basin3> Neighbors { get; } = new Neighbors<Basin3>(new Basin3[4]);

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

        public MetricType MetricType { get; set; } = MetricType.MeanEdge;

        /// <summary>
        /// rays that go either through Middle of edge (MeanEdge metric)
        ///  or through edges intersection (IntersectionRay metric)
        /// </summary>
        public Ray3D[] MetricRays { get; set; }
        public double[] Q3ToMetricRay { get; set; }
        public List<HealCoor> MetricRaysCoor
        {
            get
            {
                var l = new List<HealCoor>();
                foreach (var ray in MetricRays)
                {
                    l.Add(Utils3D.FromCartesian<HealCoor>(ray.Direction));
                }
                return l;
            }
        }

        /// <summary>
        /// was named InitialHto
        /// </summary>
        public double[] HtoBase { get; set; }

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

        public Polygon<Basin3> Polygon
        {
            get
            {
                return _polygon;
            }
            set
            {
                /*if (value != null)
                {
                    if (!value.Basins.Contains(this))
                    {
                        value.Basins.Add(this);
                    }
                }
                else
                {
                    if (_polygon != null)
                    {
                        if (_polygon.Basins.Contains(this))
                        {
                            _polygon.Basins.Remove(this);
                        }
                    }
                }*/
                _polygon = value;
            }
        }

        /// <summary>
        /// for what geoid?
        /// </summary>
        public Plane GeoidSurface { get; set; }

        public double RadiusGeoid { get; set; }

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
            MetricRays = new Ray3D[4];
            Q3ToMetricRay = new double[4];
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

        /// <param name="to">Direction</param>
        public double Metric(int to, bool initial = false)
        {
            return Metric(Neighbors[to], to, initial);
        }
        public virtual double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            switch (MetricType)
            {
                case MetricType.MeanEdge:
                case MetricType.IntersectionRay:
                    return (initial ? S_geiod : S_q).IntersectionWith(MetricRays[to]).DistanceTo(O3)
                           - HtoBase[to]; /// needed for OceanDataTests.HighBasin_31
                case MetricType.Edge:
                default:
                    /// !!!
                    return (initial ? S_geiod : S_q).IntersectionWith(MetricRays[to]).DistanceTo(O3)
                           - HtoBase[to]; /// needed for OceanDataTests.HighBasin_31
            }
        }

        /// <summary>
        /// MetricRays are required
        /// </summary>
        public void CorrectionSurface()
        {
            var points = (from edge in MetricRays select edge.Direction.ToPoint3D()).ToArray();

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

        public bool FillNewGeoid(WaterModel model)
        {
            var froms = new Dictionary<int, Basin3>();
            for (var from = 0; from < 4; from++)
            {
                var fromBasin1 = Neighbors[from];
                if (fromBasin1.Polygon != null && fromBasin1.P < P)
                {
                    froms.Add(from, fromBasin1);
                }
            }

            var water = froms.FirstOrDefault(f => !IsSolid(f.Value.Polygon.SurfaceType));
            var solid = froms.FirstOrDefault(f => IsSolid(f.Value.Polygon.SurfaceType));

            var fromWater = water.Value;
            var fromWaterTo = Opposites[water.Key];
            if (HasWater())
            {
                if (fromWater == null)
                {
                    if (froms.Count == 0)
                    {
                        SetPolygon(SurfaceType.WorldOcean, solid);
                    }
                    else
                    {
                        if (Disbalance(froms, model, false))
                            return false;

                        var diff = Radius - RadiusFromNeighbor(solid);
                        SetPolygon(
                            Math.Abs(diff) > model.Threshhold ? SurfaceType.Lake : SurfaceType.WorldOcean,
                            solid);
                    }
                }
                else
                {
                    if (Disbalance(froms, model))
                        return false;

                    var height = Metric(water.Key) - fromWater.Metric(fromWaterTo);

                    // if border of this. inner basin is thiner than a pixel
                    var thisHigherThanWater = height > 0 && WaterHeight < height;
                    var thisLowerThanWater = height < 0 && fromWater.WaterHeight < -height;
                    if (thisHigherThanWater || thisLowerThanWater)
                    {
                        SetPolygon(
                            Math.Abs(RadiusFromNeighbor(water) - Radius) < model.Threshhold
                                ? SurfaceType.WorldOcean
                                : SurfaceType.Lake,
                            water);
                    }
                    else
                    {
                        /* if rivers have slope then Threshhold is multiplied * 1.5; */
                        var diff = Math.Abs(height) * WaterModel.Koef;
                        if (diff > model.Threshhold)
                        {
                            if (solid.Value != null && water.Value.Polygon.SurfaceType == SurfaceType.Lake)
                            {
                                // there are issues on lakes (like Baikal) borders maybe because of geoid undulations in Earth2014
                                if (diff > model.Threshhold * 10)
                                    return false;
                            }
                            else
                                return false;
                        }

                        SetPolygon(fromWater.Polygon.SurfaceType, water, false);
                    }
                }
            }
            else
            { /* !HasWater() */
                if (solid.Value == null)
                {
                    if (fromWater == null)
                    {
                        throw new ApplicationException("first basin must have water to know geoid radius");
                    }

                    if (Disbalance(froms, model))
                        return false;

                    SetPolygon(SurfaceType.Solid, water);
                }
                else
                {
                    if (Disbalance(froms, model, false))
                        return false;

                    SetPolygon(SurfaceType.Solid, solid, false);
                }
            }
            return true;
        }

        public void SetPolygon(SurfaceType surfaceType, KeyValuePair<int, Basin3> from, bool newPolygon = true)
        {
            switch (surfaceType)
            {
                case SurfaceType.WorldOcean:
                    GeoidSurface = S_q;
                    RadiusGeoid = Radius;
                    break;
                case SurfaceType.Lake:
                case SurfaceType.Solid:
                    RadiusFromNeighbor(from, true);
                    break;
            }
            Polygon = newPolygon ? new Polygon<Basin3>(surfaceType, this, from.Value) : from.Value.Polygon;
        }

        /// <param name="fromPair">for solid should have actual Value.GeoidSurface</param>
        /// <param name="set">may set to this. solid basin</param>
        public double RadiusFromNeighbor(KeyValuePair<int, Basin3> fromPair, bool set = false)
        {
            var fromTo = Opposites[fromPair.Key];
            var diff = HtoBase[fromPair.Key] - fromPair.Value.HtoBase[fromTo];
            var ray = fromPair.Value.MetricRays[fromTo];

            var fromGeoidSurface = fromPair.Value.GeoidSurface;
            var geoidPoint = fromGeoidSurface.IntersectionWith(ray) + (diff * ray.Direction);

            var geoidSurface = new Plane(Normal.Value, geoidPoint);
            var q_geoid = geoidSurface.IntersectionWith(RadiusRay);
            var geoidRadius = q_geoid.DistanceTo(O3);
            if (set)
            {
                GeoidSurface = geoidSurface;
                RadiusGeoid = geoidRadius;
            }
            return geoidRadius;
        }

        private static bool IsSolid(SurfaceType surfaceType)
        {
            return surfaceType == SurfaceType.Solid;
        }

        private static bool IsSurfaceTypeNeeded(bool waterSurfaceType, KeyValuePair<int, Basin3> f)
        {
            return (waterSurfaceType && !IsSolid(f.Value.Polygon.SurfaceType))
                   || (!waterSurfaceType && IsSolid(f.Value.Polygon.SurfaceType));
        }

        private bool Disbalance(Dictionary<int, Basin3> froms, WaterModel model, bool waterSurfaceType = true)
        {
            var from = froms.FirstOrDefault(f => IsSurfaceTypeNeeded(waterSurfaceType, f));
            foreach (var fromOther in froms.Where(f => IsSurfaceTypeNeeded(waterSurfaceType, f)
                                                       && f.Value != from.Value))
            {
                var diffWaters = RadiusFromNeighbor(from) - RadiusFromNeighbor(fromOther);
                if (Math.Abs(diffWaters) > model.Threshhold)
                {
                    return true;
                }
            }
            return false;
        }
    }
}