using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using cloudfiles.filesystemcache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_BlockStore
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
        public void Upload_a_single_block()
        {
            var content = new byte[]{1, 2, 3};

            _sut.Upload_block("myblockid", content);

            Assert.IsTrue(File.Exists(CACHE_NAME + @"\myblockid.txt"));
        }

        [Test]
        public void Summarize_blocks()
        {
            var summary = new BlockUploadSummary {BlockGroupId = Guid.NewGuid(), BlockSize = 3};
            BlockUploadSummary result = null;
            _sut.On_blocks_stored += _ => result = _;

            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(new byte[] { 1, 2, 3 }, 0));
            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(new byte[] { 4, 5 }, 1));
            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(null, 2));

            Assert.AreSame(summary, result);
            Assert.AreEqual(summary.BlockGroupId, result.BlockGroupId);
            Assert.AreEqual(5, result.TotalNumberOfBytes);
            Assert.AreEqual(3, result.BlockSize);
            Assert.AreEqual(2, result.NumberOfBlocks);
        }


        [Test]
        public void Store_block_integration()
        {
            var summary = new BlockUploadSummary { BlockGroupId = Guid.NewGuid(), BlockSize = 3 };
            BlockUploadSummary result = null;
            _sut.On_blocks_stored += _ => result = _;

            _sut.Store_block(summary, new Tuple<byte[], int>(new byte[] { 1, 2, 3 }, 0));
            _sut.Store_block(summary, new Tuple<byte[], int>(new byte[] { 4, 5, 6 }, 1));
            _sut.Store_block(summary, new Tuple<byte[], int>(new byte[] { 7, 8 }, 2));
            _sut.Store_block(summary, new Tuple<byte[], int>(null, 3));

            Assert.AreEqual(3, Directory.GetFiles(CACHE_NAME).Length);
            Assert.AreEqual(summary.BlockGroupId, result.BlockGroupId);
            Assert.AreEqual(8, result.TotalNumberOfBytes);
            Assert.AreEqual(3, result.BlockSize);
            Assert.AreEqual(3, result.NumberOfBlocks);
        }


        [Test]
        public void Stream_blocks()
        {
            var result = new List<Tuple<byte[], int>>();
            var source = new MemoryStream(Encoding.ASCII.GetBytes("12345678"));

            _sut.Stream_blocks(source, result.Add);

            Assert.AreEqual(4, result.Count);
            Assert.That(result[0].Item1, Is.EqualTo(new byte[] { 49, 50, 51 }));
            Assert.AreEqual(0, result[0].Item2);
            Assert.That(result[1].Item1, Is.EqualTo(new byte[] { 52, 53, 54 }));
            Assert.AreEqual(1, result[1].Item2);
            Assert.That(result[2].Item1, Is.EqualTo(new byte[] { 55, 56 }));
            Assert.AreEqual(2, result[2].Item2);
            Assert.IsNull(result[3].Item1);
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
