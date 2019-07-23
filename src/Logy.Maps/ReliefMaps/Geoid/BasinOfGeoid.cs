using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.Exchange.Earth2014;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class BasinOfGeoid : Basin3, IDisposable
    {
        private readonly Earth2014Manager _reliefMask;

        public BasinOfGeoid()
        {
             _reliefMask = new Earth2014Manager(ReliefType.Mask, 1);
        }

        public double GeoidRadius { get; set; }
        public Polygon Polygon { get; set; }

        public Plane GeoidSurfaceForSolid { get; private set; }

        public MaskType Mask { get; set; }

        public void Dispose()
        {
             _reliefMask.Dispose();
        }

        public override void InitMetrics(NeighborManager neibors)
        {
            Mask = (MaskType)_reliefMask.GetAltitude(this);
            if (Mask == MaskType.OceanBathymetry && Hoq > 0)
            {
                // Earth2014 correction described at Logy.Maps.ReliefMaps.World.Earth2014Correction
                Hoq = 0; /// or Depth = -Hoq;
            }
            base.InitMetrics(neibors);
        }

        public bool FillNewGeoid(WaterModel model)
        {
            var froms = new Dictionary<int, BasinOfGeoid>();
            for (var from = 0; from < 4; from++)
            {
                var fromBasin1 = (BasinOfGeoid)Neibors[from];
                if (fromBasin1.Polygon != null && fromBasin1.P < P)
                {
                    froms.Add(from, fromBasin1);
                }
            }

            var water = froms.FirstOrDefault(
                    f => f.Value.Polygon.SurfaceType == SurfaceType.Water);

            var fromWater = water.Value;
            var fromWaterTo = Opposites[water.Key];
            if (HasWater()
                && (Mask == MaskType.OceanBathymetry || Mask == MaskType.LakeAbove || Mask == MaskType.LakeBelow))
            {
                if (fromWater != null)
                {
                    var height = Metric(water.Key) - fromWater.Metric(fromWaterTo);

                    // if border of this. inner basin is thiner than a pixel
                    if (height > 0 && WaterHeight < height * 2)
                    {
                        // this. basin is higher that fromWater
                        SetGeoid();

                        // todo look on other water maybe this basin should be merged
                    }
                    else if (height < 0 && fromWater.WaterHeight < -height * 2)
                    {
                        // this. basin is lower that fromWater
                        SetGeoid();

                        // todo look on other water maybe this basin should be merged
                    }
                    else
                    {
                        /* rivers have slope therefore Threshhold is multiplied * 1.5; */
                        var threshholded = Math.Abs(height) * WaterModel.Koef > model.Threshhold;
                        if (threshholded)
                        {
                            /*var diff = WaterHeight - fromWater.WaterHeight;
                            if (height > 0 && diff > 0 && height < diff)
                            {
                                // fromWater is a thin edge-shore of this. basin
                            }
                            else if (height < 0 && diff < 0 && -height < -diff)
                            {
                                // this. basin is a thin edge-shore of fromWater
                            }
                            else*/
                                return false;
                        }

                        // check whether several polygon should be merged
                        SetGeoid(fromWater.Polygon);
                    }
                }
                else
                {
                    // check inner
                    SetGeoid();
                }
            }
            else
            {
                var solid = froms.FirstOrDefault(
                    f => f.Value.Polygon.SurfaceType == SurfaceType.Solid);
                var fromSolid = solid.Value;
                if (fromSolid == null)
                {
                    var diff = HtoBase[water.Key] - fromWater.HtoBase[fromWaterTo];
                    var ray = fromWater.MeanEdges[fromWaterTo];
                    SetGeoid(null, fromWater.S_q.IntersectionWith(ray) + (diff * ray.Direction));
                }
                else
                {
                    var fromSolidTo = Opposites[solid.Key];
                    var diff = HtoBase[solid.Key] - fromSolid.HtoBase[fromSolidTo];
                    var ray = fromSolid.MeanEdges[fromSolidTo];
                    SetGeoid(
                        fromSolid.Polygon,
                        fromSolid.GeoidSurfaceForSolid.IntersectionWith(ray) + (diff * ray.Direction));
                }
            }
            return true;
        }

        public void SetGeoid(Polygon polygon = null, Point3D? solidPoint = null)
        {
            if (solidPoint != null)
            {
                GeoidSurfaceForSolid = new Plane(Normal.Value, solidPoint.Value);
                var q_geoid = GeoidSurfaceForSolid.IntersectionWith(RadiusRay);
                GeoidRadius = q_geoid.DistanceTo(O3);
            }
            else
            {
                GeoidRadius = Radius;
            }

            Polygon = polygon ?? new Polygon
            {
                SurfaceType = HasWater() ? SurfaceType.Water : SurfaceType.Solid
            };
        }
    }
}