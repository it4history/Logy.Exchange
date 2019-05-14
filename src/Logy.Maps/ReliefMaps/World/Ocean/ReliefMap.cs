#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class ReliefMap : OceanMap
    {
        public ReliefMap()
        {
            LegendNeeded = false;
        }

        [Test]
        public void Basin()
        {
            Data = new BasinData(HealpixManager, false, false
                ///       , -7000d
            )
            {
                IntegrationEndless = true,
                /// Visual = basin => basin.WaterHeight
            };

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Depth = -500;
            basin.hOQ = 10000;

            Data.DoFrames(delegate (int frame) 
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(frame);
                return 1;
            }, 600);
        }

        [Test]
        public void RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                ///, -6000d, null
                , -1000d, 5000d, true
            )
            {
                SamePolesAndEquatorGravitation = true,
                IntegrationEndless = true,
                /// Visual = basin => basin.Depth.Value
                /// Visual = basin => basin.r - Earth2014Manager.Radius2Add //*/
            };

            // 1000 does nothing when !SamePolesAndEquatorGravitation 
            if (Data.SamePolesAndEquatorGravitation)
                ChangeRotation(-1 - HealpixManager.Nside, -2000);

            var changeFrames = 15;

            var framesCount = 1000; /// 40 for k5
            Data.DoFrames(delegate(int frame) 
            {
                if (frame % changeFrames == 0)
                {
                    double koef;
                    if (frame == framesCount / 1.5)
                        koef = double.MaxValue;
                    else if ((frame / changeFrames) % 3 == 3 - 1)
                        koef = 10000;
                    else
                    {
                        koef = (K < 8
                            ? (-100 + frame / changeFrames
                               * (Data.SamePolesAndEquatorGravitation ? 100 : 2000))
                            : 500);
                    }
                    ChangeRotation(frame - HealpixManager.Nside, koef);
                }
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(frame);
                return 1 + frame / 50 * 10;
            }, framesCount);
        }
    }
}
#endif