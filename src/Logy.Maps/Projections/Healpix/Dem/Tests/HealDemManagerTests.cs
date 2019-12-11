#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Dem.Tests
{
    [TestFixture]
    public class HealDemManagerTests
    {
        [Test]
        public void GetDem()
        {
            Assert.AreEqual(new[,] { { 1, 6 }, { 7, 14 } }, new HealDemManager(1).GetDem(1));
            Assert.AreEqual(new[,] { { 2, 8 }, { 9, 16 } }, new HealDemManager(1).GetDem(2));

            Assert.AreEqual(new[,]
                {
                    { 1, 6, 15, 28 },
                    { 7, 16, 29, 44 },
                    { 17, 30, 45, 61 },
                    { 31, 46, 62, 77 }
                },
                new HealDemManager(2).GetDem(1));
            Assert.AreEqual(new[,]
                {
                    { 47, 63, 78, 94 },
                    { 64, 79, 95, 110 },
                    { 80, 96, 111, 127 },
                    { 97, 112, 128, 143 }
                },
                new HealDemManager(2).GetDem(5));
        }
    }
}
#endif