#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Exchange.Tests
{
    [TestFixture]
    public class BundleTests
    {
        [Test]
        public void Serialize()
        {
            var data = new BasinData(new HealpixManager(6));
            var bundle = new Bundle<Basin3>(new ChangeAxis(data))
            {
                Basins = { [6] = new[] { data.PixMan.Pixels[0] } }
            };

            Assert.AreEqual(
                @"{""Algorithms"":[{""Slow"":false,""Poles"":{""-1"":{""X"":0.0,""Y"":90.0}},""Data"":{""HealpixManager"":{""K"":6},""Frame"":-1,""Time"":-1,""Accuracy"":5},""Name"":""ChangeAxis"",""Diff"":0.0}],""Basins"":{""6"":[{""hOQ"":0.0,""Depth"":null,""P"":0}]}}",
                bundle.Serialize());
        }
    }
}
#endif
