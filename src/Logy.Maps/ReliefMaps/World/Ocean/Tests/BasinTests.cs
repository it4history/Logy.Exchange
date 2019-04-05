#if DEBUG
using Logy.Maps.Projections.Healpix;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean.Tests
{
    [TestFixture]
    public class BasinTests
    {
        [Test]
        public void Basin_Surface_AbsoluteDistanceTo_METRIC()
        {
            var man = new HealpixManager(0);
            var basin = man.GetCenter<Basin3>(0);
            Assert.AreEqual(0, basin.Intersect(basin));
            Assert.AreEqual(0, basin.Surface.AbsoluteDistanceTo(basin.Q3));
            var toBasin = man.GetCenter<Basin3>(1);
            var from0to1 = basin.Surface.AbsoluteDistanceTo(toBasin.Q3);
            var from1to0 = toBasin.Surface.AbsoluteDistanceTo(basin.Q3);
            Assert.AreEqual(from1to0, from0to1, 1);
            basin = man.GetCenter<Basin3>(4);
            toBasin = man.GetCenter<Basin3>(7);
            Assert.AreEqual(
                basin.Surface.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3), 
                .0001);

            man = new HealpixManager(1);
            basin = man.GetCenter<Basin3>(0);
            toBasin = man.GetCenter<Basin3>(4);

            // basin.SetKQQaxis(0, man);
            // toBasin.SetKQQaxis(0, man);
            Assert.AreEqual(
                basin.Surface.AbsoluteDistanceTo(toBasin.Q3),
                toBasin.Surface.AbsoluteDistanceTo(basin.Q3), 
                .0001);
        }
    }
}
#endif