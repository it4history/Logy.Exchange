#if DEBUG
using Logy.Maps.Projections.Healpix;
using Logy.MwAgent.Sphere;
using NUnit.Framework;

namespace Logy.Maps.Exchange.Earth2014.Tests
{
    [TestFixture]
    public class Earth2014ManagerTests
    {
        private readonly Coor _crimeaCenter = new Coor { Y = 44.93, X = 34.099, Precision = 0.00027 };

        public static HealCoor Sevastopol { get; set; } = new HealCoor { Y = 44.6, X = 33.53 };

        [Test]
        public void GetAltitude()
        {
            var mangup = new HealCoor { Y = 44.58, X = 33.8, Precision = 0.0166 };

            using (var man = new Earth2014Manager(_crimeaCenter, 5))
            {
                Assert.IsTrue(man.GetAltitude(mangup) > 200 && man.GetAltitude(mangup) < 600);
                Assert.IsTrue(man.GetAltitude(Sevastopol) > 0 && man.GetAltitude(Sevastopol) <= 160); //// why 160?
            }
            using (var man = new Earth2014Manager(_crimeaCenter))
            {
                Assert.IsTrue(man.GetAltitude(mangup) > 200 && man.GetAltitude(mangup) < 600);
                Assert.IsTrue(man.GetAltitude(Sevastopol) > 0 && man.GetAltitude(Sevastopol) < 60);
            }
        }

        [Test]
        public void GetLevelDots()
        {
            using (var man = new Earth2014Manager(_crimeaCenter))
            {
                var l = man.GetLevelDots(-12, 1);
            }
        }

        [Test]
        public void GetPerimeter()
        {
            using (var man = new Earth2014Manager(_crimeaCenter))
            {
                man.GetPerimeter(-12);
            }
        }
    }
}
#endif
