#if DEBUG
using System;
using Logy.Maps.Approximations;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Tests
{
    [TestFixture]
    public class NeighborManagerTests
    {
        private NeighborManager man;
        private HealpixManager healpixManager; /*very temp*/

        public Basin3 Center(int p)
        {
            return healpixManager.GetCenter<Basin3>(p);
        }

        public NeighborManager GetMan(int k)
        {
            healpixManager = new HealpixManager(k);
            return new NeighborManager(healpixManager);
        }

        [SetUp]
        public void SetUp()
        {
            man = GetMan(1);
        }

        [Test]
        public void GetVert()
        {
            Assert.AreEqual(NeighborVert.North, NeighborManager.GetVert(Direction.Ne));
            Assert.AreEqual(NeighborVert.South, NeighborManager.GetVert(Direction.Sw));
        }

        [Test]
        public void GetHor()
        {
            Assert.AreEqual((int)NeighborHor.East, NeighborManager.GetHor(Direction.Ne));
            Assert.AreEqual((int)NeighborHor.West, NeighborManager.GetHor(Direction.Sw));
        }

        [Test]
        public void NorthMean()
        {
            for (var p = 0; p <= 6; p++)
                Assert.AreEqual(0, man.NorthMean(Center(p)));
            Assert.AreEqual(1, man.NorthMean(Center(7)));
            Assert.AreEqual(1, man.NorthMean(Center(8)));
            Assert.AreEqual(2, man.NorthMean(Center(9)));
        }

        [Test]
        public void NorthEast_0()
        {
            man = GetMan(0);
            Assert.AreEqual(3, man.NorthEast(Center(0)));
            Assert.AreEqual(0, man.NorthEast(Center(1)));
            Assert.AreEqual(0, man.NorthEast(Center(4)));
            Assert.AreEqual(1, man.NorthEast(Center(5)));
            Assert.AreEqual(2, man.NorthEast(Center(6)));
            Assert.AreEqual(3, man.NorthEast(Center(7)));

            Assert.AreEqual(7, man.NorthEast(Center(8)));
            Assert.AreEqual(4, man.NorthEast(Center(9)));
            Assert.AreEqual(5, man.NorthEast(Center(10)));
            Assert.AreEqual(6, man.NorthEast(Center(11)));
        }

        [Test]
        public void NorthEast_1()
        {
            Assert.AreEqual(3, man.NorthEast(Center(0)));
            Assert.AreEqual(0, man.NorthEast(Center(1)));
            Assert.AreEqual(1, man.NorthEast(Center(2)));
            Assert.AreEqual(11, man.NorthEast(Center(4)));
            Assert.AreEqual(0, man.NorthEast(Center(5)));
            Assert.AreEqual(5, man.NorthEast(Center(6)));
            Assert.AreEqual(1, man.NorthEast(Center(7)));

            Assert.AreEqual(6, man.NorthEast(Center(14)));

            Assert.AreEqual(4, Center(25).Ring);
            Assert.AreEqual(19, man.NorthEast(Center(20)));
            Assert.AreEqual(14, man.NorthEast(Center(23)));
            Assert.AreEqual(15, man.NorthEast(Center(24)));

            Assert.AreEqual(5, Center(34).Ring);
            Assert.AreEqual(22, man.NorthEast(Center(30)));
            Assert.AreEqual(23, man.NorthEast(Center(31)));

            Assert.AreEqual(6, Center(43).Ring);

            Assert.AreEqual(7, Center(47).Ring);
            Assert.AreEqual(36, man.NorthEast(Center(44)));
            Assert.AreEqual(38, man.NorthEast(Center(45)));
            Assert.AreEqual(40, man.NorthEast(Center(46)));
            Assert.AreEqual(42, man.NorthEast(Center(47)));

            Assert.AreEqual(35, man.NorthEast(Center(36)));
            Assert.AreEqual(28, man.NorthEast(Center(37)));
            Assert.AreEqual(34, man.NorthEast(Center(43)));
        }

        [Test]
        public void NorthEast_2()
        {
            man = GetMan(2);
            Assert.AreEqual(11, man.NorthEast(Center(4)));
            Assert.AreEqual(0, man.NorthEast(Center(5)));

            Assert.AreEqual(23, man.NorthEast(Center(12)));
            Assert.AreEqual(4, man.NorthEast(Center(13)));
            Assert.AreEqual(5, man.NorthEast(Center(14)));
            Assert.AreEqual(14, man.NorthEast(Center(15)));
            Assert.AreEqual(6, man.NorthEast(Center(16)));

            Assert.AreEqual(4, Center(25).Ring);
            Assert.AreEqual(39, man.NorthEast(Center(24)));
            Assert.AreEqual(12, man.NorthEast(Center(25)));
            Assert.AreEqual(13, man.NorthEast(Center(26)));
            Assert.AreEqual(14, man.NorthEast(Center(27)));
            Assert.AreEqual(27, man.NorthEast(Center(28)));

            Assert.AreEqual(17, man.NorthEast(Center(31)));
            Assert.AreEqual(30, man.NorthEast(Center(46)));
            Assert.AreEqual(31, man.NorthEast(Center(47)));

            Assert.AreEqual(45, man.NorthEast(Center(62)));
            Assert.AreEqual(46, man.NorthEast(Center(63)));
            Assert.AreEqual(47, man.NorthEast(Center(64)));
            Assert.AreEqual(62, man.NorthEast(Center(78)));

            Assert.AreEqual(77, man.NorthEast(Center(94)));
            Assert.AreEqual(78, man.NorthEast(Center(95)));
            Assert.AreEqual(79, man.NorthEast(Center(96)));
            Assert.AreEqual(94, man.NorthEast(Center(110)));
            Assert.AreEqual(95, man.NorthEast(Center(111)));
            Assert.AreEqual(96, man.NorthEast(Center(112)));
            Assert.AreEqual(97, man.NorthEast(Center(113)));

            Assert.AreEqual(110, man.NorthEast(Center(127)));

            Assert.AreEqual(142, man.NorthEast(Center(159)));

            Assert.AreEqual(166, man.NorthEast(Center(179)));
            Assert.AreEqual(168, man.NorthEast(Center(180)));

            Assert.AreEqual(178, man.NorthEast(Center(187)));

            Assert.AreEqual(186, man.NorthEast(Center(191)));
        }

        [Test]
        public void NorthWest_0()
        {
            man = GetMan(0);
            Assert.AreEqual(1, man.NorthWest(Center(0)));
            Assert.AreEqual(2, man.NorthWest(Center(1)));
            Assert.AreEqual(0, man.NorthWest(Center(3)));
            Assert.AreEqual(1, man.NorthWest(Center(4)));
            Assert.AreEqual(2, man.NorthWest(Center(5)));
            Assert.AreEqual(3, man.NorthWest(Center(6)));
            Assert.AreEqual(0, man.NorthWest(Center(7)));

            Assert.AreEqual(4, man.NorthWest(Center(8)));
            Assert.AreEqual(5, man.NorthWest(Center(9)));
            Assert.AreEqual(6, man.NorthWest(Center(10)));
            Assert.AreEqual(7, man.NorthWest(Center(11)));
        }

        [Test]
        public void NorthWest_1()
        {
            Assert.AreEqual(1, man.NorthWest(Center(0)));
            Assert.AreEqual(0, man.NorthWest(Center(3)));

            Assert.AreEqual(0, man.NorthWest(Center(4)));
            Assert.AreEqual(6, man.NorthWest(Center(5)));
            Assert.AreEqual(1, man.NorthWest(Center(6)));
            Assert.AreEqual(8, man.NorthWest(Center(7)));
            Assert.AreEqual(2, man.NorthWest(Center(8)));

            Assert.AreEqual(15, man.NorthWest(Center(23)));

            Assert.AreEqual(37, man.NorthWest(Center(44)));
            Assert.AreEqual(39, man.NorthWest(Center(45)));
            Assert.AreEqual(41, man.NorthWest(Center(46)));
            Assert.AreEqual(43, man.NorthWest(Center(47)));
        }

        [Test]
        public void SouthWest_0()
        {
            man = GetMan(0);
            Assert.AreEqual(4, man.SouthWest(Center(0)));
            Assert.AreEqual(5, man.SouthWest(Center(1)));
            Assert.AreEqual(6, man.SouthWest(Center(2)));
            Assert.AreEqual(7, man.SouthWest(Center(3)));

            Assert.AreEqual(9, man.SouthWest(Center(4)));
            Assert.AreEqual(10, man.SouthWest(Center(5)));
            Assert.AreEqual(11, man.SouthWest(Center(6)));
            Assert.AreEqual(8, man.SouthWest(Center(7)));

            Assert.AreEqual(9, man.SouthWest(Center(8)));
            Assert.AreEqual(10, man.SouthWest(Center(9)));
            Assert.AreEqual(11, man.SouthWest(Center(10)));
            Assert.AreEqual(8, man.SouthWest(Center(11)));
        }

        [Test]
        public void SouthWest_1()
        {
            Assert.AreEqual(5, man.SouthWest(Center(0)));
            Assert.AreEqual(7, man.SouthWest(Center(1)));
            Assert.AreEqual(12, man.SouthWest(Center(4)));
            Assert.AreEqual(13, man.SouthWest(Center(5)));
            Assert.AreEqual(15, man.SouthWest(Center(7)));

            Assert.AreEqual(23, man.SouthWest(Center(14)));
            Assert.AreEqual(24, man.SouthWest(Center(15)));
            Assert.AreEqual(31, man.SouthWest(Center(23)));
        }

        [Test]
        public void SouthWest_2()
        {
            man = GetMan(2);
            Assert.AreEqual(80, man.SouthWest(Center(64)));
        }

        [Test]
        public void SouthEast_0()
        {
            man = GetMan(0);
            Assert.AreEqual(7, man.SouthEast(Center(0)));
            Assert.AreEqual(4, man.SouthEast(Center(1)));
            Assert.AreEqual(5, man.SouthEast(Center(2)));
            Assert.AreEqual(6, man.SouthEast(Center(3)));

            Assert.AreEqual(8, man.SouthEast(Center(4)));
            Assert.AreEqual(9, man.SouthEast(Center(5)));
            Assert.AreEqual(10, man.SouthEast(Center(6)));
            Assert.AreEqual(11, man.SouthEast(Center(7)));

            Assert.AreEqual(11, man.SouthEast(Center(8)));
            Assert.AreEqual(8, man.SouthEast(Center(9)));
            Assert.AreEqual(9, man.SouthEast(Center(10)));
            Assert.AreEqual(10, man.SouthEast(Center(11)));
        }

        [Test]
        public void SouthEast_1()
        {
            Assert.AreEqual(8, man.SouthEast(Center(2)));
            Assert.AreEqual(10, man.SouthEast(Center(3)));
            Assert.AreEqual(12, man.SouthEast(Center(5)));

            Assert.AreEqual(15, man.SouthEast(Center(8)));
            Assert.AreEqual(16, man.SouthEast(Center(9)));

            Assert.AreEqual(24, man.SouthEast(Center(16)));
        }

        [Test]
        public void SouthEast_2()
        {
            man = GetMan(2);
            Assert.AreEqual(18, man.SouthEast(Center(8)));
            Assert.AreEqual(32, man.SouthEast(Center(18)));
            Assert.AreEqual(47, man.SouthEast(Center(32)));

            Assert.AreEqual(63, man.SouthEast(Center(47)));

            Assert.AreEqual(78, man.SouthEast(Center(63)));
            Assert.AreEqual(79, man.SouthEast(Center(64)));

            Assert.AreEqual(111, man.SouthEast(Center(96)));
        }

        [Test]
        public void MeanBoundary_0()
        {
            man = GetMan(0);
            Assert.AreEqual(
                24, 
                Basin3.Oz.AngleTo(man.MeanBoundary(Center(0), Direction.Nw).Direction).Degrees,
                1);
            Assert.AreEqual(
                24,
                Basin3.Oz.AngleTo(man.MeanBoundary(Center(0), Direction.Ne).Direction).Degrees,
                1);
            var edge0_sw = man.MeanBoundary(Center(0), Direction.Sw).Direction;
            Assert.AreEqual(-.4, edge0_sw.X, .1);
            Assert.AreEqual(.8, edge0_sw.Y, .1);
            Assert.AreEqual(.4, edge0_sw.Z, .1);
        }
        [Test]
        public void MeanBoundary_NeiborsHaveTheSame()
        {
            var pixMan = new PixelsManager<Basin3>(healpixManager);
            foreach (var basin in pixMan.Pixels)
            {
                foreach (Direction to in Enum.GetValues(typeof(Direction)))
                {
                    var edge = man.MeanBoundary(basin, to).Direction;

                    var toBasin = pixMan.Pixels[man.Get(to, basin)];
                    var from = (Direction)basin.GetFromAndFillType(to, toBasin, healpixManager);
                    var edgeTo = man.MeanBoundary(toBasin, from).Direction;

                    Assert.AreEqual(edge.X, edgeTo.X, .0000000001);
                    Assert.AreEqual(edge.Y, edgeTo.Y, .0000000001);
                    Assert.AreEqual(edge.Z, edgeTo.Z, .0000000001);
                }
            }
        }
    }
}
#endif
