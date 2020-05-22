using System;
using Logy.Maps.Exchange;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    public class ReliefMap : OceanMap
    {
        public ReliefMap() : this(5)
        {
        }

        public ReliefMap(int k) : base(k)
        {
        }

        [Test]
        public void Basin()
        {
            Data = new OceanData(HealpixManager /*, -7000d*/)
            {
                /// Visual = basin => basin.WaterHeight
            };

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Depth = -500;
            basin.Hoq = 10000;

            Data.DoFrames(
                delegate(int frame)
                {
                    Draw();
                    SaveBitmap(frame);
                    return 1;
                },
                10);
        }

        /// <summary>
        /// http://hist.tk/ory/Карта_Земли,_вращение_которой_остановилось
        /// </summary>
        [Test]
        public void RotationStopped()
        {
            var algorithm = new ShiftAxis(
                new OceanData(
                    HealpixManager,
                    /*, -6000d, null*/
                    -1000d,
                    5000d,
                    true)
                {
                    WithRelief = true,
                    SamePolesAndEquatorGravitation = true,
                    // Spheric = true,
                    IntegrationEndless = true,
                    /// Visual = basin => basin.Depth.Value
                    /// Visual = basin => basin.r - Earth2014Manager.Radius2Add //*/
                });
            InitData(algorithm);

            // 1000 does nothing when !SamePolesAndEquatorGravitation 
            if (Data.SamePolesAndEquatorGravitation)
                algorithm.ChangeRotation(
                    -1 - HealpixManager.Nside, 
                    algorithm.Data.Spheric ? double.MaxValue : - 2000);

            var changeFrames = 15;

            var framesCount = K == 5 ? 150 : 1000;
            Data.DoFrames(
                delegate(int frame)
                {
                    if (frame % changeFrames == 0)
                    {
                        double koef;
                        if (frame > framesCount * .6)
                            koef = double.MaxValue;
                        else if ((frame / changeFrames) % 3 == 3 - 1)
                            koef = 10000;
                        else
                        {
                            koef = K < 8
                                ? -100 + (frame / changeFrames
                                          * (Data.SamePolesAndEquatorGravitation ? 100 : 2000))
                                : 500;
                        }
                        algorithm.ChangeRotation(frame - HealpixManager.Nside, koef);
                    }
                    Data.Draw(Bmp, 0, null, YResolution, Scale);
                    SaveBitmap(frame);
                    return 1 + (frame / 50 * 10);
                },
                framesCount);
        }

        protected void ShiftAxisBalanced(int framesCount)
        {
            var algo = Bundle.Algorithm as ShiftAxis;
            ShiftAxis(
                framesCount,
                (frame) =>
                {
                    // has sense if Slow == true
                    switch (K)
                    {
                        // to make secondShift "Y":86.6 on frame 5 (or 9 if algo.Geoisostasy) and following on frame 61, 121 etc
                        // if algo.Geoisostasy then on frame 5, 31, 91 there will be litosphere gravity change
                        case 7:
                        case 6:
                        case 5:
                            var secondShift = 4 * (algo.Geoisostasy ? 2 : 1);
                            var cycle = 60;
                            return frame <= secondShift ? secondShift : cycle; // cycle must be > secondShift*2
                    }
                    return 10;
                },
                frame =>
                {
                    switch (K)
                    {
                        case 7: return 15 + (frame / 10);
                    }
                    return 15;
                });
        }

        /// <summary>
        /// smoothing from previously calculated json at lower resolution 6
        /// similar to OceanMapGravityAxisChange.CorrectionLoadedFromParentResolutionAndCalculated()
        /// </summary>
        protected void Smoothing()
        {
            // json of parentK is needed and Data with Bundle  must be set (they receive new basins)

            var parentK = K - 1;
            var parent = (ReliefMap)Activator.CreateInstance(GetType(), parentK);
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
//?            HighFluidity();
        }
    }
}