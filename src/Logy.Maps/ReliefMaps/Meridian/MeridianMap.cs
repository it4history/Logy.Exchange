using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.Meridian.Data;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// http://logy.gq/lw/WOS_Solution#start
    /// http://hist.tk/ory/Меридианная_проекция_-_вода
    /// </summary>
    public class MeridianMap : RotationStopMap<MeridianCoor>
    {
        public MeridianMap() : this(7)
        {
        }
        public MeridianMap(int k) : base(k)
        {
        }

        /// <summary>
        /// http://hist.tk/ory/Меридианная_проекция_-_вода#третий_пример
        /// </summary>
        [Test]
        public void Water_RotationStopped()
        {
            // integration not finished on 1000, run again!
            var algorithm = new ShiftAxisGeneric<MeridianCoor>(
                new MeridianWater<MeridianCoor>(HealpixManager/*, -5032d, 5685d*/));
            SetData(algorithm);
            algorithm.ChangeRotation(-HealpixManager.Nside); ///  Ellipsoid.SiderealDayInSeconds*1000);

            // must be 10.69km 
            // for k5 is 10.94, k6 10.93, k7 10.9, k8 10.72
            Data.DoFrames(delegate(int frame) 
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);

                var times = 0;
                
                // how many times to call ChangeRotation at the beginning
                if (frame - times < -HealpixManager.Nside) 
                {
                    algorithm.ChangeRotation(frame, 80000);
                }
                switch (K)
                {
                    default:
                    case 6: return 100;
                    case 7: return 1400;
                }
            });
        }

        /// <summary>
        /// http://hist.tk/ory/Меридианная_проекция_-_вода#второй_пример
        /// slow, near 30 minutes
        /// </summary>
        [Test]
        public void Water_RotationStopping()
        {
            var algorithm = new ShiftAxisGeneric<MeridianCoor>(
                new MeridianWater<MeridianCoor>(HealpixManager/*, -2523d, 5208d*/));
            SetData(algorithm);

            Data.DoFrames(delegate(int frame) 
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);

                // (от 23,9 часов до 26,7), и в середине времени до 46,2 часов
                if ((frame % HealpixManager.Nside) / 4 == 0) 
                {
                    algorithm.ChangeRotation(frame, 10000);
                }
                switch (K)
                {
                    default:
                    case 6: return 130;
                    case 7: return 300;
                    case 9: return 1100;
                }
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
            Data.PixMan.Pixels[(HealpixManager.RingsCount / 4) + 1].Hoq = -h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount / 4) + 2].Hoq = -h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount * 3 / 4) + 1].Hoq = h;
            Data.PixMan.Pixels[(HealpixManager.RingsCount * 3 / 4) + 2].Hoq = h;
            Data.ColorsMiddle = 50;
            Data.DoFrames(delegate(int frame)
            {
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);
                return 1;
            });
        }

        /// <summary>
        /// http://hist.tk/ory/file:Gravity_sphere_hor.png
        /// </summary>
        [Test]
        public void Gravity_sphere_hor()
        {
            Data = new GravityChecking(HealpixManager);
            Data.CalcAltitudes();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }

        /// <summary>
        /// http://hist.tk/ory/file:CentrifugalAcceleration_sphere_hor.png
        /// </summary>
        [Test]
        public void CentrifugalAcceleration_sphere_hor()
        {
            Data = new CentrifugalAcceleration(HealpixManager);
            Data.CalcAltitudes();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }

        [Test]
        public void Geodesic()
        {
            Data = new StartCheckingGeodesic(HealpixManager);
            Data.CalcAltitudes();
            Data.Draw(Bmp, 0, null, YResolution, Scale);
        }
    }
}