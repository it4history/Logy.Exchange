#if DEBUG
using System.Collections.Generic;
using NUnit.Framework;

namespace Logy.Maps.Tests
{
    [TestFixture]
    public class SortUtilsTests
    {
        private readonly SortedList<int,int> _list = new SortedList<int, int>();

        [Test]
        public void FindFirstIndexGreaterThanOrEqualTo()
        {
            _list.Add(20, 0);
            _list.Add(30, 0);
            _list.Add(50, 0);
            Assert.AreEqual(0, _list.FindFirstIndexGreaterThanOrEqualTo(5));
            Assert.AreEqual(0, _list.FindFirstIndexGreaterThanOrEqualTo(20));
            Assert.AreEqual(1, _list.FindFirstIndexGreaterThanOrEqualTo(25));
            Assert.AreEqual(2, _list.FindFirstIndexGreaterThanOrEqualTo(35));
            Assert.AreEqual(3, _list.FindFirstIndexGreaterThanOrEqualTo(55));
        }
    }
}
#endif
