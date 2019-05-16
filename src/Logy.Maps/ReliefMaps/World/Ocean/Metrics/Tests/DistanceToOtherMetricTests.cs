#if DEBUG
using System;
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Metrics.Tests
{
    [TestFixture]
    public class DistanceToOtherMetricTests
    {
        [Test]
        public void AbsoluteDistanceTo_Metric()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin3>(0);
            Assert.AreEqual(0, basin.Intersect(basin));
            Assert.AreEqual(0, basin.S_q.AbsoluteDistanceTo(basin.Q3));
            var toBasin = man.GetCenter<Basin3>(1);
            var from0to1 = basin.S_q.AbsoluteDistanceTo(toBasin.Q3);
            var from1to0 = toBasin.S_q.AbsoluteDistanceTo(basin.Q3);
            Assert.AreEqual(from1to0, from0to1, 1);
            basin = man.GetCenter<Basin3>(4);
            toBasin = man.GetCenter<Basin3>(7);
            Assert.AreEqual(
                basin.S_q.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.S_q.AbsoluteDistanceTo(basin.Q3), 
                .0001);

            man = new HealpixManager(1);
            basin = man.GetCenter<Basin3>(0);
            toBasin = man.GetCenter<Basin3>(4);

            // basin.SetKQQaxis(0, man);
            // toBasin.SetKQQaxis(0, man);
            Assert.AreEqual(
                basin.S_q.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.S_q.AbsoluteDistanceTo(basin.Q3), 
                1100); //// .0001 better
        }

        [Test]
        public void Intersect_2()
        {
            var data = new BasinData(new HealpixManager(2)) { Spheric = true };
            data.OnInit();
            foreach (var basin in data.PixMan.Pixels)
            {
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var toBasin = basin.Neibors[to];
                    var from0to1 = basin.S_q.AbsoluteDistanceTo(toBasin.Q3);
                    var from1to0 = toBasin.S_q.AbsoluteDistanceTo(basin.Q3);
                    Assert.AreEqual(from1to0, from0to1, from1to0 / 100);
                }
            }
        }
    }
}
#endif