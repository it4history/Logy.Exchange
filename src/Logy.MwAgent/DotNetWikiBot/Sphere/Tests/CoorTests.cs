﻿#if DEBUG

using System;
using Logy.MwAgent.DotNetWikiBot.Wikidata;
using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Sphere.Tests
{
    [TestFixture]
    public class CoorTests
    {
        [Test]
        public void Constructor()
        {
            var coor = new Coor("70.1:52");
            Assert.AreEqual(70.1d, coor.X);
            Assert.AreEqual(52, coor.Y);
        }

        [Test]
        public void BetaLambda()
        {
            var coor = (Coor)new Coor("-180:-90").Normalize();
            Assert.AreEqual(-180d, coor.X);
            Assert.AreEqual(-90d, coor.Y);
            Assert.AreEqual(2 * Math.PI, coor.Lambda);
            Assert.AreEqual(Math.PI, coor.Beta);

            coor = (Coor)new Coor("180:90").Normalize();
            Assert.AreEqual(180d, coor.X);
            Assert.AreEqual(90d, coor.Y);
            Assert.AreEqual(0, coor.Lambda);
            Assert.AreEqual(0, coor.Beta);
        }
    }
}

#endif
