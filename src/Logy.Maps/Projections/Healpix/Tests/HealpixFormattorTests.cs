#if DEBUG
using Logy.Maps.ReliefMaps.World.Ocean;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace Logy.Maps.Projections.Healpix.Tests
{
    [TestFixture]
    public class HealpixFormattorTests
    {
        private static OceanData Data
        {
            get
            {
                var data = new OceanData(new HealpixManager(2)) { Spheric = true };
                data.Init();

                data.GradientAndHeightCrosses();
                return data;
            }
        }

        [Test]
        public void FormatManually()
        {
            var data = Data;

            var h21 = data.PixMan.Pixels[data.HealpixManager.GetP(2, 2)];
            var h31 = data.PixMan.Pixels[data.HealpixManager.GetP(3, 3)];
            var h32 = data.PixMan.Pixels[data.HealpixManager.GetP(3, 2)];

            var a = Matrix<double>.Build.DenseOfArray(new[,]
            {
                {
                    data.GetBasinHeight(h31, 3),
                    data.GetBasinHeight(h32, 2)
                },
                {
                    -data.GetBasinHeight(h31, 3) - (2 * data.GetBasinHeight(h21, 3)),
                    2 * data.GetBasinHeight(h32, 2)
                }
            });
            var b = Vector<double>.Build.Dense(new[]
            {
                data.GetBasinHeight(h31, 2),
                data.GetBasinHeight(h31, 2) - data.GetBasinHeight(h21, 2),
            });
            var x = a.Solve(b);

            Assert.AreEqual(0.39107720544643054, x[0]);
            Assert.AreEqual(0.60557311017098969, x[1]);
        }

        [Test]
        public void Format_2_3_rings()
        {
            var equations = new HealpixFormattor().Format(2, Data);
            Assert.IsNull(equations.Result);
            equations = new HealpixFormattor().Format(3, Data);
            Assert.AreEqual(1, equations.GetResult(2, 0, 2));
            Assert.AreEqual(-1, equations.GetResult(2, 0, 3), .01);
        }

        [Test]
        public void Format_4_rings()
        {
            var equations = new HealpixFormattor().Format(4, Data);
            Assert.AreEqual(1, equations.GetResult(2, 0, 2));
            Assert.AreEqual(.39, equations.GetResult(2, 0, 3), .01);
            Assert.AreEqual(.61, equations.GetResult(3, 1, 2), .01);
        }
    }
}
#endif
