#if DEBUG

using NUnit.Framework;

namespace Logy.MwAgent.DotNetWikiBot.Tests
{
    [TestFixture]
    public class PageTests
    {
        [Test]
        public void GetSection()
        {
            var p = new Page
                        {
                            Text = @"1
 =a=
 asdf== a=3
=b=
"
                        };
            Assert.AreEqual(
@"
 asdf== a=3", 
            p.GetSection("a"));
        }
    }
}
#endif
