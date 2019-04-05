#if DEBUG
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Logy.Maps.Geometry;
using Logy.Maps.Projections;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Map2D;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.World.Ocean
{
    [TestFixture]
    public class OceanMap : RotationStopMap<Basin3>
    {
        public OceanMap()
        {
            if (K < 7)
            {
                YResolution = 3;
                Scale = (7 - K) * 3;
            }
        }

        protected override int K
        {
            get { return 4; }
        }

        protected override ImageFormat ImageFormat
        {
            get { return ImageFormat.Jpeg; }
        }

        [SetUp]
        public override void SetUp()
        {
            if (Directory.Exists(Dir))
                Directory.Delete(Dir, true);
            base.SetUp();
        }

        [Test]
        public void Water_Sphere_HighBasin()
        {
            Data = new BasinData(HealpixManager, false, false //true for sphere
            , -200d//, 2000d
            );


            var h = 15000d;
            var p = HealpixManager.GetP(HealpixManager.Nside+5, HealpixManager.Nside * 2);
            var basin = Data.PixMan.Pixels[p]; 
            basin.hOQ = h;
            Data.PixMan.Pixels[HealpixManager.GetP(HealpixManager.Nside, (int)(HealpixManager.Nside * 2.5))].hOQ = h;

            var framesCountBy2 = 10;
            Data.Cycle(2, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                Circle(basin);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        [Test]
        public void Water_Gradient()
        {
            Data = new BasinData(HealpixManager, false, false //true for sphere
                , -2000d //, 2000d
            );

            var p = HealpixManager.GetP(HealpixManager.Nside - 1, HealpixManager.Nside * 1);
            var basin = Data.PixMan.Pixels[p];
            basin.Delta_g_meridian = -.2;

            p = HealpixManager.GetP(HealpixManager.Nside * 2, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside * 3, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_meridian = .2;

            p = HealpixManager.GetP(HealpixManager.Nside + 5, HealpixManager.Nside * 2);
            Data.PixMan.Pixels[p].Delta_g_traverse = .2;


            var framesCountBy2 = 15;
            Data.Cycle(1, delegate(int step)
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                Circle(basin);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        [Test]
        public void Water_ChangeRotation()
        {
            Data = new BasinData(HealpixManager, false, true
                , -200d//, 2000d
            );
            //// fill Basin.Visual and uncomment BasinData.GetAltitude to see centrifugal components

            EllipsoidAcceleration.AxisOfRotation =
                new UnitVector3D(1, 0, 0);
                ////Basin.Oz.Rotate(new UnitVector3D(1, 0, 0), new Angle(15.0, AngleUnit.Degrees))
            ChangeRotation(-HealpixManager.Nside, 0);//double.MaxValue);
            var framesCountBy2 = 500;
            Data.Cycle(1, delegate (int step)
            {
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }


        private void Circle(Basin3 basin, double r = .2)
        {
            if (basin != null)
            {
                var width = .05;//k4
                foreach (var pixel in Data.PixMan.Pixels)
                {
                    var healCoor = (HealCoor) pixel;
                    var dist = basin.DistanceTo(healCoor);
                    if (Data.Colors != null
                        && dist >= r - width && dist <= r + width)
                    {
                        var eqProjection = new Equirectangular(HealpixManager, YResolution);
                        var point = eqProjection.Offset(healCoor);
                        Data.Colors.SetPixelOnBmp(null, Bmp,
                            (int) (point.X), (int) point.Y, Scale);
                    }
                }
            }
        }

        [Test]
        public void Water_RotationStopped()
        {
            Data = new BasinData(HealpixManager, false, false
                // ,-3000d, 3000d
            );
            Data.ColorsMiddle = null;

            /*EllipsoidAcceleration.AxisOfRotation =
                new UnitVector3D(1, 0, 0);/*, new Angle(15.0, AngleUnit.Degrees));
                Basin.Oz
                    .Rotate(new UnitVector3D(0, 1, 0), new Angle(17, AngleUnit.Degrees))
                    .Rotate(new UnitVector3D(0, 0, 1), new Angle(-40, AngleUnit.Degrees))
                    ;*/

            Basin3 basin = null;
            // basin =Data.PixMan.Pixels[HealpixManager.GetP() / 2];
            var accur = 1.5;
            foreach (var b in Data.PixMan.Pixels)
            {
                if (Math.Abs(b.X - (-40)) < accur  && Math.Abs(b.Y - (73)) < accur )
                {
                    basin = b;
                    break;
                }
            }
            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            var framesCountBy2 = 200;
            Data.Cycle(50, delegate(int step) 
            {
                if (Data.Colors != null)
                    Data.Colors.DefaultColor = Color.FromArgb(255, 174, 201);
                Data.Draw(Bmp, 0, null, YResolution, Scale);
                   Circle(basin, .03);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);
        }

        [Test]
        public void Relief_RotationStopped()
        {
            Data = new BasinData(HealpixManager, true, false
                , -7000d);
            Data.CheckOcean();

            ChangeRotation(-HealpixManager.Nside, double.MaxValue);
            var framesCountBy2 = 3;
            Data.Cycle(10, delegate(int step) //240 for k8, 150 for k7, 100 for k6
            {
                Data.Draw(Bmp);
                SaveBitmap(step + framesCountBy2);
            }, framesCountBy2);

            Data.RecheckOcean();
        }
    }
}
#endif