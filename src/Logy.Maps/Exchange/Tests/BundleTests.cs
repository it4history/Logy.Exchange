#if DEBUG
using System.Reflection;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Exchange.Tests
{
    [TestFixture]
    public class BundleTests
    {
        private static readonly string Expected =
            @"{""Algorithms"":[{""Slow"":true" +
            @",""Data"":{""WithRelief"":true,""Spheric"":false,""MetricType"":0,""IntegrationEndless"":false,""SamePolesAndEquatorGravitation"":false,""Frame"":-1,""Time"":0,""TimeStep"":1,""Max"":null,""Min"":null,""K"":4,""Accuracy"":5,""Dimension"":""m""}" +
            @",""Poles"":{""-1"":{""SiderealDayInSeconds"":86164.100637,""X"":-180.0,""Y"":90.0}}" +
            $@",""Name"":""Logy.Maps.Exchange.ShiftAxis, Logy.Maps, Version={Assembly.GetExecutingAssembly().GetName().Version}, Culture=neutral, PublicKeyToken=null"",""Diff"":0.0" +
            @"}],""Basins"":{""4"":[{""Hoq"":0.0,""Depth"":2371.0}]}}";
            
        [Test]
        public void Serialize()
        {
            var data = new OceanData(new HealpixManager(4)) { WithRelief = true };
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
            var bundle = Bundle<Basin3>.Deserialize(Expected, true);

            Assert.AreEqual(Expected, bundle.Serialize());
            Assert.AreEqual(typeof(ShiftAxis), bundle.Algorithm.GetType());
            Assert.IsTrue(bundle.Algorithm.DataAbstract.WithRelief);
        }

        [Test]
        public void SerializeAndDeserialize()
        {
            var man = new HealpixManager(2);
            var data = new OceanData(man)
            {
                WithRelief = true,
                IntegrationEndless = true,
            };
            data.Init();

            var algorithm = new ShiftAxis(data) { Slow = true };
            var bundle = new Bundle<Basin3>(algorithm);
            string json5 = null;
            algorithm.Shift(
                6,
                (frame) => 2,
                delegate(int frame)
                {
                    if (frame == 5)
                        json5 = bundle.Serialize();
                });
            var jsonLast = bundle.Serialize();

            var bundle5 = Bundle<Basin3>.Deserialize(json5);
            Assert.AreEqual(json5, bundle5.Serialize());
            foreach (var basin in bundle.Algorithm.DataAbstract.PixMan.Pixels)
            {
                var basin5 = bundle.Algorithm.DataAbstract.PixMan.Pixels[basin.P];
                Assert.IsTrue(basin.HtoBase == basin5.HtoBase &&
                              basin.Hto == basin5.Hto);
            }

            var algorithm5 = bundle5.Algorithm as ShiftAxis;
            algorithm5.Shift(6, (frame) => 2);

            var restoredAndRun = bundle5.Serialize();
            Assert.AreEqual(jsonLast, restoredAndRun);
        }
    }
}
#endif
