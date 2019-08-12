using Logy.Maps.ReliefMaps.World.Ocean;

namespace Logy.Maps.Metrics
{
    public class SignedDistanceBasin : Basin3
    {
        public override double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            return
                -S_q.SignedDistanceTo(toBasin.Q3) /// bad for OceanDataTests.HighBasin_31_sphere 
                /// RadiusIntersection metric:   S_q.IntersectionWith(toBasin.RadiusRay).DistanceTo(toBasin.Q3) 
                - HtoBase[to]; /// needed for OceanDataTests.HighBasin_31 movedBasins
        }
    }
}