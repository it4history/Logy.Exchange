#if DEBUG
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Tests
{
    [TestFixture]
    public class HealpixFormattorTests
    {
        public const double Koef20Dir3 = 0.39107720544643054;
        public const double Koef31Dir2 = 0.60557311017098969;

        public static OceanData Data(int k = 2)
        {
            var data = new OceanData(new HealpixManager(k)) { Spheric = true };
            data.Init();
            data.GradientAndHeightCrosses();
            return data;
        }

        [Test]
        public void FormatManually()
        {
            var data = Data();

            var h20 = data.PixMan.Pixels[data.HealpixManager.GetP(2, 2)];
            var h30 = data.PixMan.Pixels[data.HealpixManager.GetP(3, 3)];
            var h31 = data.PixMan.Pixels[data.HealpixManager.GetP(3, 2)];

            var a = Matrix<double>.Build.DenseOfArray(new[,]
            {
                {
                    data.GetBasinHeight(h30, 3),
                    data.GetBasinHeight(h31, 2)
                },
                {
                    -data.GetBasinHeight(h30, 3) - (2 * data.GetBasinHeight(h20, 3)),
                    2 * data.GetBasinHeight(h31, 2)
                }
            });
            var b = Vector<double>.Build.Dense(new[]
            {
                data.GetBasinHeight(h30, 2),
                data.GetBasinHeight(h30, 2) - data.GetBasinHeight(h20, 2),
            });
            var x = a.Solve(b);

            Assert.AreEqual(Koef20Dir3, x[0]);
            Assert.AreEqual(Koef31Dir2, x[1]);
        }

        [Test]
        public void Format_2()
        {
            var equations = new HealpixFormattor<Basin3>(Data(), 2).Format();
            Assert.IsNull(equations.Result);
        }

        [Test]
        public void GetResult()
        {
            var data = Data();
            var equations = new HealpixFormattor<Basin3>(data).Format();
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[0], 0));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[data.HealpixManager.Npix - 1], 0));

            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[7], 0));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[7], 1));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[7], 2));
            Assert.AreEqual(Koef20Dir3, equations.GetResult(data.PixMan.Pixels[7], 3), .0000001);

            Assert.AreEqual(Koef20Dir3, equations.GetResult(data.PixMan.Pixels[16], 0), .0000001);
            Assert.AreEqual(Koef20Dir3, equations.GetResult(data.PixMan.Pixels[16], 1), .0000001);
            Assert.AreEqual(Koef31Dir2, equations.GetResult(data.PixMan.Pixels[16], 2), .0000001);
            Assert.AreEqual(Koef31Dir2, equations.GetResult(data.PixMan.Pixels[16], 3), .0000001);

            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[data.HealpixManager.GetP(12, 1)], 0));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[data.HealpixManager.GetP(12, 1)], 1));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[data.HealpixManager.GetP(12, 1)], 2));
            Assert.AreEqual(1, equations.GetResult(data.PixMan.Pixels[data.HealpixManager.GetP(12, 1)], 3));
        }

        [Test]
        public void Format_common()
        {
            for (int i = 3; i <= 8; i++)
            {
                var equations = new HealpixFormattor<Basin3>(Data(), i).Format();
                switch (i)
                {
                    case 3:
                        Assert.AreEqual(.5, equations.GetResult(2, 0, 3), .01);
                        break;
                    case 4:
                        Assert.AreEqual(Koef20Dir3, equations.GetResult(3, 0, 3), .01);
                        Assert.AreEqual(Koef31Dir2, equations.GetResult(3, 1, 2), .01);
                        break;
                    case 5:
                    case 6:
                        break;
                    case 7:
                        break;
                }
                Assert.AreEqual(1, equations.GetResult(2, 0, 2), .00000001);
                foreach (var node in equations.Nodes)
                {
                    if (node.Direction.HasValue)
                    {
                        var result = equations.GetResult(node.Basin, node.Direction.Value);
                        Assert.IsTrue(result < 1.0000001 && result > 0);
                    }
                }
            }
        }

        [Test]
        public void Format_new()
        {
            var equations = new HealpixFormattor<Basin3>(Data(),6).Format();

            equations = new HealpixFormattor<Basin3>(Data(3)).Format();
            foreach (var node in equations.Nodes)
            {
                if (node.Direction.HasValue)
                {
                    var result = equations.GetResult(node.Basin, node.Direction.Value);
                    Assert.IsTrue(result < 1.0000001 && result > 0);
                }
            }
            // var equations = new HealpixFormattor<Basin3>(Data(6)).Format();
        }
    }
}
#endif
