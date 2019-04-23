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
                //       , -7000d
            )
            {
                NoIntegrationFinish = true,
                //Visual = basin => basin.WaterHeight
            };

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Depth = -500;
            basin.hOQ = 10000;

            var framesCountBy2 = 300;
            Data.Cycle(delegate (int step) 
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step);
                return 1;
            }, framesCountBy2);
        }

        [Test]
        public void RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                //, -6000d, null
                , -1000d, 5000d, true
            )
            {
                SamePolesAndEquatorGravitation = true,
                NoIntegrationFinish = true,
                //Visual = basin => basin.Depth.Value
                //Visual = basin => basin.r - Earth2014Manager.Radius2Add //*/
            };

            // 1000 does nothing when !SamePolesAndEquatorGravitation 
            if (Data.SamePolesAndEquatorGravitation)
                ChangeRotation(-1 - HealpixManager.Nside, -2000);

            var changeSteps = 15;

            var framesCountBy2 = 500;
            Data.Cycle(delegate(int step) //20 for k5
            {
                if (step % changeSteps == 0)
                {
                    double koef;
                    if (step == framesCountBy2 / 3)
                        koef = double.MaxValue;
                    else if ((step / changeSteps) % 3 == 3 - 1)
                        koef = 10000;
                    else
                    {
                        koef = (K < 8
                            ? (-100 + step / changeSteps
                               * (Data.SamePolesAndEquatorGravitation ? 100 : 2000))
                            : 500);
                    }
                    ChangeRotation(step - HealpixManager.Nside, koef);
                }
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step);
                return 1 + step / 50 * 10;
            }, framesCountBy2);
        }
    }
}
#endif