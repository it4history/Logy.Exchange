#if DEBUG
using Logy.Maps.ReliefMaps.World;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Dem.Tests
{
    [TestFixture]
    public class HealDemManagerTests
    {
        [Test]
        public void CalcDem()
        {
            Assert.AreEqual(new[,] {{6, 1}, {14, 7}}, new HealDemManager(1).CalcDem(1));
            Assert.AreEqual(new[,] {{8, 2}, {16, 9}}, new HealDemManager(1).CalcDem(2));

            Assert.AreEqual(
                new[,]
                {
                    {28, 15, 6, 1},
                    {44, 29, 16, 7},
                    {61, 45, 30, 17},
                    {77, 62, 46, 31}
                },
                new HealDemManager(2).CalcDem(1));

            Assert.AreEqual(
                new[,]
                {
                    {94, 78, 63, 47},
                    {110, 95, 79, 64},
                    {127, 111, 96, 80},
                    {143, 128, 112, 97}
                },
                new HealDemManager(2).CalcDem(5));
        }

        [Test]
        public void CalcDem_ParentP()
        {
            var man = new HealDemManager(2);
            man.CalcDem(1, 0);
            foreach (var basin in man.GetNewBasins())
            {
                Assert.AreEqual(1, basin.ParentP);
            }

            man.CalcDem(1, 1);
            var basins = man.GetNewBasins();
            Assert.AreEqual(6, basins[0].ParentP);
            Assert.AreEqual(1, basins[2].ParentP);
            Assert.AreEqual(6, basins[4].ParentP);
            Assert.AreEqual(1, basins[6].ParentP);
            Assert.AreEqual(14, basins[8].ParentP);
            Assert.AreEqual(7, basins[10].ParentP);
        }

        [Test]
        public void AraratDem()
        {
            var dem = new HealDemManager(11, 5).CalcDem(Ararat.P);
            Assert.AreEqual(
                64 * 64,
                dem.Length);
        }


        [Test]
        public void GetCurvatureCenter()
        {
            var man = new HealDemManager(2);
            man.CalcDem(1);

            var kidsData = new DemData(man.KidsMan, man.GetNewBasins());
            kidsData.Init();
            Assert.AreEqual(
                new Plane(0.5716, 0.495, 0.6544, -6252278.4642)
                //for other order in _dem for u      new Plane(0.571, 0.5452, 0.6137, -6243679.1905)
                    .ToString(),
                man.GetCurvatureCenter(kidsData).ToString());
        }
    }
}
#endif