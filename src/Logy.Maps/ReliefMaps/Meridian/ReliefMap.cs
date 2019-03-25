using System.Drawing;
using Logy.Maps.ReliefMaps.Water;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Meridian
{
    /// <summary>
    /// http://hist.tk/hw/Рельеф_Земли_-_усредненный_меридиан
    /// </summary>
    [TestFixture]
    public class ReliefMap : MeridianMap
    {
        protected override int K
        {
            get { return 6; }
        }

        [Test]
        public void Relief_WhenRotationStopped()
        {
            var data = new WaterAndBottomData(HealpixManager
                ,-3128d, 7336d
            );
            Data = data;
            data.CheckOcean();

            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            Data.Cycle(100, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                if (Data.Colors != null)
                    Data.Colors.DefaultColor = Color.FromArgb(255, 174, 201);
                Data.Draw(Bmp, step);
            });

            data.RecheckOcean();
        }
    }
}