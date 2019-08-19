using System;
using System.Runtime.Serialization;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Metrics;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class BasinDataAbstract<T> : WaterMoving<T> where T : Basin3
    {
        private double _q3_RayDistance;

        public BasinDataAbstract() { }
        public BasinDataAbstract(
            HealpixManager man,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false)
            : base(man, null, min, max, readAllAtStart)
        {
        }

        [IgnoreDataMember]
        public override ReliefType ReliefBedType => ReliefType.Tbi;

        [IgnoreDataMember]
        public Func<T, double> Visual { get; set; } = basin => basin.Hoq; //// basin.Altitude * 1000;

        /// <param name="full">if false then Depth, Hoq are not set</param>
        public override void Init(bool full = true)
        {
            base.Init(full);

            ColorsMiddle = 0;

            _q3_RayDistance = 0;
            foreach (var basin in PixMan.Pixels)
            {
                if (full && WithRelief)
                {
                    int waterHeight;
                    var hOQ = GetHeights(basin, (int)basin.RadiusOfEllipse, out waterHeight);
                    basin.Hoq = hOQ;
                    if (waterHeight > 0)
                    {
                        basin.Depth = waterHeight - hOQ;
                    }
                    else
                    {
                        basin.Depth = -hOQ;
                    }
                }
                if (Spheric)
                {
                    basin.Delta_g_meridian = basin.Delta_g_traverse = 0;
                    if (full && WithRelief)
                    {
                        var diff = Earth2014Manager.Radius2Add - basin.RadiusOfEllipse;
                        basin.Depth += diff;
                        basin.Hoq -= diff;
                    }
                    basin.InitROfEllipse(HealpixManager);
                }
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = PixMan.Pixels[HealpixManager.Neighbors.Get(to, basin)];
                    basin.Neighbors[to] = toBasin;

                    basin.Opposites[(int)to] = basin.GetFromAndFillType(to, toBasin, HealpixManager);
                }

                InitMetrics(basin);
            }
            _q3_RayDistance /= 4 * HealpixManager.Npix;

            if (full && WithRelief)
            {
                CheckOcean();
            }
        }

        public virtual void InitMetrics(T basin)
        {
            foreach (Direction to in Enum.GetValues(typeof(Direction)))
            {
                var meanBoundary = HealpixManager.Neighbors.MeanBoundary(basin, to);
                switch (basin.MetricType)
                {
                    case MetricType.IntersectionRay:
                        var intersection = basin.S_geiod.IntersectionWith(basin.Neighbors[to].S_geiod)
                            .IntersectionWith(HealpixManager.Neighbors.Boundary(basin, to));
                        if (intersection.HasValue) /* true always, why? */
                            basin.MetricRays[(int)to] = new Ray3D(Basin3.O3, intersection.Value.ToVector3D());
                        else
                            basin.MetricRays[(int)to] = meanBoundary;
                        break;
                    case MetricType.MeanEdge:
                        basin.MetricRays[(int)to] = meanBoundary;
                        break;
                    case MetricType.Edge:
                        basin.MetricRays[(int)to] = HealpixManager.Neighbors.BoundaryRay(basin, (Compass)to);
                        break;
                }
            }

            /// CorrectionSurface();
            for (int to = 0; to < 4; to++)
            {
                basin.HtoBase[to] = basin.Metric(to, true);

                var q3_RayDistance = new Line3D(basin.Q3, basin.S_geiod.IntersectionWith(basin.MetricRays[to])).Length;
                basin.Q3ToMetricRay[to] = q3_RayDistance;
                _q3_RayDistance += q3_RayDistance;
            }
        }

        public override void GradientAndHeightCrosses()
        {
            // todo SignedDistance may be accelerated twice by calcing Hto for neighbors only once; take basin.Volumes into account 
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();
                
                /* should be commented if (basin.HasWater()
                 * because solid contour may get water */
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neighbors[to];
                    var hto = basin.Metric(toBasin, to);
                    basin.Hto[to] = hto;
                }
            }
        }

        public override double? GetAltitude(T basin)
        {
            if (basin.HasWater())
            {
                for (int to = 0; to < 4; to++)
                {
                    var toBasin = basin.Neighbors[to];

                    // null for maps that only visualize
                    if (toBasin != null)
                    {
                        var from = basin.Opposites[to];

                        /* bug http://hist.tk/ory/Искажение_начала_перетекания 
                         * may be fixed by balancing deltaH (of BasinAbstract.WaterIn method) relative to basin.WaterHeight */
                        var height = basin.Hto[to] - toBasin.Hto[from];

                        Water.PutV(
                            basin,
                            toBasin,
                            height * WaterModel.Koef, // * (basin.Q3ToMetricRay[to] / _q3_RayDistance) 
                            to,
                            from);
                    }
                }
                return Visual(basin); // basin.hOQ;
            }
            return null;
        }
    }
}