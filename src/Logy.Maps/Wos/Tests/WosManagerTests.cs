#if DEBUG
using NUnit.Framework;

namespace Logy.Maps.Wos.Tests
{
    [TestFixture]
    public class WosManagerTests
    {
        [Test]
        public void DoWork()
        {
            WosManager.DoWork();
        }
    }
}
#endif
