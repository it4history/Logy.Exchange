using System.IO;
using System.Linq;
using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefAxis17Geoisostasy : ReliefMap
    {
        public ReliefAxis17Geoisostasy() : base(7)
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
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(parent.StatsFileName(3681)), // 114 - k5, 3674 - k7,
                false /// changes rotation
                ); 
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

            var datum = parentAlgo.Poles.Values.Last();
            datum.CorrectionBundle = datum.LoadCorrection(Data.K);
            algorithm.SetDatum(datum, 0);
            //*/

            ShiftAxisBalanced(4000);
        }

        [Test]
        public void WithPoliticalMap()
        {
            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(StatsFileName(3681)));

            Data.MinDefault = -950;
            Data.MaxDefault = 950;
            Data.CalcAltitudes(false);
            Data.SetColorLists();
            Draw();
            DrawPoliticalMap(Bmp, HealpixManager, YResolution, Scale);
            SaveBitmap(Data.Frame + 1);
        }

        [Test]
        public void CheckEddies()
        {
            Subdir = "checkEddies";

            Bundle = Bundle<Basin3>.Deserialize(File.ReadAllText(StatsFileName(3674)));

            Data.DoFrames(
                (frame) =>
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                }, 
                Data.Frame + 3);
            Draw();
        }
    }
}