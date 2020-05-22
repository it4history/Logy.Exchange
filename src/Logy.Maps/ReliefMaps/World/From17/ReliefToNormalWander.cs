using Logy.Maps.Geometry;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.From17
{
    public class ReliefToNormalWander : ReliefToNormal
    {
        public ReliefToNormalWander() : base(7)
        {
        }

        /// <summary>
        /// how water moves from Greenland to Datum.Normal with solid litosphere
        /// </summary>
        [Test]
        public void Sharp()
        {
            // for k6 start 4000, 4352 (-637m..), 4852 (-510m..)
            // for k7 start 8200 (-1118m..), 8300plusHvalin
            string from = null;// "08300plusHvalin";
            Subdir = "sharp" + from;

            // InitDataWithJson(); // 
            Load(from, false, new Datum { X = -40, Y = 90 });

            Run(K == 6 ? 30 : 50);
        }

        [Test]
        public void PlusWaterToHvalin()
        {
            Subdir = "plusHvalin";
            Load();
            /// InitDataWithJson();

            // k6 GetP(22, 20)
            // for k7 near 78000 cub km
            Data.PixMan.Pixels[HealpixManager.GetP(90, 110)].Hoq = 30000; 
            Run(100); 
        }

        [Test]
        public void PlusWaterToHvalinAndSharpTo80()
        {
            Subdir = "plusHvalin";
            Load(109, false, new Datum { X = -40, Y = 80 });

            Run(113);
        }

        [Test]
        public void GeoisostasyAfterPlusAndSharp()
        {
            Load(null, true);

            Run(500);
        }
    }
}