using Logy.Maps.Geometry;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.From17
{
    public class ReliefToNormalWander : ReliefToNormal
    {
        public ReliefToNormalWander() : base(7)
        {
        }

        [Test]
        public void PlusWaterToHvalin()
        {
            Subdir = "plusHvalin";
            Load(8200);
            // InitDataWithJson();

            // k6 GetP(22, 20)
            Data.PixMan.Pixels[HealpixManager.GetP(90, 110)].Hoq = 30000; // near 78000 cub km
            Run(100); // 20, 40, 90 with adding each time
        }

        [Test]
        public void Sharp()
        {
            // for k6 start 4000, 4352 (-637m..), 4852 (-510m..)
            // for k7 start 8200 (-1118m..), 8300plusHvalin
            var from = "08300plusHvalin";
            Subdir = "sharp" + from;
            // InitDataWithJson(); // 
            Load(from, false, new Datum { X = -40, Y = 80 });

            Run(K == 6 ? 30 : 50);
        }

        [Test]
        public void GeoisostasyAfterPlusAndSharp()
        {
            Load(null, true);

            Run(500);
        }
    }
}