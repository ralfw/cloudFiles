using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using io.iron.ironcache;

namespace exploring.ironcache
{
    [TestFixture]
    public class Explorer
    {
        private IronCache _cache;

        public void Setup()
        {
            var cre = Credentials.LoadFrom(@"..\..\..\..\..\unversioned\ironcache credentials.txt");
            _cache = new io.iron.ironcache.IronCache(cre.ProjectId, cre.Token);
        }


        [Test]
        public void Adding_key_multiple_times_leaves_original_value_in_place()
        {
            _cache.Add("test", "mykey", "hello");
            _cache.Add("test", "mykey", "world");
            Assert.AreEqual("world", _cache.Get<string>("test", "mykey"));
        }


        [Test]
        public void Getting_non_existent_value_returns_default()
        {
            Assert.AreEqual("no data yet", _cache.Get("test", "nonexistentkey", "no data yet"));
        }


        [Test]
        public void Replace_non_existing_value_causes_exception()
        {
            Assert.Throws<HttpException>(() => _cache.Replace("test", "nonexistentkey", "hello"));
        }


        [Test]
        public void Inc_on_non_existent_key_causes_no_exception_and_returns_inc_amount()
        {
            _cache.Remove("test", "counter");
            Assert.AreEqual(1, _cache.Increment("test", "counter", 1));
        }
    }
}
