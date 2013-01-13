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
        [Test]
        public void Add_and_load()
        {
            var cache = Connect();

            cache.Add("test", "mykey", "hello");
            Assert.AreEqual("hello", cache.Get("test", "mykey", "key not found"));
        }


        [Test]
        public void Get_non_existent_value()
        {
            var cache = Connect();
            Assert.AreEqual("no data yet", cache.Get("test", "nonexistentkey", "no data yet"));
        }


        [Test]
        public void Replace_non_existing_value()
        {
            var cache = Connect();
            Assert.Throws<HttpException>(() => cache.Replace("test", "nonexistentkey", "hello"));
        }


        [Test]
        public void Inc_for_the_first_time()
        {
            var cache = Connect();
            cache.Remove("test", "counter");
            Assert.AreEqual(1, cache.Increment("test", "counter", 1));
        }


        private IronCache Connect()
        {
            var cre = Credentials.LoadFrom(@"..\..\..\..\..\unversioned\ironcache credentials.txt");
            return new io.iron.ironcache.IronCache(cre.ProjectId, cre.Token);
        }
    }
}
