#if DEBUG
using System;
using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Smw.Tests
{
    [TestFixture]
    public class PropertyValueTests
    {
        [Test]
        public void DateTime()
        {
            var p = new PropertyValue { Raw = "1/2000" };
            Assert.AreEqual(new DateTime(2000), p.DateTime);
        }
    }
}
#endif
