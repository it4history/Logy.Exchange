#if DEBUG
using System.IO;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.World.Ocean;
using Newtonsoft.Json;
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

        [Test]
        public void SerializeAndDeserialize()
        {
            var man = new HealpixManager(4);
            var data = new BasinData(man)
            {
                WithRelief = true,
                IntegrationEndless = true,
            };
            data.Init();

            string json5 = null, json7 = null;

            var algorithm = new ShiftAxis(data) { Slow = true };
            var bundle = new Bundle<Basin3>(algorithm);
            algorithm.Shift(
                8,
                () => 2,
                delegate(int frame)
                {
                    if (frame == 5)
                        json5 = bundle.Serialize();
                    if (frame == 7)
                        json7 = bundle.Serialize();
                });

            var bundle5 = Bundle<Basin3>.Deserialize(json5);
            var algorithm5 = bundle5.Algorithm as ShiftAxis;
            algorithm5.Data.Frame++;
            algorithm5.Shift(8, () => 2);

            Assert.AreEqual(json7, bundle5.Serialize());
        }
    }
}
#endif
