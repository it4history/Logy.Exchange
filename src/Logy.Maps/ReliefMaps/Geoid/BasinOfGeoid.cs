using System;
using System.Collections.Generic;
using System.Linq;
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Spatial.Euclidean;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class BasinOfGeoid : Basin3
    {
        public double GeoidRadius { get; set; }
        public Polygon Polygon { get; set; }

        public Plane GeoidSurfaceForSolid { get; private set; }

        public bool FillNewGeoid(double waterThreshhold)
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
                    var height = Hto[water.Key] - fromWater.Hto[fromWaterTo];
                    var threshholded = Math.Abs(height) > waterThreshhold;
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
                if (fromSolid == null)
                {
                    SetGeoid(null, fromWater.S_q.IntersectionWith(fromWater.MeanEdges[fromWaterTo]));
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
                GeoidRadius = GeoidSurfaceForSolid.IntersectionWith(RadiusRay).DistanceTo(O3);
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