﻿using System;
using System.Runtime.Serialization;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Geometry;
using Logy.Maps.Metrics;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Basemap
{
    public class BasinDataAbstract<T> : WaterMoving<T> where T : Basin3
    {
        public BasinDataAbstract()
        {
        }

        public BasinDataAbstract(
            HealpixManager man,
            T[] basins = null,
            double? min = null,
            double? max = null,
            bool readAllAtStart = false)
            : base(man, basins, min, max, readAllAtStart)
        {
        }

        [IgnoreDataMember]
        public override ReliefType ReliefBedType => ReliefType.Tbi;

        [IgnoreDataMember]
        public Func<T, double> Visual { get; set; } = basin => basin.Hoq; //// basin.Altitude * 1000;

        public virtual void InitMetrics(T basin)
        {
            foreach (Direction to in Enum.GetValues(typeof(Direction)))
            {
                var meanBoundary = HealpixManager.Neighbors.MeanBoundary(basin, to);
                switch (Basin3.MetricType)
                {
                    case MetricType.IntersectionRay:
                        var intersection = basin.S_geiod.IntersectionWith(basin.Neighbors[to].S_geiod)
                            .IntersectionWith(HealpixManager.Neighbors.Boundary(basin, to));
                        if (intersection.HasValue) /* why always true? */
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
                    case MetricType.HEALPixaS:
                        var toBasin = basin.Neighbors[to];
                        basin.MetricRays[(int)to] =
                            new Ray3D(Basin3.O3, basin.RadiusRay.Direction + toBasin.RadiusRay.Direction);
                        break;
                }
            }

            /// CorrectionSurface();

            for (int to = 0; to < 4; to++)
            {
                basin.Koefs[to] = WaterModel.Koef;
                basin.HtoBase[to] = basin.Metric(to, true);
            }
        }

        /// <param name="full">if false then Depth, Hoq are not set</param>
        public override void Init(bool full = true)
        {
            base.Init(full);

            ColorsMiddle = 0;

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

            if (full && WithFormattor)
            {
                // HtoBase was calculated without formatting
                var sphericData = new BasinDataAbstract<T>(HealpixManager) { Spheric = true };
                sphericData.Init(false);
                sphericData.GradientAndHeightCrosses();
                var equations = new HealpixFormattor<T>(sphericData).Format();
                foreach (var basin in PixMan.Pixels)
                {
                    for (int to = 0; to < 4; to++)
                    {
                        basin.Koefs[to] *= equations.GetResult(basin, to);
                    }
                }
            }

            if (full && WithRelief)
            {
                CheckOcean();
            }
        }

        public override void GradientAndHeightCrosses()
        {
            // todo SignedDistance may be accelerated twice by calcing Hto for neighbors only once; take basin.Volumes into account 
            foreach (var basin in PixMan.Pixels)
            {
                basin.WaterReset();

                /* if (basin.HasWater() should be commented
                 * because solid contour may get water */
                for (int to = 0; to < 4; to++)
                {
                    basin.Hto[to] = basin.Metric(to);
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
                    var from = basin.Opposites[to];
                    var height = GetBasinHeight(basin, toBasin, to, from);

                    Water.PutV(
                        basin,
                        toBasin,
                        height * basin.Koefs[to],
                        to,
                        from);
                }
                return Visual(basin); // basin.hOQ;
            }
            return null;
        }

        internal double GetBasinHeight(Basin3 basin, int to)
        {
            var toBasin = basin.Neighbors[to];
            var from = basin.Opposites[to];
            return GetBasinHeight(basin, toBasin, to, from);
        }

        protected double GetBasinHeight(Basin3 basin, Basin3 toBasin, int to, int from)
        {
            // null for maps that only visualize
            if (toBasin == null)
                return 0;

            switch (Basin3.MetricType)
            {
                case MetricType.Edge:
                    Compass sameRingCompass, sameRingCompass2;
                    var compass = NeighborManager.Compasses((Direction)to, out sameRingCompass);
                    var compass2 = NeighborManager.Compasses((Direction)from, out sameRingCompass2);
                    return (basin.Hto[(int)compass] + basin.Hto[(int)sameRingCompass]
                            - toBasin.Hto[(int)compass2] - toBasin.Hto[(int)sameRingCompass2]) * .5;
                default:
                    /* bug http://hist.tk/ory/Искажение_начала_перетекания 
                     * may be fixed by balancing deltaH (of BasinAbstract.WaterIn method) relative to basin.WaterHeight */
                    return basin.Hto[to] - toBasin.Hto[from];
            }
        }
    }
}