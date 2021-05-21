#if DEBUG
using Logy.Maps.Exchange;
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
        public ReliefAxis17Geoisostasy() : this(7) // till 9
        {
        }

        public ReliefAxis17Geoisostasy(int k) : base(k)
        {
            // YResolution = 4;
        }

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
            SetAlgorithm();

            ShiftAxisBalanced(100);
        }

        [Test]
        public void SmoothingTest()
        {
            SetAlgorithm();

            Smoothing();

            // todo problem with CorrectionBundle workarounded by fast shift and later MoreAccurate()
            ShiftAxis(10060);
        }

        [Test]
        public void MoreAccurate()
        {
            InitDataWithJson();
            //HighFluidity();
            ShiftAxis(300);

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
#endif