#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Tests
{
    [TestFixture]
    public class HealpixManagerTests
    {
        public const double L41 = 41.81;

        /// <summary>
        /// I(x) is the largest integer number smaller than x.
        /// </summary>
        [Test]
        public void Ix()
        {
            Assert.AreEqual(1, (int)1.99d);
            Assert.AreEqual(1, (int)1.5d);
            Assert.AreEqual(1, (int)1d);
        }

        [Test]
        public void GetP_0()
        {
            var man = new HealpixManager(0);
            Assert.AreEqual(1, man.GetP(1, 2));
            Assert.AreEqual(5, man.GetP(2, 2));
            Assert.AreEqual(10, man.GetP(3, 3));
        }
        [Test]
        public void GetP_1()
        {
            var man = new HealpixManager(1);
            Assert.AreEqual(23, man.GetP(4, 4));
        }
        [Test]
        public void GetP_2()
        {
            var man = new HealpixManager(2);
            Assert.AreEqual(31, man.GetP(4, 8));
            Assert.AreEqual(47, man.GetP(5, 8));
            Assert.AreEqual(63, man.GetP(6, 8));
            Assert.AreEqual(79, man.GetP(7, 8));
            Assert.AreEqual(77, man.GetP(7, 6));
            Assert.AreEqual(95, man.GetP(8, 8));

            Assert.AreEqual(72, man.GetP(7, 1));
            Assert.AreEqual(87, man.GetP(7, 16));
            Assert.AreEqual(88, man.GetP(8, 1));
            Assert.AreEqual(103, man.GetP(8, 16));
            Assert.AreEqual(104, man.GetP(9, 1));
            Assert.AreEqual(118, man.GetP(9, 15));
            Assert.AreEqual(119, man.GetP(9, 16));
            Assert.AreEqual(120, man.GetP(10, 1));
            Assert.AreEqual(135, man.GetP(10, 16));
            Assert.AreEqual(136, man.GetP(11, 1));
            Assert.AreEqual(137, man.GetP(11, 2));
            Assert.AreEqual(151, man.GetP(11, 16));
            Assert.AreEqual(152, man.GetP(12, 1));
        }

        [Test]
        public void PixelsInRing()
        {
            var man = new HealpixManager(0);
            Assert.AreEqual(4, man.PixelsCountInRing(1));
            Assert.AreEqual(4, man.PixelsCountInRing(2));
            Assert.AreEqual(4, man.PixelsCountInRing(3));
            man = new HealpixManager(1);
            Assert.AreEqual(2, man.Nside);
            Assert.AreEqual(4, man.PixelsCountInRing(1));
            Assert.AreEqual(8, man.PixelsCountInRing(2));
            Assert.AreEqual(8, man.PixelsCountInRing(3));
            Assert.AreEqual(8, man.PixelsCountInRing(4));
            Assert.AreEqual(8, man.PixelsCountInRing(5));
            Assert.AreEqual(8, man.PixelsCountInRing(6));
            Assert.AreEqual(4, man.PixelsCountInRing(7));
            man = new HealpixManager(2);
            Assert.AreEqual(4, man.Nside);
            Assert.AreEqual(4, man.PixelsCountInRing(1));
            Assert.AreEqual(8, man.PixelsCountInRing(2));
            Assert.AreEqual(12, man.PixelsCountInRing(3));
            Assert.AreEqual(16, man.PixelsCountInRing(4));
            Assert.AreEqual(16, man.PixelsCountInRing(5));
            Assert.AreEqual(16, man.PixelsCountInRing(6));
            Assert.AreEqual(16, man.PixelsCountInRing(7));
            Assert.AreEqual(16, man.PixelsCountInRing(8));
            Assert.AreEqual(16, man.PixelsCountInRing(9));
            Assert.AreEqual(16, man.PixelsCountInRing(10));
            Assert.AreEqual(16, man.PixelsCountInRing(11));
            Assert.AreEqual(16, man.PixelsCountInRing(12));
            Assert.AreEqual(12, man.PixelsCountInRing(13));
            Assert.AreEqual(8, man.PixelsCountInRing(14));
            Assert.AreEqual(4, man.PixelsCountInRing(15));
        }

        [Test]
        public void GetCenter_0()
        {
            var man = new HealpixManager(0);
            Assert.AreEqual(135, man.GetCenter(0).X);
            Assert.AreEqual(L41, man.GetCenter(0).Y, .01);
            Assert.AreEqual(45, man.GetCenter(1).X);
            Assert.AreEqual(L41, man.GetCenter(1).Y, .01);
            Assert.AreEqual(-45, man.GetCenter(2).X);
            Assert.AreEqual(L41, man.GetCenter(2).Y, .01);
            Assert.AreEqual(-135, man.GetCenter(3).X);
            Assert.AreEqual(L41, man.GetCenter(3).Y, .01);

            Assert.AreEqual(90, man.GetCenter(4).X);
            Assert.AreEqual(0, man.GetCenter(4).Y);
            Assert.AreEqual(0, man.GetCenter(5).X);
            Assert.AreEqual(0, man.GetCenter(5).Y);
            Assert.AreEqual(2, man.GetCenter(5).Ring);
            Assert.AreEqual(2, man.GetCenter(5).PixelInRing);
            Assert.AreEqual(-90, man.GetCenter(6).X);
            Assert.AreEqual(0, man.GetCenter(6).Y);
            Assert.AreEqual(2, man.GetCenter(6).Ring);
            Assert.AreEqual(3, man.GetCenter(6).PixelInRing);
            Assert.AreEqual(-180, man.GetCenter(7).X);
            Assert.AreEqual(0, man.GetCenter(7).Y);

            Assert.AreEqual(135, man.GetCenter(8).X);
            Assert.AreEqual(-L41, man.GetCenter(8).Y, .01);
            Assert.AreEqual(3, man.GetCenter(8).Ring);
            Assert.AreEqual(1, man.GetCenter(8).PixelInRing);
            Assert.AreEqual(45, man.GetCenter(9).X);
            Assert.AreEqual(-L41, man.GetCenter(9).Y, .01);
            Assert.AreEqual(-45, man.GetCenter(10).X);
            Assert.AreEqual(-L41, man.GetCenter(10).Y, .01);
            Assert.AreEqual(-135, man.GetCenter(11).X);
            Assert.AreEqual(-L41, man.GetCenter(11).Y, .01);
        }
        
        [Test]
        public void GetCenter_1()
        {
            var man = new HealpixManager(1);
            Assert.AreEqual(135, man.GetCenter(0).X);
            Assert.AreEqual(66.44, man.GetCenter(0).Y, .01);
            Assert.AreEqual(1, man.GetCenter(0).Ring);
            Assert.AreEqual(157.5, man.GetCenter(4).X);
            Assert.AreEqual(L41, man.GetCenter(4).Y, .01);
            Assert.AreEqual(2, man.GetCenter(4).Ring);
            Assert.AreEqual(67.5, man.GetCenter(6).X);
            Assert.AreEqual(-22.5, man.GetCenter(8).X, .00001);

            Assert.AreEqual(135, man.GetCenter(12).X);
            Assert.AreEqual(19.47, man.GetCenter(12).Y, .01);
            Assert.AreEqual(3, man.GetCenter(12).Ring);
            Assert.AreEqual(90, man.GetCenter(13).X);
            Assert.AreEqual(45, man.GetCenter(14).X);
            Assert.AreEqual(0, man.GetCenter(15).X);
            Assert.AreEqual(-45, man.GetCenter(16).X);
            Assert.AreEqual(-135, man.GetCenter(18).X);
            Assert.AreEqual(-180, man.GetCenter(19).X);
            Assert.AreEqual(157.5, man.GetCenter(20).X);
            Assert.AreEqual(0, man.GetCenter(20).Y);
            Assert.AreEqual(4, man.GetCenter(20).Ring);
            Assert.AreEqual(112.5, man.GetCenter(21).X);
            Assert.AreEqual(67.5, man.GetCenter(22).X);
            Assert.AreEqual(22.5, man.GetCenter(23).X);
            Assert.AreEqual(-22.5, man.GetCenter(24).X, .1);
            Assert.AreEqual(-67.5, man.GetCenter(25).X, .1);
            Assert.AreEqual(-112.5, man.GetCenter(26).X, .1);
            Assert.AreEqual(-157.5, man.GetCenter(27).X, .1);
            Assert.AreEqual(4, man.GetCenter(27).Ring);
            Assert.AreEqual(135, man.GetCenter(28).X);
            Assert.AreEqual(-19.47, man.GetCenter(28).Y, .01);
            Assert.AreEqual(5, man.GetCenter(28).Ring);
            Assert.AreEqual(45, man.GetCenter(30).X);
            Assert.AreEqual(0, man.GetCenter(31).X);
            Assert.AreEqual(-180, man.GetCenter(35).X);
            Assert.AreEqual(null, man.GetCenter(35).NorthCap);

            Assert.AreEqual(false, man.GetCenter(36).NorthCap);
            Assert.AreEqual(157.5, man.GetCenter(36).X);
            Assert.AreEqual(-L41, man.GetCenter(36).Y, .01);
            Assert.AreEqual(6, man.GetCenter(36).Ring);
            Assert.AreEqual(112.5, man.GetCenter(37).X);
            Assert.AreEqual(22.5, man.GetCenter(39).X);
            Assert.AreEqual(-22.5, man.GetCenter(40).X, .0000001);
            Assert.AreEqual(-112.5, man.GetCenter(42).X, .00000001);
            Assert.AreEqual(-157.5, man.GetCenter(43).X, .00000001);
            Assert.AreEqual(135, man.GetCenter(44).X);
            Assert.AreEqual(-66.44, man.GetCenter(44).Y, .01);
            Assert.AreEqual(7, man.GetCenter(44).Ring);
            Assert.AreEqual(45, man.GetCenter(45).X);
            Assert.AreEqual(-135, man.GetCenter(47).X);
        }

        [Test]
        public void GetCenter_2()
        {
            var man = new HealpixManager(2);
            Assert.AreEqual(135, man.GetCenter(0).X);
            Assert.AreEqual(22.5, man.GetCenter(7).X);

            Assert.AreEqual(3, man.GetCenter(18).Ring);
            Assert.AreEqual(15, man.GetCenter(17).X, .0000001);
            Assert.AreEqual(-15, man.GetCenter(18).X, .0000001);

            Assert.AreEqual(4, man.GetCenter(29).Ring);
            Assert.AreEqual(11.25, man.GetCenter(31).X, .0000001);
            Assert.AreEqual(-11.25, man.GetCenter(32).X, .0000001);

            Assert.AreEqual(5, man.GetCenter(47).Ring);
            Assert.AreEqual(0, man.GetCenter(47).X);

            Assert.AreEqual(null, man.GetCenter(47).NorthCap);
            Assert.AreEqual(11.25, man.GetCenter(63).X, .00001);
            Assert.AreEqual(-11.25, man.GetCenter(64).X, .00001);
            Assert.AreEqual(0, man.GetCenter(79).X);
            Assert.AreEqual(-180, man.GetCenter(87).X);
            Assert.AreEqual(168.75, man.GetCenter(88).X);
            Assert.AreEqual(0, man.GetCenter(88).Y);
            Assert.AreEqual(11.25, man.GetCenter(95).X, .00001);
            Assert.AreEqual(-168.75, man.GetCenter(103).X);
            Assert.AreEqual(0, man.GetCenter(111).X);
            Assert.AreEqual(-180, man.GetCenter(119).X);
            Assert.AreEqual(168.75, man.GetCenter(120).X);
            Assert.AreEqual(11.25, man.GetCenter(127).X, .00001);
            Assert.AreEqual(-11.25, man.GetCenter(128).X, .00001);
            Assert.AreEqual(10, man.GetCenter(135).Ring);
            Assert.AreEqual(-168.75, man.GetCenter(135).X);
            Assert.AreEqual(157.5, man.GetCenter(136).X);
            Assert.AreEqual(135, man.GetCenter(137).X);
            Assert.AreEqual(0, man.GetCenter(143).X);
            Assert.AreEqual(11, man.GetCenter(151).Ring);
            Assert.AreEqual(-180, man.GetCenter(151).X);

            Assert.AreEqual(false, man.GetCenter(152).NorthCap);
            Assert.AreEqual(12, man.GetCenter(152).Ring);
            Assert.AreEqual(168.75, man.GetCenter(152).X);
            Assert.AreEqual(11.25, man.GetCenter(159).X, .00001);
            Assert.AreEqual(-168.75, man.GetCenter(167).X);

            Assert.AreEqual(13, man.GetCenter(168).Ring);
            Assert.AreEqual(165, man.GetCenter(168).X);

            Assert.AreEqual(14, man.GetCenter(187).Ring);
            Assert.AreEqual(157.5, man.GetCenter(180).X, .00001);
            Assert.AreEqual(-112.5, man.GetCenter(186).X, .00001);
            Assert.AreEqual(-157.5, man.GetCenter(187).X, .00001);
            Assert.AreEqual(15, man.GetCenter(188).Ring);
            Assert.AreEqual(135, man.GetCenter(188).X, .00001);
        }

        [Test]
        public void GetCenter_5()
        {
            var man = new HealpixManager(5);
            for (int p = 24; p < 24 + man.PixelsCountInRing(4); p++)
            {
                Assert.AreEqual(4, man.GetCenter(p).Ring);
                Assert.AreEqual(84.15, man.GetCenter(p).Y, .001);
            }
            Assert.AreEqual(3, man.GetCenter(18).Ring);
        }
    }
    }
#endif
