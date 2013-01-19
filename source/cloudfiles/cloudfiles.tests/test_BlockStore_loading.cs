using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using cloudfiles.blockstore;
using cloudfiles.filesystemcache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_BlockStore_loading
    {
        private const string CACHE_NAME = "testcache";
        private BlockStore _sut;

        [SetUp]
        public void Setup()
        {
            var cache = new FilesystemCache(CACHE_NAME);
            cache.Clear();
            _sut = new BlockStore(cache, 3);
        }


        [Test]
        public void Load_blocks()
        {
            var source = new MemoryStream(Encoding.ASCII.GetBytes("the quick brown fox"));

            var id = Guid.Empty;
            _sut.On_blocks_stored += _ =>
                                         {
                                             id = _.BlockGroupId;
                                             Console.WriteLine("bytes stored: {0}", _.TotalNumberOfBytes);
                                         };
            _sut.Store_blocks(source);

            var result = new MemoryStream();
            _sut.Load_blocks(id, result);

            var content = Encoding.ASCII.GetString(result.ToArray());
            Assert.AreEqual("the quick brown fox", content);
        }
    }
}
