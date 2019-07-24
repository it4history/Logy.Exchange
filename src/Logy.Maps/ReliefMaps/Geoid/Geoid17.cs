using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid17 : Map2DBase<Basin3>
    {
        private Geoid17Data _data;

        public Geoid17() : base(7)
        {
            YResolution = 3;
            Scale = 2;
        }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        protected override DataForMap2D<Basin3> MapData => _data;

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        [Test]
        public void Relative()
        {
            var reliefAxis17 = new ReliefAxis17(5);
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(reliefAxis17.StatsFileName()));

            Geoid.Obtain(bundle.Algorithm.DataAbstract);
        }

        /// <summary>
        /// http://hist.tk/ory/Рельеф_после_сдвига_полюса_на_17_градусов
        /// </summary>
        [Test]
        public void RelativePaleogeoid()
        {
            var reliefAxis17 = new ReliefAxis17();
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(reliefAxis17.StatsFileName()), false, false);
            /* var data = new GeoidData(HealpixManager)
            {
                WithRelief = true,
                Accuracy = 1
            };
            data.Init(); */
            var data = bundle.Algorithm.DataAbstract;

            Geoid.Obtain(data);
            _data = new Geoid17Data(this, data.PixMan.Pixels);
            Draw();
        }
    }
}