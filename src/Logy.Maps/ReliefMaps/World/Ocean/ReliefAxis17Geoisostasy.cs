using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Карта_сдвига_полюса_с_учётом_геоизостазии
    /// </summary>
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(6) // run till 9
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
        }

        [Test]
        public void AxisChange()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager)
            {
                WithRelief = true,
            }) { Slow = false, Geoisostasy = true };
            InitData(algorithm, true);

            /* smoothing from previously calculated json at lower resolution 6 */
            var parentK = K - 1;
            var parent = new ReliefAxis17Geoisostasy(parentK);
            var bundle = Bundle<Basin3>.DeserializeFile(
                parent.StatsFileName(3730), // 114 - k5, 3674 - k7, 3681 - k8, 3730 - k9
                false); /// changes rotation
            for (var p = 0; p < bundle.Basins[parentK].Length; p++)
            {
                var parentBasin = bundle.Basins[parentK][p];
                var volumeForKid = parentBasin.WaterHeight;
                foreach (var kidP in parent.HealpixManager.GetCenter(p).GetKids(HealpixManager))
                {
                    Data.PixMan.Pixels[kidP].Hoq = volumeForKid - Data.PixMan.Pixels[kidP].Depth.Value;
                }
            }
            var parentAlgo = (ShiftAxis)bundle.Algorithm;
            Data.Frame = parentAlgo.DataAbstract.Frame;
            Data.Time = parentAlgo.DataAbstract.Time; /// let TimeStep be 1 at beginning

            algorithm.SetGeoisostasyDatum(parentAlgo);
            /// */

            ShiftAxisBalanced(4000);
        }

        /// protected override ImageFormat ImageFormat => ImageFormat.Tiff;
        [Test]
        public void WithPoliticalMap()
        {
            Bundle = Bundle<Basin3>.DeserializeFile(StatsFileName(4000)); // 3681 - k7, 3730 - k8, 4000 - k9

            Data.MinDefault = -950;
            Data.MaxDefault = 950;
            Data.CalcAltitudes(false);
            Data.SetColorLists();
            Draw();
            PoliticalMap.Draw(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame + 1);
        }
    }
}