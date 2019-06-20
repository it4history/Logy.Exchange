using System.Drawing;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// http://hist.tk/ory/Рельеф_Земли_-_усредненный_меридиан
    /// </summary>
    public class ReliefMap : MeridianMap
    {
        public ReliefMap() : base(6)
        {
        }

        [Test]
        public void Relief_WhenRotationStopped()
        {
            var algorithm = new ShiftAxisGeneric<MeridianCoor>(
                new WaterAndBottomData(HealpixManager, -3128d, 7336d));
            SetData(algorithm);
            algorithm.ChangeRotation(-HealpixManager.Nside);
            Data.DoFrames(delegate(int frame) 
            {
                if (Data.Colors != null)
                    Data.Colors.DefaultColor = Color.FromArgb(255, 174, 201);
                Data.Draw(Bmp, frame - HealpixManager.Nside, null, YResolution, Scale);
                return 100; /// 240 for k8, 150 for k7, 100 for k6
            });
        }
    }
}