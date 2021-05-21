#if DEBUG
using Logy.Maps.Exchange;
using Logy.Maps.Geometry;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefParadiseEnd : ReliefParadise
    {
        public ReliefParadiseEnd() : base(5)
        {
            MainAlgorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            })
            {
                Geoisostasy = true,
                DesiredDatum = Datum.Strahov48
            };
        }
    }
}
#endif