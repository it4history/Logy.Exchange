using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Карта_сдвига_полюса_с_учётом_геоизостазии
    /// 
    /// for performance better to run AxisChange() at k6 till 4000, than MoreAccurate(), 
    ///  and afterwards AxisChange() with k7-9 and uncommented smoothing
    /// </summary>
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(7) // till 9
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
            var smoothing = true;
            if (smoothing)
            {
                var parentK = K - 1;
                var parent = new ReliefAxis17Geoisostasy(parentK);
                var bundle = Bundle<Basin3>.DeserializeFile(
                    FindJson(parent.Dir)); // frames were 114 - k5, 3674 - k7, 3681 - k8, 3730 - k9
                for (var p = 0; p < bundle.Basins[parentK].Length; p++)
                {
                    var parentBasin = bundle.Basins[parentK][p];
                    var volumeForKid = parentBasin.WaterHeight;
                    foreach (var kidP in parent.HealpixManager.GetCenter(p).GetKids(HealpixManager))
                    {
                        Data.PixMan.Pixels[kidP].Hoq = volumeForKid - Data.PixMan.Pixels[kidP].Depth.Value;
                    }
                }
                Bundle.Deserialized = bundle.Deserialized;
                var parentAlgo = (ShiftAxis)bundle.Algorithm;
                Data.Frame = parentAlgo.DataAbstract.Frame;
                Data.Time = parentAlgo.DataAbstract.Time; /// let TimeStep be 1 at beginning

                algorithm.SetGeoisostasyDatum(parentAlgo);
                HighFluidity();
            }

            ShiftAxisBalanced(10000);
        }

        [Test]
        public void MoreAccurate()
        {
            InitDataWithJson();
            HighFluidity();
            ShiftAxis(10000);

            DrawPoliticalMap();
        }

        public void HighFluidity()
        {
            Data.Water.Threshhold *= .1; /// for k6 standard Threshhold is 2.4m
            Data.Water.Fluidity = .9;
        }

        /// protected override ImageFormat ImageFormat => ImageFormat.Tiff;
        [Test]
        public void AndPoliticalMap()
        {
            InitDataWithJson();
            /// Bundle = Bundle<Basin3>.DeserializeFile(StatsFileName(4000)); // 3681 - k7, 3730 - k8, 4000 - k9

            Data.MinDefault = -950;
            Data.MaxDefault = 950;
            DrawPoliticalMap();
        }
    }
}