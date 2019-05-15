#if DEBUG
using System;
using Logy.Maps.Projections.Healpix;
using Logy.Maps.ReliefMaps.Meridian;
using Logy.Maps.ReliefMaps.Meridian.Data;
using NUnit.Framework;

namespace Logy.Maps.ReliefMaps.Water.Tests
{
    /// <summary>
    /// dots  ̣ ․.·˙
    /// slashes  /／∕⁄̷     \⍀＼
    /// tilde combining ̰. ̃.    ͠ ͊ ̾   ˷
    /// </summary>
    public enum BottomForm
    {
        No,
        Shallow = 1,
        InDepth = 2,
        BasinExcited = 3,
        BasinInDepth = 4,
        ShallowSouth = 5,
        NorthBasinInDepth = 6,
        ReliefLikeWater = 100,
    }

    [TestFixture]
    public class WaterModelTests
    {
        /// <summary>
        /// in meters
        /// </summary>
        private const int HeightExcitement = 2;

        private MeridianCoor basin;
        private MeridianCoor northBasin;
        private MeridianCoor southBasin;

        [Test]
        public void Threshhold()
        {
            Assert.AreEqual(59.2, new WaterModel(new HealpixManager(1)).ThreshholdNotReliable);
            Assert.AreEqual(3.7, new WaterModel(new HealpixManager(5)).ThreshholdNotReliable);
            Assert.AreEqual(1.85, new WaterModel(new HealpixManager(6)).ThreshholdNotReliable);
            Assert.AreEqual(.95, new WaterModel(new HealpixManager(7)).ThreshholdNotReliable, .1);
            Assert.AreEqual(.4625, new WaterModel(new HealpixManager(8)).ThreshholdNotReliable);
            Assert.AreEqual(.23125, new WaterModel(new HealpixManager(9)).ThreshholdNotReliable);

            Assert.AreEqual(.3, new WaterModel(new HealpixManager(9)).Threshhold, .01);
        }

        public MeridianWater<MeridianCoor> GetData(
            bool fullMeridian = false,
            bool excite = false,
            BottomForm bottomForm = BottomForm.No)
        {
            var man = new HealpixManager(9); /// 2 is good too
            northBasin = man.GetCenter<MeridianCoor>(0);
            basin = man.GetCenter<MeridianCoor>(4);
            southBasin = man.GetCenter<MeridianCoor>(12);
            var pix = fullMeridian
                /// -.=.=.- 
                ? new[] { northBasin, basin, southBasin }
                /// -.=.- 
                : new[] { northBasin, basin };

            var data = new MeridianWater<MeridianCoor>(man, pix, false);
            data.GradientAndHeightCrosses();
            if (excite)
            {
                // data.Water.Move()
                basin.HeightOQ = HeightExcitement;
                data.GradientAndHeightCrosses();
            }
            switch (bottomForm)
            {
                case BottomForm.Shallow:
                    foreach (var t in pix)
                        t.Depth = 0;
                    break;
                case BottomForm.InDepth:
                    foreach (var t in pix)
                        t.Depth = HeightExcitement;
                    break;
                case BottomForm.BasinExcited:
                    foreach (var t in pix)
                        t.Depth = t == basin
                            ? -HeightExcitement
                            : HeightExcitement;
                    break;
                case BottomForm.BasinInDepth:
                    foreach (var t in pix)
                        t.Depth = t == basin ? HeightExcitement : 0;
                    break;
                case BottomForm.ShallowSouth:
                    foreach (var t in pix)
                        t.Depth = t == southBasin ? 0 : HeightExcitement;
                    break;
                case BottomForm.NorthBasinInDepth:
                    northBasin.Depth = HeightExcitement;
                    basin.Depth = -HeightExcitement;
                    southBasin.Depth = 0;
                    break;
                case BottomForm.ReliefLikeWater:
                    foreach (var t in pix)
                        t.Depth = (int)-t.HeightOQ;
                    break;
            }
            return data;
        }

        [Test]
        public void Intersect()
        {
            var water = GetData().Water;
            var hQ = northBasin.Intersect(basin);
            /* диапазон 1,8м для k8, 1м для k9 http://hist.tk/hw/file:Deltah_Q-h_Q_s.png */
            Assert.Less(Math.Abs(basin.Hto[0] - hQ), water.Threshhold);
            Assert.AreEqual(0, water.Move(basin, northBasin, NeighborVert.North));
        }

