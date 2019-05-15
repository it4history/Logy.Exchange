using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.Meridian.Data;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// http://logy.gq/lw/WOS_Solution#start
    /// http://hist.tk/hw/%D0%9C%D0%B5%D1%80%D0%B8%D0%B4%D0%B8%D0%B0%D0%BD%D0%BD%D0%B0%D1%8F_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%86%D0%B8%D1%8F_-_%D0%B2%D0%BE%D0%B4%D0%B0
    /// </summary>
    [TestFixture]
    public class MeridianMap : RotationStopMap<MeridianCoor>
    {
        protected override int K => 7;

        /// <summary>
        /// http://hist.tk/hw/Меридианная_проекция_-_вода#третий_пример
        /// </summary>
        [Test]
        public void Water_RotationStopped()
        {
            Data = new MeridianWater<MeridianCoor>(HealpixManager); /// -5032d, 5685d); / /integration not finished on 1000, run again!
            ChangeRotation(-HealpixManager.Nside, double.MaxValue); ///  Ellipsoid.SiderealDayInSeconds*1000);

            // must be 10.69km 
            // for k5 is 10.94, k6 10.93, k7 10.9, k8 10.72
            Data.DoFrames(delegate(int frame) 
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);

                var times = 0;
                
                // how many times to call ChangeRotation at the beginning
                if (frame - times < -HealpixManager.Nside) 
                {
                    ChangeRotation(frame, 80000);
                }
                return 1400; /// 1400 for k7
            });
        }

        /// <summary>
        /// http://hist.tk/hw/Меридианная_проекция_-_вода#второй_пример
        /// slow, near 30 minutes
        /// </summary>
        [Test]
        public void Water_RotationStopping()
        {
            Data = new MeridianWater<MeridianCoor>(HealpixManager); /// -3690d, 4185d);
            Data.DoFrames(delegate(int frame) 
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);

                // (îò 23,9 ÷àñîâ äî 26,7), è â ñåðåäèíå âðåìåíè äî 46,2 ÷àñîâ
                if ((frame % HealpixManager.Nside) / 4 == 0) 
                {
                    ChangeRotation(frame);
                }
                return 110; /// 1100 for k9
            });
        }

        /// <summary>
        /// один тазик резко вырос или опустился
        /// </summary>
        [Test]
        public void HighLowBasin()
        {
            var h = 500d;
            Data = new MeridianWater<MeridianCoor>(HealpixManager, -h, h);
            Data.PixMan.Pixels[(HealpixManager.RingsCount / 4) + 1].HeightOQ = -h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount / 4) + 2].HeightOQ = -h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount * 3 / 4) + 1].HeightOQ = h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount * 3 / 4) + 2].HeightOQ = h;
            Data.ColorsMiddle = 50;
            Data.DoFrames(delegate(int frame)
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);
                return 1;
            });
        }

        /// <summary>
        /// http://hist.tk/hw/file:Gravity_sphere_hor.png
        /// </summary>
        [Test]
        public void Gravity_sphere_hor()
        {
            Data = new Gravity(HealpixManager);
            Data.MoveAllWater();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }

        /// <summary>
        /// http://hist.tk/hw/file:CentrifugalAcceleration_sphere_hor.png
        /// </summary>
        [Test]
        public void CentrifugalAcceleration_sphere_hor()
        {
            Data = new CentrifugalAcceleration(HealpixManager);
            Data.MoveAllWater();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }

        [Test]
        public void Geodesic()
        {
            Data = new StartCheckingGeodesic(HealpixManager);
            Data.MoveAllWater();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }
    }
}