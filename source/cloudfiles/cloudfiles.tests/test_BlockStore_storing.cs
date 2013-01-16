using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using cloudfiles.blockstore;
using cloudfiles.filesystemcache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_BlockStore_storing
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
        public void Store_block_integration()
        {
            Guid id = Guid.NewGuid();
            Tuple<byte[],int> result = null;

            var block = new Tuple<byte[], int>(new byte[] {1, 2, 3}, 0);
            _sut.Store_block(id, block, _ => result = _);

            Assert.AreEqual(1, Directory.GetFiles(CACHE_NAME).Length);
            Assert.AreEqual(block, result);
        }

        [Test]
        public void Store_blocks_integration()
        {
            var source = new MemoryStream(Encoding.ASCII.GetBytes("1234567890"));
            BlockUploadSummary result = null;
            _sut.On_blocks_stored += _ => result = _;

            _sut.Store_blocks(source);

            Assert.AreEqual(4, Directory.GetFiles(CACHE_NAME).Length);
            Assert.AreNotEqual(Guid.Empty, result.BlockGroupId);
            Assert.AreEqual(10, result.TotalNumberOfBytes);
            Assert.AreEqual(3, result.BlockSize);
            Assert.AreEqual(4, result.NumberOfBlocks);
        }
    }
}
