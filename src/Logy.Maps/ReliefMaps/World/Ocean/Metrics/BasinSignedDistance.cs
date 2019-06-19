namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics
{
    public class BasinSignedDistance : Basin3
    {
        public override double Metric(Basin3 toBasin, int to, bool initial = false)
        {
            return
                -S_q.SignedDistanceTo(toBasin.Q3) /// bad for BasinDataTests.HighBasin_31_sphere 
                /// S_q.IntersectionWith(toBasin.RadiusRay).DistanceTo(toBasin.Q3)
                - HtoBase[to]; /// needed for BasinDataTests.HighBasin_31 movedBasins
        }
    }
}