        [Test]
        public void Move_HalfOfMeridian()
        {
            var data = GetData(false, true);
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(-.16, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North));
            Assert.AreEqual(1.36, basin.HeightOQ, .01);
            Assert.AreEqual(1.28, northBasin.HeightOQ, .01);

            // masses сonservation law
            Assert.AreEqual(
                HeightExcitement,
                basin.HeightOQ + (northBasin.HeightOQ * (northBasin.RingArea / basin.RingArea)),
                .000001);
        }

        [Test]
        public void Move_Meridian()
        {
            var data = GetData(true, true);
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
            Assert.AreEqual(.8, data.Water.Move(basin, southBasin, NeighborVert.South), .01);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(-.48, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
            Assert.AreEqual(0, data.Water.Move(basin, southBasin, NeighborVert.South), .001);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North));
            Assert.AreEqual(.138, data.Water.Move(basin, southBasin, NeighborVert.South), .001);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(0, data.Water.Move(basin, southBasin, NeighborVert.South));
            Assert.AreEqual(.74, basin.HeightOQ, .01);
            Assert.AreEqual(.64, northBasin.HeightOQ, .01);
            Assert.AreEqual(.62, southBasin.HeightOQ, .01);

            // masses сonservation law
            var actual = basin.HeightOQ + (northBasin.HeightOQ * northBasin.RingArea / basin.RingArea)
                         + (southBasin.HeightOQ * southBasin.RingArea / basin.RingArea);
            Assert.AreEqual(HeightExcitement, actual, .000001);
        }

        [Test]
        public void Relief_HalfOfMeridian()
        {
            var data = GetData(false, false, BottomForm.Shallow);
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North));
            data = GetData(false, false, BottomForm.InDepth);
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North));
            data = GetData(false, false, BottomForm.BasinExcited);
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North));

            data = GetData(false, true, BottomForm.Shallow);
            /*    ̣ 
               _./
               ̃˜˜˜˜˜ */
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
            data.GradientAndHeightCrosses();
            Assert.AreEqual(-.16, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(false, true, BottomForm.InDepth);
            /*    ̣ 
               _./

               ̃˜˜˜˜˜ */
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(false, true, BottomForm.BasinExcited);
            /*    ̣ 
               _./˜˜

               ̃˜˜˜  */
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(false, true, BottomForm.ReliefLikeWater);
            /*    ̣ 
               _./˜˜
               ̃˜˜˜   */
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
        }

        [Test]
        public void Relief_Meridian_Shallow()
        {
            var data = GetData(true, true, BottomForm.Shallow);
            /*    ̣ 
               _./ \._
               ̃˜˜˜˜˜˜˜ */
            data.DoFrame(); /// data.MoveAllWater(); was enough
            Assert.AreEqual(.8, basin.HeightOQ, .01); /*per .8 to north and south*/
            Assert.IsTrue(data.WasWaterMoved());
            data.DoFrame();
///            Assert.AreEqual(.8, basin.hOQ, .01); /*per -.16 to north and south*/
///            data.Frame();
            Assert.IsFalse(data.WasWaterMoved());

            /*    ̣ 
               _./ \.
               ̃˜˜˜˜˜˜\.
                     ˜˜   */
        }

        [Test]
        public void Relief_Meridian()
        {
            var data = GetData(true, true, BottomForm.InDepth);
            /*    ̣ 
               _./ \._

               ̃˜˜˜˜˜˜˜ */
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(true, true, BottomForm.BasinExcited);
            /*    ̣ 
               _./˜\._
                  
               ̃˜˜˜  ˜˜˜ */
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(true, true, BottomForm.BasinInDepth);
            /*    ̣ 
               _./ \._
               ˜˜˜ ˜˜˜
                  ˜   */
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(true, true, BottomForm.ShallowSouth);
            /*    ̣ 
               _./ \._
                    ˜˜
               ̃˜˜˜˜˜   */
            Assert.AreEqual(.8, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(true, true, BottomForm.NorthBasinInDepth);
            /*    ̣ 
               _./˜\._
                    ˜˜˜
               ̃˜˜      */
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North), .001);

            data = GetData(true, true, BottomForm.ReliefLikeWater);
            /*    ̣ 
               _./˜\._
               ̃˜˜˜ ˜˜˜ */
            Assert.AreEqual(0, data.Water.Move(basin, northBasin, NeighborVert.North), .001);
        }
    }
}
#endif