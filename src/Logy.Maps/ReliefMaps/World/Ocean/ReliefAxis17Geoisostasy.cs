using System.Drawing.Imaging;
using Logy.Maps.Exchange;
using Logy.Maps.Metrics;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    /// <summary>
    /// http://hist.tk/ory/Сдвиг_полюса_с_учётом_геоизостазии
    /// 
    /// for performance better to run AxisChange() at k6 till 4000, then MoreAccurate(), 
    ///  and afterwards Smoothing() with k7-9 and finally MoreAccurate()
    /// </summary>
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : this(6) // till 9
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
            // YResolution = 4;
        }

        // protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        public void SetAlgorithm()
        {
            var algorithm = new ShiftAxis(new OceanData(HealpixManager)
                {

                    WithRelief = true,
                })
                { Geoisostasy = true };
            InitData(algorithm, true);
        }

        [Test]
        public void AxisChange()
        {
            InitDataWithJson(); // from beginning SetAlgorithm();
            //Data.MetricType = MetricType.RadiusIntersection; HighFluidity(true);

            ShiftAxisBalanced(10000);
        }

        /// <summary>
        /// smoothing from previously calculated json at lower resolution 6
        /// </summary>
        [Test]
        public void Smoothing()
        {
            SetAlgorithm();

            var parentK = K - 1;
            var parent = new ReliefAxis17Geoisostasy(parentK);
            var bundle = Bundle<Basin3>.DeserializeFile(FindJson(parent.Dir));
            for (var p = 0; p < bundle.Basins[parentK].Length; p++)
            {
                var parentBasin = bundle.Basins[parentK][p];
                var volumeForKid = parentBasin.WaterHeight;
                foreach (var kidP in parent.HealpixManager.GetCenter(p).GetKids(parent.HealpixManager, HealpixManager))
                {
                    Data.PixMan.Pixels[kidP].Hoq = volumeForKid - Data.PixMan.Pixels[kidP].Depth.Value;
                }
            }
            Bundle.Deserialized = bundle.Deserialized;
            var parentAlgo = (ShiftAxis)bundle.Algorithm;
            Data.Frame = parentAlgo.DataAbstract.Frame;
            Data.Time = parentAlgo.DataAbstract.Time;

            /// let TimeStep be 1 at beginning

            var algorithm = (ShiftAxis)Bundle.Algorithm;
            algorithm.SetGeoisostasyDatum(parentAlgo); 
            HighFluidity();

            // todo problem with CorrectionBundle workarounded by fast shift and later MoreAccurate()
            ShiftAxis(10060);
        }

        [Test]
        public void MoreAccurate()
        {
            InitDataWithJson();
            HighFluidity();
            ShiftAxis(10100);

            DrawPoliticalMap();
        }

        [Test]
        public void AndPoliticalMap()
        {
            InitDataWithJson();
            /// Bundle = Bundle<Basin3>.DeserializeFile(StatsFileName(4000)); // 3681 - k7, 3730 - k8, 4000 - k9

            Data.MinDefault = -1100;
            Data.MaxDefault = 1100;
            DrawPoliticalMap();
        }
    }
}