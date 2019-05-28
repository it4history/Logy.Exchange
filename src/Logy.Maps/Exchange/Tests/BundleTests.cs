#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Exchange.Tests
{
    [TestFixture]
    public class BundleTests
    {
        private static readonly string Expected =
            @"{""Algorithms"":[{""Slow"":true,""Poles"":{""-1"":{""X"":0.0,""Y"":90.0,""SiderealDayInSeconds"":null}}" +
            @",""Data"":{""WithRelief"":true,""Spheric"":false,""Frame"":0,""Time"":0,""TimeStep"":1,""Max"":null,""Min"":null,""K"":4,""Accuracy"":5,""Dimension"":""m""}" +
            $@",""Name"":""Logy.Maps.Exchange.ShiftAxis, Logy.Maps, Version={Global.Version}, Culture=neutral, PublicKeyToken=null"",""Diff"":0.0" +
            @"}],""Basins"":{""4"":[{""Hoq"":0.0,""Depth"":2371.0,""P"":0}]}}";
            
        [Test]
        public void Serialize()
        {
            var data = new BasinData(new HealpixManager(4)) { WithRelief = true };
            data.Init();
            var bundle = new Bundle<Basin3>(new ShiftAxis(data)
            {
                Slow = true
            })
            {
                Basins = { [4] = new[] { data.PixMan.Pixels[0] } },
            };

            Assert.AreEqual(Expected, bundle.Serialize());
        }

        [Test]
        public void Deserialize()
        {
            var bundle = Bundle<Basin3>.Deserialize(Expected);

            Assert.AreEqual(Expected, bundle.Serialize());
            Assert.AreEqual(typeof(ShiftAxis), bundle.Algorithm.GetType());
            Assert.IsTrue(bundle.Algorithm.DataAbstract.WithRelief);
        }
    }
}
#endif
