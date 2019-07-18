using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.Water;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class BasinOfGeoid : Basin3
    {
        public double GeoidRadius { get; set; }
        public Polygon Polygon { get; set; }

        public Plane GeoidSurfaceForSolid { get; private set; }

        /*public Ray3D[] GeoidRays { get; set; }

        public override void InitMetrics()
        {
            base.InitMetrics();
        }*/

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
            if (HasWater())
            {
                if (fromWater != null)
                {
                    var height = Metric(water.Key) - fromWater.Metric(fromWaterTo);
                    var threshholded = Math.Abs(height) * WaterModel.Koef > model.Threshhold;
                    if (threshholded)
                        return false;
                    
                    // check whether several polygon should be merged
                    SetGeoid(fromWater.Polygon);
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
                if (P == 8)
                {
                }
                if (fromSolid == null)
                {
                    var diff = HtoBase[water.Key] - fromWater.HtoBase[fromWaterTo];
                    var ray = fromWater.MeanEdges[fromWaterTo];
                    SetGeoid(null, fromWater.S_q.IntersectionWith(ray) + (diff * ray.Direction));
                }
                else
                {
                    var fromSolidTo = Opposites[solid.Key];
                    SetGeoid(
                        fromSolid.Polygon, 
                        fromSolid.GeoidSurfaceForSolid.IntersectionWith(fromSolid.MeanEdges[fromSolidTo]));
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