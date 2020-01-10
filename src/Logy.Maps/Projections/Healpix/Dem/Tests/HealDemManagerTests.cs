#if DEBUG
using Logy.Maps.ReliefMaps.World;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Dem.Tests
{
    [TestFixture]
    public class HealDemManagerTests
    {
        [Test]
        public void CalcDem()
        {
            Assert.AreEqual(new[,] { { 1, 6 }, { 7, 14 } }, new HealDemManager(1).CalcDem(1));
            Assert.AreEqual(new[,] { { 2, 8 }, { 9, 16 } }, new HealDemManager(1).CalcDem(2));

            Assert.AreEqual(
                new[,]
                {
                    { 1, 6, 15, 28 },
                    { 7, 16, 29, 44 },
                    { 17, 30, 45, 61 },
                    { 31, 46, 62, 77 }
                },
                new HealDemManager(2).CalcDem(1));

            Assert.AreEqual(
                new[,]
                {
                    { 47, 63, 78, 94 },
                    { 64, 79, 95, 110 },
                    { 80, 96, 111, 127 },
                    { 97, 112, 128, 143 }
                },
                new HealDemManager(2).CalcDem(5));
        }

        [Test]
        public void CalcDem_ParentP()
        {
            var man = new HealDemManager(2);
            man.CalcDem(1, 0);
            foreach (var basin in man.Basins)
            {
                Assert.AreEqual(1, basin.ParentP);
            }

            man.CalcDem(1, 1);
            Assert.AreEqual(1, man.Basins[0].ParentP);
            Assert.AreEqual(6, man.Basins[2].ParentP);
            Assert.AreEqual(1, man.Basins[4].ParentP);
            Assert.AreEqual(6, man.Basins[6].ParentP);
            Assert.AreEqual(7, man.Basins[8].ParentP);
            Assert.AreEqual(14, man.Basins[10].ParentP);
        }

        [Test]
        public void AraratDem()
        {
            var dem = new HealDemManager(11, 5).CalcDem(Ararat.P);
            Assert.AreEqual(
                64 * 64,
                dem.Length);
        }
    }
}
#endif