using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Exchange;
using Logy.Maps.ReliefMaps.Map2D;
using Logy.Maps.ReliefMaps.World.Ocean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Geoid
{
    public class Geoid17Map : Map2DBase<Basin3>
    {
        private ComplexData _data;

        public Geoid17Map() : base(7)
        {
            // LegendNeeded = false;
        }

        public override Projection Projection => Projection.Healpix2EquirectangularFast;

        protected override DataForMap2D<Basin3> MapData => _data;

        protected override ImageFormat ImageFormat => ImageFormat.Bmp;

        [Test]
        public void Relative()
        {
            var reliefAxis17 = new ReliefAxis17();
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(reliefAxis17.StatsFileName()), false, false);

            Geoid.Obtain(bundle.Algorithm.DataAbstract);
        }

        /// <summary>
        /// http://hist.tk/ory/file:ReliefAxis17_SlowChange_2761_Currents.png
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
            _data = new ComplexData(this, data.PixMan.Pixels);
            Draw();
        }

        /// <summary>
        /// http://hist.tk/ory/Искажение_начала_перетекания
        /// </summary>
        [Test]
        public void Eddies()
        {
            var reliefAxis17 = new ReliefAxis17(7);
            var rectangle = new Rectangle<Basin3>(-98, 57, -82, 67);
            var bundle = Bundle<Basin3>.Deserialize(
                File.ReadAllText(reliefAxis17.StatsFileName()), 
                false, 
                false,
                d =>
                {
                    // d.InitialBasins = rectangle.Subset(reliefAxis17.HealpixManager);
                });
            var data = bundle.Algorithm.DataAbstract;

            _data = new ComplexData(this, data.PixMan.Pixels) { Rectangle = rectangle };
            _data.CalcArrows();
            Draw();
        }
    }
}