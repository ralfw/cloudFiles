using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using cloudfiles.contract;

namespace cloudfiles.ironiocache.tests
{
    [TestFixture]
    public class test_IronIOCache
    {
        private IronIOCache _sut;

        [SetUp]
        public void Setup()
        {
            var cre = IronIOCredentials.LoadFrom(@"..\..\..\..\..\unversioned\ironcache credentials.txt");
            _sut = new IronIOCache("test", cre);
        }


        [Test]
        public void Integration()
        {
            var value = "";

            _sut.Remove("newkey");
            _sut.Add("newkey", "hello");
            Assert.AreEqual("hello", _sut.Get("newkey"));
            Assert.IsTrue(_sut.TryGet("newkey", out value));
            Assert.AreEqual("hello", value);

            _sut.Remove("newkey");
            Assert.Throws<KeyValueStoreException>(() => _sut.Get("newkey"));
            Assert.IsFalse(_sut.TryGet("newkey", out value));

            _sut.Remove("newkey2");
            _sut.ReplaceOrAdd("newkey2", "world");
            Assert.AreEqual("world", _sut.Get("newkey2"));
            _sut.ReplaceOrAdd("newkey2", "quick brown fox");
            Assert.AreEqual("quick brown fox", _sut.Get("newkey2"));

            _sut.Remove("newcounter");
            Assert.AreEqual(1, _sut.Increment("newcounter", 1));
            Assert.AreEqual(10, _sut.Increment("newcounter", 9));
        }
    }
}
