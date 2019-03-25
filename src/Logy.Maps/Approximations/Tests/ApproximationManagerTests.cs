#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.Maps.Projections.Healpix.Tests;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using NUnit.Framework;

namespace Logy.Maps.Approximations.Tests
{
    [TestFixture]
    public class ApproximationManagerTests
    {
        [Test]
        public void GetMeanAltitude()
        {
            var man = new ApproximationManager(new HealpixManager(0));
            foreach (var pixel in man.Pixels)
            {
                pixel.Altitude = pixel.P;
            }
            Assert.AreEqual(4, man.GetMeanAltitude(new Coor {X = 90, Y = 0}));
            Assert.AreEqual(5, man.GetMeanAltitude(new Coor {X = 0, Y = 0}));

            Assert.AreEqual(1.5, man.GetMeanAltitude(new Coor {X = 0, Y = 70}));

            Assert.AreEqual(1.5, man.GetMeanAltitude(new Coor {X = 0, Y = 90}), .02);
            Assert.AreEqual(0, man.GetMeanAltitude(new Coor {X = 135, Y = 90}));
            Assert.AreEqual(0, man.GetMeanAltitude(new Coor {X = 135, Y = 80}));
            Assert.AreEqual(5.5, man.GetMeanAltitude(new Coor {X = 135, Y = 0}));

            Assert.AreEqual(0, man.GetMeanAltitude(new Coor {X = 135, Y = HealpixManagerTests.L41}), .1);
            Assert.AreEqual(1.3, man.GetMeanAltitude(new Coor {X = 135, Y = 10}), .1);
            Assert.AreEqual(1.8, man.GetMeanAltitude(new Coor {X = 135, Y = 1}), .1);

            Assert.AreEqual(3.6, man.GetMeanAltitude(new Coor {X = 0, Y = HealpixManagerTests.L41}), .1);

            // vertical border at left
            Assert.AreEqual(9.5, man.GetMeanAltitude(new Coor {X = 0, Y = -80}));
            Assert.AreEqual(9.9, man.GetMeanAltitude(new Coor {X = -170, Y = -80}), .1);
            Assert.AreEqual(11, man.GetMeanAltitude(new Coor {X = -135, Y = -89}), .1);
        }

        [Test]
        public void GetMeanDeltas()
        {
            var man = new ApproximationManager(new HealpixManager(3));
            Assert.AreEqual(7, man.GetMeanDeltas(new Coor { X = 0, Y = 41.8 }), 1);
            Assert.AreEqual(7, man.GetMeanDeltas(new Coor { X = 0, Y = 42 }), 1);

            Assert.AreEqual(12, man.GetMeanDeltas(new Coor { X = 0, Y = 60 }), 1);
            Assert.AreEqual(9, man.GetMeanDeltas(new Coor { X = 0, Y = 50 }), 1);
        }
    }
}
#endif
